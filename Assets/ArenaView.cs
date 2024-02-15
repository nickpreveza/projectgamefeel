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
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject unitPrefab;

    public List<Vector2> spawnPositions = new List<Vector2>();
    public List<Vector2> enemyPosition = new List<Vector2>();

    public Transform tileParent;
    public Transform unitParent;
    public WorldTile[,] arenaTiles;

    public List<WorldTile> playerSpawnPositions = new List<WorldTile>();
    public List<WorldTile> oponentSpawnPositions = new List<WorldTile>();
    public int MaxSize { get { return mapWidth * mapHeight; }}
    
    public List<Item> playerUnits = new List<Item>();
    public List<Item> enemyUnits = new List<Item>();

    public List<GameObject> activePlayerUnits = new List<GameObject>();
    public List<GameObject> activeEnemyUnits = new List<GameObject>();

    public List<Vector2Int> playerSpawnCoords = new List<Vector2Int>();
    public List<Vector2Int> enemySpawnCoords = new List<Vector2Int>();

    void Awake()
    {
        Instance = this;
    }

    public void GenerateArena(List<Item> _playerUnits, List<Item> _enemyUnits)
    {
        ClearArena();

        playerUnits = new List<Item>(_playerUnits);
        enemyUnits = new List<Item>(_enemyUnits);

        arenaTiles = new WorldTile[mapWidth, mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                GameObject newTile = Instantiate(tilePrefab, tileParent);
                arenaTiles[x, y] = newTile.GetComponent<WorldTile>();

                arenaTiles[x, y].SetDataArena(x, y, cellSize);
                arenaTiles[x, y].gameObject.transform.position = new Vector3(x, y) * cellSize + offset;
            }
        }


        FindAdjacentTiles();

        SpawnPlayerUnits();
        SpawnEnemyUnits();
    }

    void SpawnPlayerUnits()
    {
        activePlayerUnits = new List<GameObject>();

        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (!playerUnits[i].invalidated) 
            SpawnUnit(i, true, playerUnits[i]);
        }
    }


    void SpawnEnemyUnits()
    {
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (!enemyUnits[i].invalidated)
                SpawnUnit(i, false, enemyUnits[i]);
        }
    }

    public void SpawnUnit(int index, bool isPlayer, Item _item)
    {
        Vector2Int spawnCoords = new Vector2Int();

        if (isPlayer)
        {
            spawnCoords = playerSpawnCoords[index];
        }
        else
        {
            spawnCoords = enemySpawnCoords[index];

        }
        GameObject obj = Instantiate(unitPrefab, arenaTiles[spawnCoords.x, spawnCoords.y].transform.position, Quaternion.identity);
        WorldUnit unit = obj.GetComponent<WorldUnit>();
        obj.transform.SetParent(unitParent);

        if (isPlayer)
        {
            activePlayerUnits.Add(obj);
            unit.item = playerUnits[index];
        }
        else
        {
            activeEnemyUnits.Add(obj);
            unit.item = enemyUnits[index];
        }

        unit.ArenaSpawn(this, arenaTiles[spawnCoords.x, spawnCoords.y], _item);
    }

    public void OnUnitKilled(WorldUnit unit)
    {
        if (unit == null )
        {
            Debug.LogWarning("Tried to register unit after deletion");
            return;
        }

        if (activePlayerUnits.Contains(unit.gameObject))
        {
            if (playerUnits.Contains(unit.item))
            {

            }
            unit.gameObject.SetActive(false);
        }
        else if (activeEnemyUnits.Contains(unit.gameObject))
        {
            unit.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Registered unit was not found in eitehlist");
        }
    }


    public void ClearArena()
    {
        while (tileParent.childCount > 0)
        {
            Destroy(tileParent.GetChild(0).gameObject);
        }

        while (unitParent.childCount > 0)
        {
            Destroy(unitParent.GetChild(0).gameObject);
        }
    }

    public WorldTile GetTile(Vector3 worldPosition)
    {
        WorldTile tileToReturn = null;

        int x, y;
        GetXY(worldPosition, out x, out y);
        if (x >= 0 && y >= 0 && x < mapWidth && y < mapHeight)
        {
            tileToReturn = arenaTiles[x, y];
        }

        return tileToReturn;
    }

    void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x + 0.5f / cellSize);
        y = Mathf.FloorToInt(worldPosition.y + 0.5f / cellSize);
    }



  

    public void FindAdjacentTiles()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                WorldTile tile = arenaTiles[x, y];
                List<WorldTile> tilesToAdd = GetTileListWithinRadius(tile, 1, true);
                if (tilesToAdd.Contains(tile)) //leftover safety check, shouldn't be needed
                {
                    tilesToAdd.Remove(tile);
                }

                tile.adjacent = new List<WorldTile>(tilesToAdd);


                tilesToAdd = GetTileListWithinRadius(tile, 1, false);
                if (tilesToAdd.Contains(tile)) //leftover safety check, shouldn't be needed
                {
                    tilesToAdd.Remove(tile);
                }

                tile.adjacentSides = new List<WorldTile>(tilesToAdd);
                //you might want to filter out the unwanted tiles here for city placement 
            }
        }
    }

    public List<WorldTile> GetTileListWithinRadius(WorldTile centerTile, int range, bool getDiagonalTiles)
    {
        List<WorldTile> adjacentTiles = new List<WorldTile>();

        if (getDiagonalTiles)
        {
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = -range; dy <= range; dy++)
                {
                    if (dx + centerTile.posX >= 0 && dx + centerTile.posX < mapWidth && dy + centerTile.posY >= 0 && dy + centerTile.posY < mapHeight)
                    {
                        adjacentTiles.Add(arenaTiles[dx + centerTile.posX, dy + centerTile.posY]);
                    }
                }
            }
        }
        else
        {
            int[] dx = { 0, 1, 0, -1 }; // Change in x-coordinate
            int[] dy = { -1, 0, 1, 0 }; // Change in y-coordinate

            for (int i = 0; i < dx.Length; i++)
            {
                int newX = centerTile.posX + dx[i];
                int newY = centerTile.posY + dy[i];

                // Check if the new coordinates are within the bounds of the grid
                if (newX >= 0 && newX < mapWidth && newY >= 0 && newY < mapHeight)
                {
                    // If within bounds, add the value of the adjacent tile to the list
                    adjacentTiles.Add(arenaTiles[newX, newY]);
                }
            }
        }

        return adjacentTiles;
    }

    public Direction GetTileDirection(WorldTile origin, WorldTile target)
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

    public List<WorldTile> FindPath(WorldTile start, WorldTile end, bool onlyCheckForRoadPath)
    {
        Heap<WorldTile> openSet = new Heap<WorldTile>(MaxSize);
        List<WorldTile> closedSet = new List<WorldTile>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            WorldTile current = openSet.RemoveFirst();
            closedSet.Add(current);

            if (current == end)
            {
                List<WorldTile> path = new List<WorldTile>();
                WorldTile retractCurrent = end;

                while (retractCurrent != start)
                {
                    path.Add(retractCurrent);
                    retractCurrent = retractCurrent.pathParent;
                }

                path.Reverse();
                return path;
            }

            bool shouldSkipAdj = false;

            if (current.type == TileType.WATER)
            {
                shouldSkipAdj = true;
            }


            if (onlyCheckForRoadPath)
            {
                if (!current.hasRoad)
                {
                    shouldSkipAdj = true;
                }
            }

            if (!shouldSkipAdj)
            {
                foreach (WorldTile adj in current.adjacentSides)
                {
                    if (closedSet.Contains(adj) || adj.type != TileType.LAND)
                    {
                        continue;
                    }


                    if (onlyCheckForRoadPath)
                    {
                        if (!adj.hasRoad)
                        {
                            continue;
                        }
                    }

                    int movementCostToAdj = current.gCost + GetDistance(current, adj) + adj.movePenalty;

                    if (movementCostToAdj < adj.gCost || !openSet.Contains(adj))
                    {
                        adj.gCost = movementCostToAdj;
                        adj.hCost = GetDistance(adj, end);
                        adj.pathParent = current;

                        if (!openSet.Contains(adj))
                        {
                            openSet.Add(adj);
                        }
                        else
                        {
                            openSet.UpdateItem(adj);
                        }
                    }
                }
            }
        }

        return null;
    }

    public int GetDistance(WorldTile start, WorldTile end)
    {
        WorldTile a = start;
        WorldTile b = end;

        int dY = Mathf.Abs(a.posY - b.posY);
        int dX = Mathf.Abs(a.posX - b.posX);

        if (dX > mapWidth / 2)
        {
            dX = mapWidth - dX;
        }

        if (dY > mapHeight / 2)
        {
            dY = mapHeight - dY;
        }

        //int dZ = (-dC - dR);

        return Mathf.Max(dX, dY);//, Mathf.Abs(a.S - b.S));
        //return Mathf.Max(Mathf.Max(Mathf.Abs(dR), Mathf.Abs(dC)), Mathf.Abs(dZ));
    }

}
