# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**God's Eye** is a Unity-based first-person dungeon crawler with AI-driven dynamic gameplay. The game's core innovation is a **dual-persona AI system** where Claude API analyzes game screenshots and simultaneously acts as:

1. **GOD (Antagonist)**: Silent antagonist who spawns enemies and manipulates the environment to eliminate the player
2. **LUCIFER (Helper)**: Fallen oracle who wants to help but is cursed to only ask Socratic questions, never give direct answers

The game captures screenshots during gameplay (T key), sends them to Claude API with player stats, receives JSON responses with God's action plan and Lucifer's question, then executes the actions in-game.

## High-Level Architecture

### Screenshot → AI → Game State Flow

```
Player presses T key
    ↓
DualAIController captures screenshot
    ↓
ClaudeAPIClient sends screenshot + dual persona prompt to Claude API
    ↓
Claude responds with JSON: { god_plan: [...actions], lucifer_question: "..." }
    ↓
DualAIController parses JSON
    ↓
GodPlanExecutor executes actions (spawn enemies with delays)
    ↓
Lucifer's question displays in UI
```

### Core System Relationships

**AI Integration Layer** (`Assets/Scripts/AI/`)
- `DualAIController` - Orchestrates the entire flow, owns the screenshot capture coroutine
- `ClaudeAPIClient` - Handles HTTP communication with Claude API (base64 encoding, JSON serialization)
- `GodPlanExecutor` - Executes God's plan by instantiating enemy prefabs at spawn points with delay coroutines
- `ScreenshotCapture` - Legacy standalone screenshot system (DualAIController handles this now)

**Dungeon Systems** (`Assets/scripts/`)
- `SceneInitializer` - Master coordinator that initializes DungeonManager and LightingSetup on Start()
- `DungeonManager` - Runtime procedural generation (10x10 grid, walls, rooms, treasure cubes)
- `LightingSetup` - Runtime lighting generation (corner lights with flickering, ambient pulsing, fog)

**Enemy AI** (`Assets/scripts/Enemies/`)
- All enemies auto-find player via `GameObject.FindGameObjectWithTag("Player")`
- `WhiteAngelAI` - Basic chase/attack FSM (Idle → Chase → Attack based on distance)
- `RedAngelAI` - Elite enemy with freeze gaze: checks eye contact via `Vector3.Angle(cameraForward, directionToAngel)` and freezes both player and self
- `FoxAI` - Flee behavior with angel summoning

**Player Systems** (`Assets/scripts/Player/`)
- `PlayerMovement` - WASD + mouse look, has `Freeze()`/`Unfreeze()` methods for red angel gaze mechanic
- `PlayerHealth` - Health management with `TakeDamage()` method
- `PlayerAttack` - Combat mechanics

### Critical Design Constraints

**Enemy Spawn Limits** (enforced in `GodPlanExecutor`)
- White Angels: max 3 active (tracked via `activeWhiteAngels` counter)
- Red Angels: max 1 active (tracked via `activeRedAngels` counter)
- Foxes: no limit
- Counters must be decremented when enemies are destroyed (call `OnWhiteAngelDestroyed()` / `OnRedAngelDestroyed()`)

**Spawn Points** (hardcoded Vector3 positions in `GodPlanExecutor`)
- Wall centers: `north_wall_center`, `south_wall_center`, `east_wall_center`, `west_wall_center`
- Corners: `northeast_corner`, `northwest_corner`, `southeast_corner`, `southwest_corner`
- Dungeon floor grid: (0,0) to (9,9), spawn points are 1 unit inward from walls

**Claude API Configuration** (`ClaudeAPIClient`)
- Model: `claude-sonnet-4-20250514`
- Endpoint: `https://api.anthropic.com/v1/messages`
- Max tokens: 4096
- API key stored in Inspector field (must be set manually in Unity Editor)
- Request format: multipart content with base64 PNG image + text prompt
- Response format: strict JSON `{ "god_plan": [...], "lucifer_question": "..." }`

**Dual Persona System Prompt** (`DualAIController.GetDualPersonaSystemPrompt()`)
- Includes player stats: HP, current room, enemies defeated
- Defines God's available actions: `spawn_white_angel`, `spawn_red_angel`, `spawn_fox`, `teleport_player`
- Defines Lucifer's constraint: only questions, no direct answers (he's helping but cursed)
- Enforces JSON-only response (no markdown code blocks)

### JSON Response Structure

```json
{
  "god_plan": [
    {"action": "spawn_white_angel", "delay_seconds": 0, "spawn_point": "north_wall_center"},
    {"action": "spawn_red_angel", "delay_seconds": 30, "spawn_point": "south_wall_center"}
  ],
  "lucifer_question": "Do you notice the pattern in how the shadows move along the northern wall?"
}
```

Actions execute sequentially with `WaitForSeconds(delay_seconds)` between each.

## Unity Editor Requirements

**You cannot create GameObjects or modify Unity scenes from the terminal.** Changes to:
- Scene hierarchy (creating GameObjects, attaching components)
- Inspector field assignments (prefab references, component links)
- Scene file modifications

Must be done manually in Unity Editor. See `UNITY_SETUP_GUIDE.md` for detailed instructions.

**Required Manual Setup:**
1. Create GameObjects for SceneInitializer, DungeonManager, LightingSetup
2. Attach corresponding scripts to each GameObject
3. Wire up references in SceneInitializer Inspector (drag DungeonManager, LightingSetup GameObjects)
4. Set API key in ClaudeAPIClient Inspector field
5. Assign enemy prefabs in GodPlanExecutor Inspector (whiteAngelPrefab, redAngelPrefab, foxPrefab)

## Development Commands

**Testing in Unity:**
- Open `Assets/Scenes/TestScene.unity` in Unity Editor
- Press Play button to enter Play mode
- Press `T` key in Play mode to trigger AI screenshot analysis
- Check Console window for debug logs (all scripts have `showDebugLogs` toggle)

**Building:**
- Use Unity Editor: File → Build Settings → Build
- No command-line build scripts currently configured

**Running Tests:**
- No automated test suite currently exists

**Git Workflow:**
- Main branch: `main`
- API keys excluded via `.gitignore`
- Scene files tracked (but API key references in scene should be null)

## Key Implementation Details

### Red Angel Freeze Gaze Mechanic

The freeze gaze works by:
1. Red angel checks eye contact every `gazeCheckInterval` (0.1s) in `CheckEyeContact()`
2. Calculates angle between camera forward vector and direction to angel using `Vector3.Angle()`
3. If angle ≤ `gazeDetectionAngle` (45°), triggers mutual freeze
4. Calls `PlayerMovement.Freeze()` and sets `isFrozen = true` on angel for `freezeDuration` (3s)
5. After freeze ends, 2s cooldown prevents immediate re-triggering

**Why this matters:** If modifying enemy behavior, preserve the freeze/unfreeze pattern and the cooldown mechanism.

### Async API Communication Pattern

`ClaudeAPIClient.SendScreenshot()` is async but Unity coroutines are used for coordination:
1. `DualAIController.CaptureAndSendDualPersonaCoroutine()` captures screenshot (sync)
2. Calls `claudeAPIClient.SendScreenshot()` (returns `Task<string>`)
3. Waits in coroutine: `while (!apiTask.IsCompleted) yield return null;`
4. Gets result: `string rawResponse = apiTask.Result;`

**Why this matters:** Unity's main thread must remain responsive. Don't use `.Wait()` or `.GetAwaiter().GetResult()` as they block the main thread.

### JSON Response Cleaning

Claude sometimes returns markdown code blocks despite instructions. `DualAIController.CleanJsonResponse()`:
1. Strips `\`\`\`json` and `\`\`\`` markers
2. Finds first `{` and last `}` to extract pure JSON
3. Parses with `JsonUtility.FromJson<DualAIResponse>()`

**Why this matters:** If adding new JSON parsing, use the same cleaning pattern.

### Enemy Counter Management

`GodPlanExecutor` tracks `activeWhiteAngels` and `activeRedAngels` to enforce spawn limits. Enemy death scripts **must** call:
- `godPlanExecutor.OnWhiteAngelDestroyed()` when white angel dies
- `godPlanExecutor.OnRedAngelDestroyed()` when red angel dies

**Why this matters:** If counters aren't decremented, spawn limits will be reached and no more enemies will spawn.

## Current Development State

**Completed:**
- Dungeon generation with 10x10 grid, walls, rooms, doors, treasure cubes
- Dramatic lighting with flickering corner lights, ambient pulsing, fog
- Scene initialization system
- Player movement (WASD + mouse look) and combat (health, attack)
- Claude API client integration with base64 image encoding
- Screenshot capture system (T key)
- Dual persona AI controller with JSON parsing
- God plan executor with timed enemy spawning
- White angel chase/attack AI
- Red angel freeze gaze mechanic
- Fox AI structure

**Known Issues / TODO:**
- Test red angel freeze gaze in actual gameplay
- Implement fox flee behavior and angel summoning fully
- Balance enemy spawn rates and difficulty
- Add player teleportation (currently just logs)
- Expand room variations in dungeon generation
- Enemy death callbacks to decrement spawn counters

## Important Files to Check

When debugging or extending functionality:
- `CHECKPOINT.md` - Development checkpoint with completed features and next steps
- `UNITY_SETUP_GUIDE.md` - Step-by-step Unity Editor setup instructions
- `CLAUDE_CONTEXT.md` - Detailed project context (more comprehensive than this file)
- Unity Console - Runtime errors and debug logs (all scripts have extensive logging)

## Testing the AI System

1. Set API key in `ClaudeAPIClient` component Inspector
2. Open `TestScene.unity` in Unity Editor
3. Enter Play mode
4. Press `T` to capture screenshot and trigger AI analysis
5. Check Console for:
   - "DualAIController: Starting dual persona screenshot capture..."
   - "ClaudeAPIClient: Raw API Response: ..."
   - "GOD ACTS: Spawning [enemy] at [spawn_point]..."
   - "Lucifer's question displayed: ..."
6. Verify enemies spawn at designated positions after delays
7. Verify Lucifer's question appears in UI

## API Key Security

**NEVER commit API keys to git.** The API key is set via Unity Inspector field and should be excluded from scene commits. For production:
- Use environment variables
- Implement secure storage
- Consider backend proxy for API calls
