using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistnace;
    public float lacunarity;

    public int arenaWidth;
    public int arenaHeight;
    public int obstacleChance;

    public int seed;
    public Vector2 offset;

    public GameObject tilePrefab;

    public bool useFalloff;

    public TerrainType[] regions;

    public Transform tileParent;

    public GameObject[,] worldTileGameObject;
    public WorldTile[,] worldTiles;
    public Color[,] colorMap;
    public float[,] noiseMap;
    public float[,] falloffMap;
    public TerrainType[,] regionMap;

    [SerializeField] Renderer textureRender;

    public WorldTile[] tilesAvailableForCities;
    public WorldTile[] walkableTiles;

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }

        if (mapHeight < 1)
        {
            mapHeight = 1;
        }

        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    public void ClearMap()
    {
        foreach(Transform obj in tileParent)
        {
            DestroyImmediate(obj.gameObject);
        }
    }

    public void DebugDrawNoiseMap()
    {
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistnace, lacunarity, offset);
        Texture2D texture = new Texture2D(noiseMap.GetLength(0), noiseMap.GetLength(1));

        Color[] colorMap = new Color[mapHeight * mapWidth];
      
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                colorMap[y * mapWidth + x] = Color.Lerp(Color.black, Color.white, noiseMap[x,y]);

            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localPosition = new Vector3(mapWidth, 1, mapHeight);

    }
    public void GenerateMap()
    {
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistnace, lacunarity, offset);
        colorMap = new Color[mapWidth, mapHeight];
        regionMap = new TerrainType[mapWidth, mapHeight];
        worldTileGameObject = new GameObject[mapWidth, mapHeight];
        worldTiles = new WorldTile[mapWidth, mapHeight];
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }

                float currentHeight = noiseMap[x, y];
              
                for (int i = 0; i <regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        //found region
                        //y * mapWidth + x for linear 
                        colorMap[x, y] = regions[i].colour;
                        regionMap[x,y] = regions[i];
                        break;
                    }                
                }

                GameObject newTile = Instantiate(tilePrefab, tileParent);

                worldTileGameObject[x, y] = newTile;
                worldTiles[x, y] = newTile.GetComponent<WorldTile>();
               
                worldTiles[x, y].SetData(x, y, colorMap[x, y], currentHeight, regionMap[x, y].type);
            }
        }

        FindAdjacentTiles(); //second pass to find adjacent tiles 
        GenerateCities();
    }

    public void GenerateCities()
    {
        int citiesSpawned = 0;
        Random.InitState(seed);
        List<WorldTile> hexesInRadius = new List<WorldTile>();
    }
    public void FindAdjacentTiles()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                WorldTile tile = worldTiles[x, y];
                List<WorldTile> tilesToAdd = GetHexesListWithinRadius(tile, 1);
                if (tilesToAdd.Contains(tile)) //leftover safety check, shouldn't be needed
                {
                    tilesToAdd.Remove(tile);
                }

                tile.adjacent = tilesToAdd;
                tile.walkableAdjacent = tilesToAdd;

                foreach(WorldTile adjTile in tile.walkableAdjacent)
                {
                    if (adjTile.type != TileType.LAND)
                    {
                        tile.walkableAdjacent.Remove(adjTile);
                    }
                }

                //you might want to filter out the unwanted tiles here for city placement 
            }
        }
    }

    public void FindAdjacentWalkable()
    {

    }

    public List<WorldTile> GetHexesListWithinRadius(WorldTile centerTile, int range)
    {
        List<WorldTile> adjacentTiles = new List<WorldTile>();

        int[] dx = { 0, 1, 0, -1 }; // Change in x-coordinate
        int[] dy = { -1, 0, 1, 0 }; // Change in y-coordinate

        // Loop through each possible adjacent tile
        for (int i = 0; i < dx.Length; i++)
        {
            int newX = centerTile.posX + dx[i];
            int newY = centerTile.posY + dy[i];

            // Check if the new coordinates are within the bounds of the grid
            if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapWidth)
            {
                // If within bounds, add the value of the adjacent tile to the list
                adjacentTiles.Add(worldTiles[newX, newY]);
            }
        }

        return adjacentTiles;
    }

    public Direction GetHexDirection(WorldTile tileOrigin, WorldTile tileTarget)
    {
        if (tileOrigin.posX == tileTarget.posX)
        {
            if (tileTarget.posY > tileOrigin.posY)
            {
                return Direction.RightUp;
            }
            else if (tileTarget.posY < tileOrigin.posY)
            {
                return Direction.LeftDown;
            }
        }
        else if (tileOrigin.posY == tileTarget.posY)
        {
            if (tileTarget.posX > tileOrigin.posX)
            {
                return Direction.Right;
            }
            else if (tileTarget.posX < tileOrigin.posX)
            {
                return Direction.Left;
            }
        }
        else if (tileOrigin.posY < tileTarget.posY && tileOrigin.posX > tileTarget.posX)
        {
            return Direction.LeftUp;
        }
        else if (tileOrigin.posX < tileTarget.posX && tileOrigin.posY > tileTarget.posY)
        {
            return Direction.RightDown;
        }

        return Direction.Right;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string label;
    public float height;
    public Color colour;
    public TileType type;
}

public enum Direction
{
    RightUp,
    Right,
    RightDown,
    LeftDown,
    Left,
    LeftUp
}

