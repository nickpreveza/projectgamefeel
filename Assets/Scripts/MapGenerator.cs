using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
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

    public bool autoUpdate;

    public TerrainType[] regions;

    Color[,] colorMap;
    float[,] noiseMap;

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
    public void GenerateMap()
    {
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistnace, lacunarity, offset);
        colorMap = new Color[mapWidth, mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
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
