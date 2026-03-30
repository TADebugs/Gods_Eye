# God's Eye - Project Context

## Project Overview
**God's Eye** is an innovative Unity-based dungeon crawler with AI-powered dynamic gameplay using Claude API. The game features a unique dual-persona AI system where Claude acts as both antagonist (God) and helper (Lucifer) simultaneously.

### Core Concept
- **Genre**: First-person dungeon crawler with AI-driven enemy spawning
- **Unique Feature**: Screenshot-based AI analysis - the game captures screenshots and sends them to Claude API, which analyzes the game state and responds as two personas
- **Platform**: Unity 3D (C#)
- **Development Stage**: Active development / prototype phase
- **Timeline**: Started late 2024

### Project Vision & Philosophy
**"The AI doesn't just play the game, it IS the game"**

This project represents a novel intersection of game development and AI - using Vision API not just for analysis, but as a core gameplay mechanic that generates content dynamically based on what it sees. The result is a dungeon crawler where the dungeon master is literally an AI watching your screen, creating emergent, contextually-aware gameplay that feels genuinely intelligent rather than scripted.

### Tech Stack Details
- **Engine:** Unity 6 (6000.0.37f1) with URP (Universal Render Pipeline)
- **Platform:** macOS (M3 MacBook Air, 16GB RAM)
- **IDE:** Cursor (AI-assisted code editor with Copilot-like features)
- **Language:** C# for Unity scripting
- **AI Model:** Anthropic Claude Sonnet 4 (`claude-sonnet-4-20250514`)
- **Development Approach:** Rapid prototyping with AI-assisted coding

## Key Game Mechanics

### Dual Persona AI System
The game's main innovation - Claude API analyzes game screenshots and responds in two voices:

1. **GOD (Antagonist)**
   - Silent, all-powerful controller of the dungeon
   - Views player as an anomaly to eliminate
   - Spawns enemies and controls environment
   - Actions: spawn_white_angel, spawn_red_angel, spawn_fox, teleport_player
   - Strategy: Uses timed spawns and spawn points to challenge player

2. **LUCIFER (Helper, but Limited)**
   - Fallen oracle who wants to help player escape
   - Can ONLY ask questions (cursed to never give direct answers)
   - Uses Socratic questioning to guide players toward solutions
   - Sees everything God sees but is constrained in how he helps

### Enemy Types
- **White Angels**: Basic enforcers that chase and attack (max 3 at once)
- **Red Angels**: Elite enemies with freeze gaze mechanic, 2x damage (max 1 at once)
- **Foxes**: Distraction enemies that flee and summon angels if not killed quickly (max 1 at once)

### Player Systems
- First-person movement (WASD + mouse look)
- Combat system with health tracking
- Attack mechanics
- Health pickups

### Dungeon Generation
- 10x10 procedural grid layout
- Perimeter walls and interior rooms
- Roof at height 3
- Door openings and corridors
- Gold treasure cubes

### Lighting System
- 4 corner point lights with dramatic flickering
- Center illumination light
- Ambient light pulsing
- Directional fill light
- Fog effects (density 0.05f)

## API Cost & Performance Considerations

### Budget & Pricing Model (Pay-per-use)
- **Current Budget:** $5.31 prepaid balance
- **Cost per screenshot:** ~$0.015-0.025
  - Image processing: ~$0.005 per screenshot
  - Text generation: ~$0.01-0.02 depending on response length
- **Estimated capacity:** ~265 screenshots
- **Sufficient for:** 2-week development + demo presentations

### Performance Metrics
- **Screenshot capture:** <50ms
- **PNG encoding:** ~100-200ms
- **API round-trip:** 1-3 seconds (network dependent)
- **Total T-key to response:** ~2-4 seconds

### Why No Real-Time Streaming
**Initial consideration:** Stream video at 60 FPS for continuous AI monitoring

**Cost calculation that killed the idea:**
- 60 screenshots/second × $0.02/screenshot = **$1.20/second**
- 1 minute of gameplay = **$72**
- 10 minutes = **$720**
- Result: **Instant bankruptcy** ❌

**Solution:** Manual trigger (T key press) for strategic AI analysis points

### Future Optimization Possibilities
- Local AI models (LLaVA, Moondream) for future continuous monitoring
- Rate limiting to prevent accidental API spam
- Caching common scene analyses to reduce redundant calls
- Backend proxy for API calls in production

## Technical Architecture

### Core Scripts

#### AI Integration (`Assets/Scripts/AI/`)
- **ClaudeAPIClient.cs** - Handles API communication with Claude
  - Sends base64-encoded screenshots
  - Uses model: claude-sonnet-4-20250514
  - Endpoint: https://api.anthropic.com/v1/messages
  - Max tokens: 4096

- **DualAIController.cs** - Orchestrates the dual persona system
  - Captures screenshots on T key press
  - Builds dual persona prompt with player stats (HP, room, enemies defeated)
  - Parses JSON responses
  - Triggers God's action plan execution
  - Displays Lucifer's questions in UI

- **GodPlanExecutor.cs** - Executes God's action plan
  - Spawns enemies at designated spawn points
  - Handles action delays
  - Manages spawn point locations (wall centers, corners)
  - Enforces spawn limits

- **ScreenshotCapture.cs** - Screenshot capture system

#### Dungeon Systems (`Assets/scripts/`)
- **DungeonManager.cs** - Generates dungeon layout at runtime
- **LightingSetup.cs** - Creates and manages lighting effects
- **SceneInitializer.cs** - Master controller that coordinates managers

#### Player Systems (`Assets/scripts/Player/`)
- **PlayerMovement.cs** - WASD + mouse look controls
- **PlayerHealth.cs** - Health management
- **PlayerAttack.cs** - Combat mechanics

#### Enemy AI (`Assets/scripts/Enemies/`)
- **WhiteAngelAI.cs** - Basic chase/attack behavior
- **RedAngelAI.cs** - Elite enemy with freeze gaze mechanic
- **FoxAI.cs** - Flee behavior and angel summoning

#### Items (`Assets/scripts/Items/`)
- **HealthPickup.cs** - Health restoration items

### Spawn Points Configuration
- **Wall Centers**: north_wall_center, south_wall_center, east_wall_center, west_wall_center
- **Corners**: northeast_corner, northwest_corner, southeast_corner, southwest_corner

## Current State (from CHECKPOINT.md)

### Completed Features
- Dungeon generation (10x10 grid with walls, rooms, roof)
- Interior walls with door openings
- 3 gold treasure cubes
- Dramatic lighting system with flickering effects
- Fog atmosphere
- Scene initialization system
- Player movement and combat
- Claude API client integration
- Screenshot capture system
- Dual persona AI controller

### Configuration
- Corner lights: 3.5f intensity (flickers 1.5-4.5), 15f range
- Center light: 2.5f intensity, 12f range
- Directional light: 0.8f intensity
- Ambient: Pulses RGB(0.15,0.15,0.2) to RGB(0.25,0.25,0.3)
- Fog: 0.05f density, exponential mode

## Development Guidelines

### Development Workflow (Unity + Cursor Integration)
1. **Write code in Cursor** (with AI assistance/suggestions)
2. **Save files** (⌘+S / Ctrl+S) - Unity auto-reloads scripts
3. **Test in Unity Play mode** - Press Play button
4. **Check Console** for logs, errors, and AI responses
5. **Iterate rapidly** with AI-generated code suggestions
6. **Manual git commits** (deferred to key milestones)

**Cursor Setup:**
- External Script Editor set to VS Code in Unity Preferences
- Unity project files (.sln) regenerated for IntelliSense
- Full C# autocomplete and AI code suggestions working

### Unity Editor Setup Required
You CANNOT create GameObjects from terminal alone. Manual Unity Editor setup is required for:
- Creating GameObjects in scene
- Attaching scripts to GameObjects
- Setting up Inspector references
- Configuring component values

(See UNITY_SETUP_GUIDE.md for detailed instructions)

### API Key Management
- API key is stored in ClaudeAPIClient component (Inspector field)
- IMPORTANT: API key should NOT be committed to git
- Consider using environment variables or secure storage for production

### Code Standards
- All scripts use XML documentation comments
- Detailed debug logging with toggleable flags (`showDebugLogs` boolean in Inspector)
- MonoBehaviour lifecycle methods (Start, Update)
- Coroutines for async operations (Unity's async/await wrapped in coroutines)
- Unity's JsonUtility for JSON parsing (no external JSON libraries)
- Frame-rate independent movement using `Time.deltaTime`

### Testing Workflow
1. Press Play in Unity Editor
2. Use WASD + mouse to navigate dungeon
3. Press T key to trigger AI analysis
4. Monitor Unity Console for:
   - "DualAIController: Starting dual persona screenshot capture..."
   - "ClaudeAPIClient: Raw API Response: ..."
   - "GOD ACTS: Spawning [enemy]..."
5. Stop Play mode to iterate on code

## File Organization
```
Assets/
├── Scenes/           # Unity scene files (TestScene.unity)
├── Scripts/
│   ├── AI/          # Claude API and dual persona system
│   ├── Player/      # Player movement, health, combat
│   ├── Enemies/     # Enemy AI behaviors
│   ├── Items/       # Pickups and collectibles
│   └── Testing/     # Test scripts
├── prefabs/         # Reusable GameObjects
├── Settings/        # Project settings
└── TextMesh Pro/    # UI text system

Root/
├── CHECKPOINT.md           # Development checkpoint/progress
├── UNITY_SETUP_GUIDE.md   # Manual Unity setup instructions
└── CLAUDE_CONTEXT.md      # This file
```

## Known Issues & Solutions (Historical Record)

### Issue 1: Light Flickering Too Dramatic ✅ SOLVED
**Problem:** Initial Perlin noise-based flicker was too extreme (0.5-3.0 intensity range causing seizure-like effects)
**Solution:** Removed flickering, using stable 5.0 intensity lights for better visibility
**Status:** Resolved - Stable lights chosen for demo clarity

### Issue 2: Dungeon Too Dark Initially ✅ SOLVED
**Problem:** Claude Vision API reported "too dark to see anything" in initial tests
**Solution:**
- Increased light intensity from 3.0 → 5.0
- Increased range from 10 → 15 units
- Added center fill light (intensity 3.0)
- Brightened ambient lighting
- Reduced fog density from 0.1 → 0.05
**Status:** Resolved - Claude can now clearly see and analyze environment

### Issue 3: .sln File Not Generating ✅ SOLVED
**Problem:** Cursor couldn't find C# project files for IntelliSense/autocomplete
**Solution:**
- Set Unity's External Script Editor to VS Code in Preferences
- Triggered "Regenerate project files" from Assets menu
**Status:** Resolved - Full IntelliSense working in Cursor

### Issue 4: Player Passing Through Walls ✅ SOLVED
**Problem:** No collision detection on Player GameObject
**Solution:** Added CapsuleCollider + Rigidbody with frozen rotation to Player prefab
**Status:** Resolved - Proper physics collisions working

### Current TODO / Active Issues
- Test red angel freeze gaze in actual gameplay scenarios
- Implement fox flee behavior and angel summoning logic fully
- Balance enemy spawn rates based on player performance
- Add player teleportation (currently only logs the action)
- Expand room variations in dungeon generation
- Enemy death callbacks to decrement spawn counters properly
- Create UI system for displaying Lucifer's questions (currently using Console)

## Dependencies
- Unity Engine (3D)
- TextMesh Pro (UI)
- Unity Input System
- Claude API (Anthropic) - Sonnet 4 model

## Git Configuration
- Branch: main
- .gitignore configured for Unity projects
- API keys excluded from version control

## Future Roadmap (Post-MVP)

### Phase 2: Dynamic Content Generation
**Goal:** Claude analyzes screenshots and dynamically modifies dungeon in real-time

**Planned Features:**

1. **Room Templates System**
   - Predefined room types: treasure_room, enemy_room, hallway, boss_arena, puzzle_room
   - Claude decides which template to spawn and in which direction
   - JSON response parsing: `{"action": "spawn_room", "direction": "north", "type": "treasure"}`
   - Templates stored as prefabs for instant instantiation

2. **Enemy Spawning Evolution**
   - `SimpleEnemy.cs` with basic AI (chase player when in range, patrol when distant)
   - Claude decides spawn locations based on visual analysis of player position
   - Enemy count/type scales with player progress and performance
   - Dynamic difficulty adjustment based on player health and enemies defeated

3. **UI System Implementation**
   - On-screen text box displaying Claude's guidance questions
   - Replace Console.Log with proper in-game UI
   - Typing effect for dramatic presentation of Lucifer's questions
   - Fade-in/fade-out animations for immersion

4. **Dual Persona Full Implementation**
   - **God's actions:** Silent (no text), manifests through enemy spawns and room changes
   - **Lucifer's voice:** On-screen questions guiding player toward solutions
   - Separate API calls with different system prompts for each persona
   - God acts first (spawns), then Lucifer comments (questions)

### Phase 3: Polish & Demo Prep
**Goal:** Production-ready demo presentation

**Polish Features:**
- Sound effects (footsteps, ambient dungeon sounds, enemy growls)
- Particle effects (torch flames, enemy spawn VFX, damage indicators)
- Health system UI (health bar, damage flash effect)
- Combat feedback (hit markers, enemy death animations)
- Win/lose conditions (reach exit, defeat boss, or die)
- Title screen and restart functionality
- Pause menu with controls reference
- Performance optimization (object pooling for enemies)

### Phase 4: Future Enhancements (Optional)
- Transition to local AI models (LLaVA, Moondream) for cost-free continuous monitoring
- Multiplayer support (co-op dungeon crawling)
- Persistent progression system (unlocks, upgrades)
- Procedural weapon generation
- Steam release consideration

## Demo Strategy

### Key Selling Points
1. **Novel AI Integration:** "The AI doesn't just give scripted responses - it literally sees what you see and makes strategic decisions based on visual analysis"
2. **Emergent Gameplay:** "Every playthrough is different because Claude generates content based on real-time visual analysis of your actual game state"
3. **Technical Achievement:** "Real-time screenshot analysis with Vision AI in a game engine - bridging web APIs with game development in an unexpected way"

### Demo Script (2-3 minutes)
1. **Introduction** (15 sec) - "God's Eye: Where AI IS the game"
2. **Show movement** (15 sec) - Navigate dungeon, show atmosphere and lighting
3. **Press T, demonstrate AI analysis** (30 sec) - Wait for response, show Console logs
4. **Explain what Claude sees** (30 sec) - Point out specific elements Claude noticed in response
5. **Show dynamic content** (45 sec) - Demonstrate enemy spawns or room changes (if Phase 2 complete)
6. **Vision statement** (30 sec) - "This is just the beginning - imagine AI dungeon masters that truly see and respond to your actions"

### Risk Mitigation
- **Backup plan:** Pre-recorded gameplay footage if API fails during presentation
- **Pre-demo testing:** Ensure API key works and balance is sufficient
- **Console window visible:** Show real API responses to prove it's not fake/scripted
- **Offline mode:** Implement mock API responses for demo if internet fails

## Technical Challenges Overcome

### 1. Unity-Cursor Integration
**Challenge:** Setting up C# development environment outside Unity Editor
**Solution:** Configured VS Code as external editor, regenerated .sln files
**Learning:** Unity's project file generation is essential for IDE integration

### 2. Async in Unity
**Challenge:** Unity's single-threaded execution model vs modern async/await patterns
**Solution:** Wrapped async Tasks in coroutines using `while (!task.IsCompleted) yield return null;`
**Learning:** Never use `.Wait()` or `.GetAwaiter().GetResult()` - they block main thread

### 3. Vision API Format
**Challenge:** Correctly formatting multipart image+text requests as JSON
**Solution:** Manual JSON building with proper base64 encoding and escaping
**Learning:** Claude API expects specific JSON structure with nested content arrays

### 4. Screenshot Pipeline
**Challenge:** Converting Unity's screen content to API-ready format
**Solution:** `ScreenCapture.CaptureScreenshotAsTexture()` → `EncodeToPNG()` → Base64
**Learning:** Must wait one frame after capture before encoding texture

### 5. Lighting Balance
**Challenge:** Finding sweet spot between atmospheric darkness and functional visibility for AI
**Solution:** Iterative testing with Claude - adjusted until AI could see clearly
**Learning:** What looks good to humans may be too dark for AI vision systems

## Lessons Learned

1. **API costs matter:** Streaming would cost $72/min - careful consideration of API usage patterns is critical
2. **Lighting is crucial:** Dark atmosphere vs functional visibility requires careful balance, especially for AI vision
3. **Unity quirks:** .sln generation, component attachment, prefab workflow all have learning curves
4. **AI coding tools acceleration:** Cursor dramatically accelerates Unity learning for developers from other backgrounds
5. **Scope management:** MVP first (working AI integration) before feature creep - tight timeline demands focus
6. **Async patterns in game engines:** Game loops have unique threading constraints compared to web servers
7. **Vision AI feedback loop:** Testing with actual AI responses reveals issues humans might miss (e.g., "too dark")

## Why This Project Is Impressive

### For Game Development
- Demonstrates understanding of Unity fundamentals (GameObjects, Components, Prefabs, Scene hierarchy)
- Shows procedural generation techniques (runtime dungeon creation)
- Implements real-time lighting and atmospheric effects
- Uses proper game architecture patterns (managers, controllers, FSM for enemies)

### For AI/ML Integration
- Practical application of Vision AI in unexpected domain (games, not typical CV tasks like object detection)
- Handles async API communication within game loop constraints
- Cost-aware architecture (understands API pricing implications and designs accordingly)
- Innovative use of AI as game mechanic rather than just support tool

### For Software Engineering
- Clean code organization with separation of concerns
- Manager pattern for scene orchestration
- Comprehensive error handling and edge case management
- Rapid prototyping with AI-assisted development
- Documented decision-making process and technical debt awareness

## Important Notes
- The dual persona system is the core innovation - keep this concept central
- Lucifer is a LIMITED helper, not omnipotent - questions only, no direct answers
- God's plan should escalate difficulty based on player performance
- Red angels are elite enemies - use strategically, not constantly
- Screenshot capture on T key is the main AI trigger mechanism
- JSON responses must follow exact structure for parsing to work

## Quick Start for Claude Code Sessions
When starting a new session:
1. Check recent git commits to see what's changed
2. Read CHECKPOINT.md for current development state
3. Reference this file for architecture and design decisions
4. Use UNITY_SETUP_GUIDE.md if working on Unity Editor setup
5. Check Console logs in Unity for runtime errors
6. Test AI integration by pressing T key in Play mode

---

**Last Updated:** 2025-11-15 (Comprehensive update with Claude AI chat context)
**Project Started:** Late 2024
**Current Phase:** MVP complete, working on Phase 2 features
