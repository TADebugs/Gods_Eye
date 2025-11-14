# 🎮 God's Eye

*Where AI isn't your assistant—it's your omnipotent adversary and your cursed ally.*

---

## 🌟 What Makes This Different

**God's Eye** is a revolutionary dungeon crawler where artificial intelligence doesn't just guide you—it **controls the game itself**. Using Claude's Vision API, the game analyzes real-time screenshots to dynamically spawn enemies, control gameplay mechanics, and guide the player through a unique dual-persona AI system.

Unlike traditional games with static enemy spawns or scripted events, every playthrough is unique. The AI watches you play, analyzes your situation, and makes strategic decisions in real-time. Press **T** to send a screenshot, and watch as the game responds to your actions.

---

## 🎭 The Dual Persona System

### **GOD - The Architect** (Antagonist)
- **Silent, omnipotent controller** of the dungeon
- Analyzes game state through vision and spawns threats strategically
- Commands three types of enemies with tactical precision
- **Never speaks—only acts** through game mechanics
- Views the player as an anomaly to be eliminated
- Controls the environment, teleports, and orchestrates chaos

### **LUCIFER - The Cursed Oracle** (Helper)
- **Sees everything God does** (shared omniscient vision)
- Wants to help the player escape/rebel against the system
- **Cursed**: Can **ONLY ask questions**, never give direct answers
- Guides through Socratic method—helping you discover solutions yourself
- Your reluctant ally in the Matrix, working within brutal constraints
- Tone: Urgent, concerned, trying to help despite the curse

---

## 🎯 Key Features

### **AI-Driven Gameplay**
- Press **T** to capture and send a screenshot to Claude
- AI analyzes the scene and makes strategic decisions in real-time
- Dynamic difficulty scaling based on player performance

### **Dynamic Enemy Spawning**
- God spawns enemies with timed delays based on real-time game analysis
- Strategic positioning at wall centers and corners
- Maximum limits: 3 White Angels, 1 Red Angel, 1 Fox at a time

### **Three Enemy Types**

#### **White Angels** 👼
- Standard chasers that hunt the player
- Fast movement speed (3 units/sec)
- Attack damage: 1 HP per hit
- Chase range: 15 units, Attack range: 2 units

#### **Red Angels** 😈
- Elite units with **freeze gaze mechanic**
- **Mutual paralysis**: When you make eye contact, both you and the Red Angel freeze for 3 seconds
- Slower but deadlier (2.5 units/sec, 20 damage per hit)
- Strategic spawns for maximum psychological impact

#### **White Fox** 🦊
- Fast distraction enemy (5 units/sec)
- Flees from the player when approached
- **Summons reinforcements**: If alive for 60 seconds, howls and spawns a White Angel
- **Drops health pickup** on death (if killed before summoning)

### **Dual AI Responses**
- One API call returns both:
  - **God's action plan** (JSON format with spawn commands and delays)
  - **Lucifer's guiding question** (text displayed in UI)

### **Full Combat System**
- Player health system (100 HP default)
- Space bar to attack enemies (raycast-based)
- Health pickups dropped by defeated foxes
- Death triggers scene reload after 2 seconds

### **Atmospheric Dungeon**
- Procedurally generated 10x10 dungeon with interior walls
- Dynamic flickering torch lighting
- Fog effects for depth and atmosphere
- Treasure objects scattered throughout

---

## 🛠️ Tech Stack

- **Unity** - Game Engine
- **C#** - Game Logic & AI Integration
- **Claude Sonnet 4** - AI Decision Making via Vision API
- **Claude Vision API** - Real-time Screenshot Analysis
- **TextMeshPro** - UI Text Rendering

---

## 🎮 How It Works

1. **Player presses T** → Game captures screenshot of current camera view
2. **Screenshot sent to Claude** with dual-persona system prompt including:
   - Current player stats (HP, room, enemies defeated)
   - Game state context
   - Spawn point locations
   - Action constraints
3. **Claude responds as both God and Lucifer simultaneously**:
   - **God**: Returns JSON action plan with enemy spawns, teleports, and delays
   - **Lucifer**: Returns a guiding question to help the player
4. **Game parses response**:
   - Extracts God's plan and executes actions with specified delays
   - Displays Lucifer's question in UI
5. **Actions execute over time**:
   - Enemies spawn at designated locations
   - Player may be teleported
   - Environment changes based on God's will

---

## 🚀 Setup

### Prerequisites
- Unity 2022.3 LTS or later
- Claude API Key from [Anthropic](https://console.anthropic.com/)
- Git

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/TADebugs/Gods_Eye.git
cd Gods_Eye
```

2. **Open in Unity**
   - Open Unity Hub
   - Click "Add" and select the `God's Eye` folder
   - Open the project

3. **Configure API Key**
   - Navigate to `Assets/Scripts/AI/ClaudeAPIClient.cs`
   - Replace `"YOUR_API_KEY_HERE"` with your Claude API key
   - Or set it in the Unity Inspector

4. **Scene Setup** (if needed)
   - Open `Assets/Scenes/TestScene.unity`
   - Ensure the SceneInitializer GameObject has references to:
     - DungeonManager
     - LightingSetup
   - Ensure DualAIController has references to:
     - ClaudeAPIClient
     - GodPlanExecutor
     - Lucifer Question Text (TextMeshProUGUI)

5. **Player Setup**
   - Tag your player GameObject as "Player"
   - Attach scripts:
     - `PlayerMovement.cs`
     - `PlayerHealth.cs`
     - `PlayerAttack.cs`
   - Ensure player has a Camera tagged as "MainCamera"

6. **Enemy Prefabs**
   - Create prefabs for White Angel, Red Angel, and Fox
   - Tag them as "Enemy"
   - Assign to `GodPlanExecutor` in Inspector

7. **Press Play** and press **T** to trigger the AI system!

---

## 🎮 Controls

- **WASD** - Move
- **Mouse** - Look around
- **Space** - Attack
- **T** - Capture screenshot and send to AI (triggers dual-persona system)

---

## 📁 Project Structure

```
God's Eye/
├── Assets/
│   ├── Scripts/
│   │   ├── AI/
│   │   │   ├── ClaudeAPIClient.cs       # API communication
│   │   │   ├── DualAIController.cs      # Dual persona orchestration
│   │   │   ├── GodPlanExecutor.cs       # Executes God's actions
│   │   │   └── ScreenshotCapture.cs     # Screenshot handling
│   │   ├── Enemies/
│   │   │   ├── WhiteAngelAI.cs          # Standard enemy
│   │   │   ├── RedAngelAI.cs            # Elite enemy with freeze
│   │   │   └── FoxAI.cs                 # Distraction enemy
│   │   ├── Player/
│   │   │   ├── PlayerMovement.cs         # Movement & freeze mechanic
│   │   │   ├── PlayerHealth.cs           # Health system
│   │   │   └── PlayerAttack.cs          # Combat system
│   │   ├── Items/
│   │   │   └── HealthPickup.cs           # Collectible health
│   │   ├── scripts/
│   │   │   ├── DungeonManager.cs         # Procedural generation
│   │   │   ├── LightingSetup.cs         # Atmospheric lighting
│   │   │   └── SceneInitializer.cs       # Scene setup
│   │   └── Testing/
│   │       └── RedAngelTestSpawner.cs    # Testing utilities
│   └── Scenes/
│       └── TestScene.unity               # Main game scene
└── README.md
```

---

## 🔧 Configuration

### AI Behavior
- Edit `DualAIController.cs` to modify the dual-persona prompt
- Adjust spawn constraints in `GodPlanExecutor.cs`
- Modify enemy behavior in respective AI scripts

### Gameplay Balance
- Player health: `PlayerHealth.cs` → `maxHealth`
- Enemy damage: Individual AI scripts → `attackDamage`
- Spawn limits: `GodPlanExecutor.cs` → `MAX_WHITE_ANGELS`, etc.

### Lighting & Atmosphere
- Flicker intensity: `LightingSetup.cs` → `FLICKER_MIN`, `FLICKER_MAX`
- Fog density: `LightingSetup.cs` → `fogDensity`
- Light positions and colors: `LightingSetup.cs` → `SetupPointLights()`

---

## 🐛 Known Issues & Future Improvements

### Current Limitations
- API calls are synchronous (may cause brief freezes)
- Enemy spawning uses hardcoded positions
- No save/load system

### Planned Features
- [ ] Asynchronous API calls for smoother gameplay
- [ ] Procedural spawn point generation
- [ ] Save/load system
- [ ] More enemy types
- [ ] Environmental hazards controlled by God
- [ ] Lucifer's questions affecting gameplay mechanics
- [ ] Multiple dungeon rooms/levels

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

---

## 📝 License

This project is open source and available under the MIT License.

---

## 🙏 Acknowledgments

- **Anthropic** for Claude AI and Vision API
- **Unity Technologies** for the game engine
- Inspired by the concept of AI as both adversary and guide

---

## 📧 Contact

For questions or feedback, please open an issue on GitHub.

---

**Remember**: In God's Eye, the AI is always watching. Every screenshot you send is analyzed, every action you take is observed. Will you survive God's will, or will Lucifer's questions guide you to freedom?

*Press T to find out.*

