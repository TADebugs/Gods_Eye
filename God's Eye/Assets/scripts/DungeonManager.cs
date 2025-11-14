using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    // Material colors for floor and walls
    private Color floorColor = new Color(0.2f, 0.2f, 0.2f); // Dark gray (RGB: 0.2, 0.2, 0.2)
    private Color wallColor = new Color(0.4f, 0.4f, 0.4f);  // Lighter gray (RGB: 0.4, 0.4, 0.4)
    
    void Start()
    {
        Initialize();
    }
    
    /// <summary>
    /// Public initialization method that can be called by SceneInitializer
    /// </summary>
    public void Initialize()
    {
        // Create the parent GameObject to organize all dungeon elements
        GameObject dungeonParent = new GameObject("Dungeon");
        
        // Generate the floor
        GenerateFloor(dungeonParent.transform);
        
        // Generate the walls around the perimeter
        GenerateWalls(dungeonParent.transform);
        
        // Generate interior walls to create rooms/corridors
        GenerateInteriorWalls(dungeonParent.transform);
        
        // Generate treasure objects
        GenerateTreasures(dungeonParent.transform);
        
        // Generate the roof
        GenerateRoof(dungeonParent.transform);
    }
    
    /// <summary>
    /// Generates a solid 10x10 floor made of individual cube GameObjects with no gaps
    /// </summary>
    private void GenerateFloor(Transform parent)
    {
        // Create 10x10 solid floor grid
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                GameObject floorTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floorTile.name = $"FloorTile_{x}_{z}";
                floorTile.transform.position = new Vector3(x, 0, z);
                floorTile.transform.localScale = new Vector3(1, 0.1f, 1);
                floorTile.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
                floorTile.transform.SetParent(parent);
                
                // Verify BoxCollider exists (CreatePrimitive adds it automatically)
                if (floorTile.GetComponent<BoxCollider>() == null)
                {
                    floorTile.AddComponent<BoxCollider>();
                }
            }
        }
    }
    
    /// <summary>
    /// Generates walls around the perimeter of the dungeon, positioned outside the 10x10 grid
    /// </summary>
    private void GenerateWalls(Transform parent)
    {
        // Wall scale: 1x3x1 (tall walls)
        Vector3 wallScale = new Vector3(1f, 3f, 1f);
        
        // Wall height offset (half of wall height to sit on floor)
        float wallYPosition = 1.5f;
        
        // North wall: x from 0-9, z = 10
        for (int x = 0; x < 10; x++)
        {
            GameObject northWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            northWall.name = $"NorthWall_{x}";
            northWall.transform.position = new Vector3(x, wallYPosition, 10);
            northWall.transform.localScale = wallScale;
            northWall.transform.SetParent(parent);
            northWall.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 0.4f);
            
            // Verify BoxCollider exists
            if (northWall.GetComponent<BoxCollider>() == null)
            {
                northWall.AddComponent<BoxCollider>();
            }
        }
        
        // South wall: x from 0-9, z = -1
        for (int x = 0; x < 10; x++)
        {
            GameObject southWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            southWall.name = $"SouthWall_{x}";
            southWall.transform.position = new Vector3(x, wallYPosition, -1);
            southWall.transform.localScale = wallScale;
            southWall.transform.SetParent(parent);
            southWall.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 0.4f);
            
            // Verify BoxCollider exists
            if (southWall.GetComponent<BoxCollider>() == null)
            {
                southWall.AddComponent<BoxCollider>();
            }
        }
        
        // East wall: x = 10, z from 0-9
        for (int z = 0; z < 10; z++)
        {
            GameObject eastWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eastWall.name = $"EastWall_{z}";
            eastWall.transform.position = new Vector3(10, wallYPosition, z);
            eastWall.transform.localScale = wallScale;
            eastWall.transform.SetParent(parent);
            eastWall.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 0.4f);
            
            // Verify BoxCollider exists
            if (eastWall.GetComponent<BoxCollider>() == null)
            {
                eastWall.AddComponent<BoxCollider>();
            }
        }
        
        // West wall: x = -1, z from 0-9
        for (int z = 0; z < 10; z++)
        {
            GameObject westWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            westWall.name = $"WestWall_{z}";
            westWall.transform.position = new Vector3(-1, wallYPosition, z);
            westWall.transform.localScale = wallScale;
            westWall.transform.SetParent(parent);
            westWall.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 0.4f);
            
            // Verify BoxCollider exists
            if (westWall.GetComponent<BoxCollider>() == null)
            {
                westWall.AddComponent<BoxCollider>();
            }
        }
    }
    
    /// <summary>
    /// Generates interior walls to create rooms and corridors
    /// </summary>
    private void GenerateInteriorWalls(Transform parent)
    {
        // Wall scale: 1x3x1 (tall walls, same as perimeter walls)
        Vector3 wallScale = new Vector3(1f, 3f, 1f);
        
        // Wall height offset (half of wall height to sit on floor)
        float wallYPosition = 1.5f;
        
        // Vertical dividing wall at x=5, from z=2 to z=7
        // Skip z=4 and z=5 to create a doorway (2-unit gap)
        for (int z = 2; z <= 7; z++)
        {
            // Skip z=4 and z=5 to create the door opening
            if (z == 4 || z == 5)
            {
                continue;
            }
            
            GameObject interiorWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            interiorWall.name = $"InteriorWall_Vertical_{z}";
            interiorWall.transform.position = new Vector3(5, wallYPosition, z);
            interiorWall.transform.localScale = wallScale;
            interiorWall.transform.SetParent(parent);
            interiorWall.GetComponent<Renderer>().material.color = wallColor; // Same color as perimeter walls
            
            // Verify BoxCollider exists
            if (interiorWall.GetComponent<BoxCollider>() == null)
            {
                interiorWall.AddComponent<BoxCollider>();
            }
        }
        
        // Horizontal wall at z=5, from x=2 to x=4
        for (int x = 2; x <= 4; x++)
        {
            GameObject interiorWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            interiorWall.name = $"InteriorWall_Horizontal_{x}";
            interiorWall.transform.position = new Vector3(x, wallYPosition, 5);
            interiorWall.transform.localScale = wallScale;
            interiorWall.transform.SetParent(parent);
            interiorWall.GetComponent<Renderer>().material.color = wallColor; // Same color as perimeter walls
            
            // Verify BoxCollider exists
            if (interiorWall.GetComponent<BoxCollider>() == null)
            {
                interiorWall.AddComponent<BoxCollider>();
            }
        }
    }
    
    /// <summary>
    /// Generates treasure placeholder objects (gold-colored cubes)
    /// </summary>
    private void GenerateTreasures(Transform parent)
    {
        // Gold color for treasure
        Color goldColor = new Color(1f, 0.9f, 0.2f);
        
        // Treasure scale
        Vector3 treasureScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        // Treasure positions
        Vector3[] treasurePositions = new Vector3[]
        {
            new Vector3(2f, 0.5f, 2f),  // Treasure 1
            new Vector3(7f, 0.5f, 7f),  // Treasure 2
            new Vector3(8f, 0.5f, 3f)   // Treasure 3
        };
        
        // Create treasure objects
        for (int i = 0; i < treasurePositions.Length; i++)
        {
            GameObject treasure = GameObject.CreatePrimitive(PrimitiveType.Cube);
            treasure.name = $"Treasure_{i + 1}";
            treasure.transform.position = treasurePositions[i];
            treasure.transform.localScale = treasureScale;
            treasure.GetComponent<Renderer>().material.color = goldColor;
            treasure.transform.SetParent(parent);
            
            // Verify BoxCollider exists
            if (treasure.GetComponent<BoxCollider>() == null)
            {
                treasure.AddComponent<BoxCollider>();
            }
        }
    }
    
    /// <summary>
    /// Generates a solid 10x10 roof made of individual cube GameObjects
    /// </summary>
    private void GenerateRoof(Transform parent)
    {
        // Create 10x10 solid roof grid
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                GameObject roofTile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                roofTile.name = $"RoofTile_{x}_{z}";
                roofTile.transform.position = new Vector3(x, 3, z); // Height 3 matches wall height
                roofTile.transform.localScale = new Vector3(1, 0.1f, 1); // Thin like floor
                roofTile.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 0.4f); // Same color as walls
                roofTile.transform.SetParent(parent);
                
                // Verify BoxCollider exists (CreatePrimitive adds it automatically)
                if (roofTile.GetComponent<BoxCollider>() == null)
                {
                    roofTile.AddComponent<BoxCollider>();
                }
            }
        }
    }
    
}
