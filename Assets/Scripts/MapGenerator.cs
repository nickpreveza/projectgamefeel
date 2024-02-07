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

    [SerializeField] Renderer textureRender;

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
                        break;
                    }                
                }

              

                GameObject newTile = Instantiate(tilePrefab, tileParent);

                worldTileGameObject[x, y] = newTile;
                worldTiles[x, y] = newTile.GetComponent<WorldTile>();
               
                worldTiles[x, y].SetData(x, y, colorMap[x, y], currentHeight);


            }
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string label;
    public float height;
    public Color colour;

}
