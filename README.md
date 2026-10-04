# God's Eye

A first-person dungeon crawler in Unity where a vision model plays the dungeon master. Press **T** and the game sends a screenshot of what you see to Claude; one response comes back as two personas: **GOD**, who silently plans enemy spawns and teleports as timed JSON actions, and **LUCIFER**, a cursed helper who can only ask you questions. The game parses the plan and executes it: 5–10 timed AI actions per vision plan, with no scripted spawns. The vision round-trip itself takes 1–3 seconds.

**Live:** https://tanmaydesai.xyz/gods-eye

## Screenshots

<!-- PLACEHOLDER: screenshot -->

No screenshot images exist in this repo yet.

## Stack

- Unity 6 (`6000.0.37f1`), Universal Render Pipeline (`ProjectSettings/ProjectVersion.txt`)
- C#, `UnityWebRequest` for HTTP, `JsonUtility` for parsing
- Anthropic Messages API (`https://api.anthropic.com/v1/messages`), model `claude-sonnet-4-20250514`, vision input
- TextMeshPro for the LUCIFER text

## Architecture

```
T key
  → DualAIController captures a screenshot (base64 image)
  → ClaudeAPIClient POSTs it, with the dual-persona prompt, to the vision model
  → model returns JSON: { "god_plan": [ {action, delay_seconds, spawn_point}, ... ],
                          "lucifer_question": "..." }
  → DualAIController parses it
  → GodPlanExecutor runs each god_plan action after its delay_seconds
  → LUCIFER's question is shown in the UI
```

- **GOD actions** (`GodPlanExecutor.cs`): `spawn_white_angel`, `spawn_red_angel`, `spawn_fox`, `teleport_player`. Spawn points are hardcoded wall centers and corners of the 10×10 dungeon.
- **Caps:** 3 White Angels and 1 Red Angel at a time.
- **Enemies:** White Angels chase the player. The Red Angel's freeze gaze freezes both it and the player for 3 seconds on eye contact. The Fox flees, and if left alive for 60 seconds it summons an angel (the `summonTimer` field has a comment noting it was raised for testing).
- **Why T-press and not continuous:** `CLAUDE_CONTEXT.md` records ~$0.015–0.025 per screenshot, so streaming 60 per second was rejected as about $72 per minute.

Code is in `God's Eye/Assets/scripts/` (`AI/`, `Enemies/`, `Player/`, `Items/`).

## Run it

Requires Unity Hub with editor version `6000.0.37f1` and an [Anthropic API key](https://console.anthropic.com/).

1. Clone: `git clone https://github.com/TADebugs/Gods_Eye.git`
2. In Unity Hub, **Add** the `God's Eye` folder and open it.
3. Open `Assets/Scenes/TestScene.unity`.
4. Select the GameObject with the `ClaudeAPIClient` component and paste your key into its `Api Key` field. It is a serialized field defaulting to `YOUR_API_KEY_HERE`, and the key in the committed scene is empty. Don't commit your key.
5. Press Play. Move with **WASD** and the mouse, attack with **Space**, press **T** to send a screenshot.

I haven't run the project in this environment (no Unity editor), so these steps come from reading the project files, not from a test run.

## Docs in this repo

- `God's Eye/CLAUDE.md`, `CLAUDE_CONTEXT.md`, `CHECKPOINT.md`, `UNITY_SETUP_GUIDE.md`: development notes
- `God's Eye — Research Paper/`: `.docx` research outline and references

## License

No license file yet. Until one is added, all rights are reserved by the author.
