# Unity Scene Setup Guide

## Important Note
**You CANNOT create GameObjects from terminal or code alone.** Unity requires the Unity Editor to:
- Create GameObjects in the scene
- Attach scripts/components to GameObjects
- Set up references between scripts
- Configure Inspector values

The scripts you've written will **generate** GameObjects at runtime (like the dungeon cubes and lights), but you need to manually set up the initial manager GameObjects in the Unity Editor.

---

## Step-by-Step Setup Instructions

### Step 1: Open Your Unity Project
1. Open Unity Hub
2. Open your "God's Eye" project
3. Wait for Unity to finish importing/compiling scripts

### Step 2: Open Your Scene
1. In the **Project** window (bottom), navigate to: `Assets/Scenes/`
2. Double-click `TestScene.unity` to open it (or create a new scene if needed)

### Step 3: Create the Manager GameObjects

You need to create **3 empty GameObjects** in the Hierarchy:

#### 3a. Create "SceneInitializer" GameObject
1. In the **Hierarchy** window (left side), right-click in empty space
2. Select **Create Empty** (or press `Ctrl+Shift+N` / `Cmd+Shift+N` on Mac)
3. Name it: `SceneInitializer`
4. In the **Inspector** window (right side), click **Add Component**
5. Search for and select: `Scene Initializer` (your script)
6. The script should now be attached

#### 3b. Create "DungeonManager" GameObject
1. Right-click in Hierarchy → **Create Empty**
2. Name it: `DungeonManager`
3. In Inspector, click **Add Component**
4. Search for and select: `Dungeon Manager`
5. The script should now be attached

#### 3c. Create "LightingSetup" GameObject
1. Right-click in Hierarchy → **Create Empty**
2. Name it: `LightingSetup`
3. In Inspector, click **Add Component**
4. Search for and select: `Lighting Setup`
5. The script should now be attached

### Step 4: Wire Up References in SceneInitializer

This is the **critical step** that connects everything:

1. Select the **SceneInitializer** GameObject in the Hierarchy
2. Look at the **Inspector** window - you should see:
   - A section called "Scene Managers"
   - Two empty fields: "Dungeon Manager" and "Lighting Setup"
3. **Drag and drop** the GameObjects:
   - Drag `DungeonManager` from Hierarchy → drop into "Dungeon Manager" field
   - Drag `LightingSetup` from Hierarchy → drop into "Lighting Setup" field

**Alternative method** (if drag-drop doesn't work):
- Click the small circle icon next to each field
- A window will pop up - search for "DungeonManager" or "LightingSetup"
- Select the GameObject from the list

### Step 5: Verify Your Setup

Your Hierarchy should look like this:
```
TestScene
├── Main Camera
├── Directional Light (default Unity light - you can delete this)
├── SceneInitializer
├── DungeonManager
└── LightingSetup
```

Your SceneInitializer Inspector should show:
- **Scene Managers**
  - Dungeon Manager: `DungeonManager` (GameObject)
  - Lighting Setup: `LightingSetup` (GameObject)

### Step 6: Test It!

1. Click the **Play** button (▶️) at the top center of Unity
2. Watch the Console window (Window → General → Console) for any errors
3. In the Scene view, you should see:
   - A 10x10 floor grid appear
   - Walls around the perimeter
   - 4 orange point lights at the corners
   - A directional light from above

### Step 7: Stop Play Mode

1. Click the **Play** button again to stop
2. Note: Any GameObjects created during Play mode will be deleted when you stop

---

## Troubleshooting

### Scripts Not Showing Up in Add Component?
- Wait for Unity to finish compiling (check bottom-right corner)
- Make sure scripts are in `Assets/scripts/` or `Assets/Scripts/`
- Check Console for compilation errors

### References Not Working?
- Make sure you're dragging the **GameObject** (not the script file)
- The GameObject must have the script component attached
- Check that field names match exactly

### Nothing Happens When Playing?
- Check the Console for warnings/errors
- Verify SceneInitializer has both references assigned
- Make sure you're in the correct scene

### Can't See the Dungeon?
- Switch to **Scene** view (not Game view)
- Press `F` to focus on selected object
- Check that camera is positioned correctly

---

## Quick Reference: What Each Script Does

- **SceneInitializer**: Master controller - calls Initialize() on both managers
- **DungeonManager**: Generates floor tiles and walls at runtime
- **LightingSetup**: Sets up ambient light and creates point/directional lights at runtime

---

## Next Steps

Once this is working, you can:
- Adjust lighting colors/intensity in `LightingSetup.cs`
- Change dungeon size in `DungeonManager.cs`
- Add more managers and wire them up in `SceneInitializer.cs`

