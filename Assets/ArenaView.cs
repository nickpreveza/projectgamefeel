using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaView : MonoBehaviour
{
    public static ArenaView Instance;

    public int mapWidth;
    public int mapHeight;

    public float obstacleChance;
    public float cellSize = 1;
    public Vector3 offset;

    public int seed;
    public GameObject tilePrefab;
   
    public List<Vector2> spawnPositions = new List<Vector2>();
    public List<Vector2> enemyPosition = new List<Vector2>();

    public Transform tileParent;
    public WorldTile[,] arenaTiles;

    public int MaxSize { get { return mapWidth * mapHeight; }}
    
    void Awake(){
        Instance = this;
    }


}
