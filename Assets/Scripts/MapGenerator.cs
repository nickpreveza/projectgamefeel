using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[System.Serializable]
public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public float cellSize = 1;
    public int citiesToGenerate;

    List<string> availableCityNames = new List<string>();

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
    public GameObject cityPrefab;

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

    public List<WorldTile> tilesAvailableForCities = new List<WorldTile>();
    public WorldTile[] walkableTiles;

    public List<WorldTile> worldCities = new List<WorldTile>();
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

    private void Awake()
    {
        Instance = this;
        availableCityNames = new List<string>(cityNames);

    }
    public void ClearMap(bool isEditor)
    {
        while (tileParent.childCount > 0)
        {
            DestroyImmediate(tileParent.GetChild(0).gameObject);
        }

    }

    public void DebugDrawNoiseMap()
    {
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, cellSize, seed, noiseScale, octaves, persistnace, lacunarity, offset);
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

    public WorldTile GetTile(Vector3 worldPosition)
    {
        WorldTile tileToReturn = null;

        int x, y;
        GetXY(worldPosition, out x, out y);
        if (x >= 0 && y >= 0 && x < mapWidth &&  y < mapHeight)
        {
            tileToReturn = worldTiles[x, y];
        }

        return tileToReturn;
    }

    void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / 1) + 1;
        y = Mathf.FloorToInt(worldPosition.y / 1) + 1;
    }
    public void GenerateMap()
    {
        ClearMap(false);

        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, cellSize, seed, noiseScale, octaves, persistnace, lacunarity, offset);
        colorMap = new Color[mapWidth, mapHeight];
        regionMap = new TerrainType[mapWidth, mapHeight];
        worldTileGameObject = new GameObject[mapWidth, mapHeight];
        worldTiles = new WorldTile[mapWidth, mapHeight];
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth);
        tilesAvailableForCities = new List<WorldTile>();

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
               
                worldTiles[x, y].SetData(x, y, colorMap[x, y], currentHeight, cellSize, regionMap[x, y].type, regionMap[x, y]);
                worldTiles[x, y].type = regionMap[x, y].type;

                if (worldTiles[x,y].type == TileType.LAND)
                {
                    tilesAvailableForCities.Add(worldTiles[x, y]);
                }

                //Debug.DrawLine(GetWorldPosition)
            }
        }

        FindAdjacentTiles(); //second pass to find adjacent tiles 
        GenerateCities();
        SpawnPlayer();
    }

    //this goes to game manager
    void SpawnPlayer()
    {
        int randomCityFound = Random.Range(0, worldCities.Count);
        FeudGameManager.Instance.CreatePlayer(worldCities[randomCityFound]);

    }

    public void GenerateCities()
    {
        int citiesSpawned = 0;
        Random.InitState(seed);
        List<WorldTile> tilesInRadius = new List<WorldTile>();

        List<WorldTile> tilesToFilterOut = new List<WorldTile>();

        for(int i = 0; i < tilesAvailableForCities.Count; i++)
        {
            List<WorldTile> markedForRemoval = new List<WorldTile>();
            int waterAdjCount = 0;

            foreach (WorldTile tileToCheck in tilesAvailableForCities[i].adjacent)
            {
                
                if (tileToCheck.type == TileType.WATER)
                {
                    waterAdjCount++;

                    if (waterAdjCount > 3)
                    {
                        markedForRemoval.Add(tilesAvailableForCities[i]);
                        continue;
                    }
                    Direction hexDirection = GetHexDirection(tilesAvailableForCities[i], tileToCheck);
                    if (hexDirection == Direction.Right || hexDirection == Direction.Up || hexDirection == Direction.RightUp)
                    {
                        markedForRemoval.Add(tilesAvailableForCities[i]);
                        continue;
                    }
                }
              
            }
        }

        foreach(WorldTile tile in tilesToFilterOut)
        {
            if (tilesAvailableForCities.Contains(tile))
            {
                tilesAvailableForCities.Remove(tile);
            }
        }

        for(int i = 0; i < citiesToGenerate; i++)
        {
            if (tilesAvailableForCities.Count <= 0)
            {
                Debug.LogWarning("No more available spaces where found for citis");
                break;
            }

            int randomTileIndex = Random.Range(0, tilesAvailableForCities.Count);
            WorldTile newCityTile = tilesAvailableForCities[randomTileIndex];
            string cityName = "defaultCity";

            if (availableCityNames.Count > 0)
            {
                int randomIndex = Random.Range(0, availableCityNames.Count);
                cityName = availableCityNames[randomIndex];
                availableCityNames.RemoveAt(randomIndex);
            }
            else
            {
                Debug.LogWarning("No more available names for Cities found");
            }

            newCityTile.SpawnCity(cityName, cityPrefab);
            worldCities.Add(newCityTile);
            citiesSpawned++;

            newCityTile.baseSprite.color = Color.white;
            foreach(WorldTile tile in GetTileListWithinRadius(newCityTile, 1))
            {
                tile.cityObject = newCityTile.cityObject;
                tile.baseSprite.color = Color.white;
            }

            tilesInRadius = GetTileListWithinRadius(newCityTile, 5);

            foreach (WorldTile tile in tilesInRadius)
            {
                if (tilesAvailableForCities.Contains(tile))
                {
                    tilesAvailableForCities.Remove(tile);
                }
            }
        }

        List<WorldTile> worldCitiesToAssign = new List<WorldTile>(worldCities);

        //foreach civ, give them approximately the same number of cities 
        //then for each of those civs, arrange the cities to villages, forts and castles 

    }

    public void FindAdjacentTiles()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                WorldTile tile = worldTiles[x, y];
                List<WorldTile> tilesToAdd = GetTileListWithinRadius(tile, 1);
                if (tilesToAdd.Contains(tile)) //leftover safety check, shouldn't be needed
                {
                    tilesToAdd.Remove(tile);
                }

                tile.adjacent = new List<WorldTile>(tilesToAdd);

                //you might want to filter out the unwanted tiles here for city placement 
            }
        }
    }

    public List<WorldTile> GetTileListWithinRadius(WorldTile centerTile, int range)
    {
        List<WorldTile> adjacentTiles = new List<WorldTile>();

       // int[] dx = { 0, 1, 0, -1 }; // Change in x-coordinate
        //int[] dy = { -1, 0, 1, 0 }; // Change in y-coordinate

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                if (dx + centerTile.posX >= 0 && dx + centerTile.posX < mapWidth && dy + centerTile.posY >= 0 && dy + centerTile.posY < mapHeight)
                {
                    adjacentTiles.Add(worldTiles[dx + centerTile.posX, dy + centerTile.posY]);
                }
            }
        }
        /*
        // Loop through each possible adjacent tile
        for (int i = 0 ; i < dx.Length; i++)
        {
            int newX = centerTile.posX + dx[i];
            int newY = centerTile.posY + dy[i];

            // Check if the new coordinates are within the bounds of the grid
            if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapWidth)
            {
                // If within bounds, add the value of the adjacent tile to the list
                adjacentTiles.Add(worldTiles[newX, newY]);
            }
        }*/

        return adjacentTiles;
    }

    public Direction GetHexDirection(WorldTile origin, WorldTile target)
    {
        if (origin.posX == target.posX)
        {
            if (target.posY > origin.posY)
            {
                return Direction.Up;
            }
            else if (target.posY < origin.posY)
            {
                return Direction.Down;
            }
        }
        else if (origin.posY == target.posY)
        {
            if (target.posX > origin.posX)
            {
                return Direction.Right;
            }
            else if (target.posX < origin.posX)
            {
                return Direction.Left;
            }
        }
        else if (target.posX > origin.posX && target.posY > origin.posY)
        {
            return Direction.RightUp;
        }
        else if (target.posX < origin.posX && target.posY > origin.posY)
        {
            return Direction.LeftUp;
        }
        else if (target.posX > origin.posX && target.posY < origin.posY)
        {
            return Direction.RightDown;
        }
        else if (target.posX < origin.posX && target.posY < origin.posY)
        {
            return Direction.LeftDown;
        }
     

        return Direction.Right;
    }

    public void UpdateColors()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (worldTiles[x, y].cityObject == null)
                worldTiles[x, y].baseSprite.color = regionMap[x, y].colour;
            }
        }
    }

    public string[] cityNames = new string[] {
    "Bamery",
    "Ochepsa",
    "Edosgend",
    "Pihsea",
    "Vleuver",
    "Osrery",
    "Yhok",
    "Hurg",
    "Acomond",
    "Ocksas",
    "Crietsa",
    "Yreford",
    "Krehledo",
    "Vruelwell",
    "Keburn",
    "Oprey",
    "Grose",
    "Sheley",
    "Odonsea",
    "Ingate",
    "Crevale",
    "Zremont",
    "Floshire",
    "Stigow",
    "Lapus",
    "Clurgh",
    "Cront",
    "Outinsburgh",
    "Adenagow",
    "Slespolis",
    "Hundale",
    "Obrasall",
    "Saxsa",
    "Phanard",
    "Srico",
    "Drico",
    "Preah",
    "Estertin",
    "Illesall",
    "Halas",
    "Sraset",
    "Frahsey",
    "Juistin",
    "Priekfast",
    "Oclando",
    "Dria",
    "Sria",
    "Ingow",
    "Asoby",
    "Netgas",
    "Phaham",
    "Baihta",
    "Plabury",  
    "Yuaburn",
    "Rada",
    "Flago",
    "Shoni",
    "Oseton",
    "Adenabus",
    "Gunburg",
    "Yrecphis",
    "Ablaaytin",
    "Depfield",
    "Chamond",
    "Qrila",
    "Clanbu",
    "Zhord",
    "Inasshire",
    "Iriecester",   
    "Preveza" };
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
    Up,
    Down,
    Right,
    Left,

    RightUp,
    RightDown,
    LeftUp,
    LeftDown
}

public enum CivilizationType
{
    RED,
    PURPLE,
    YELLOW,
    BLACK
}

