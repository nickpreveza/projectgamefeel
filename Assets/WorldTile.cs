using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldTile : MonoBehaviour, IHeapItem<WorldTile>
{
    public int posX;
    public int posY;
    public float elevation;
    public TileType type;
    public TerrainType terrain;
    public SpriteRenderer baseSprite;

    public List<WorldTile> adjacent = new List<WorldTile>();
    public List<WorldTile> adjacentSides = new List<WorldTile>();

    public GameObject cityObject;

    public bool occupied;
    public bool isHidden;
    public bool hasRoad;
    public bool isCityOrigin;

    int heapIndex;
    public int gCost;
    public int hCost;
    public int movePenalty;
    public WorldTile pathParent;
 

    [SerializeField] GameObject highlight;
    public WorldUnit associatedUnit;

    public List<WorldTile> connectedCities = new List<WorldTile>();

    public Vector2 PositionVector
    {
        get
        {
            return new Vector2(posX, posY);
        }
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    public bool hasCity
    {
        get
        {
            return (cityObject != null);
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(WorldTile tileToCompare)
    {
        int compare = fCost.CompareTo(tileToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(tileToCompare.hCost);
        }

        return -compare;
    }

    public void SetDataArena(int x, int y, float cellSize)
    {
        this.name = "ArenaTile:" + x + "," + y;
        posX = x;
        posY = y;
        //baseSprite.color = newColor; 
    }

    public void SetData(int x, int y, Color newColor, float _elevation, float cellSize, TileType _type, TerrainType _terrain)
    {
        terrain = _terrain;
        elevation = _elevation;
        type = _type;
        this.name = "Tile:" + x + "," + y;
        posX = x;
        posY = y;
        // this.transform.eulerAngles = new Vector3(90, 0, 0);
    
        this.transform.position = new Vector3(x, y, 0);
        baseSprite.color = newColor;
    }

    public void Select(bool isRepeatSelection)
    {
        if (hasCity)
        {
            if (City().parentTile.occupied)
            {
                FeudGameManager.Instance.ViewCity(true, this);
                cityObject.GetComponent<WorldCity>().wiggler?.Wiggle();
            }
            else if (FeudGameManager.Instance.playerInWorld.GetComponent<WorldUnit>().citiesInRange.Contains(this.City().parentTile))
            {
                UnitManager.Instance.SelectUnit(FeudGameManager.Instance.playerInWorld.GetComponent<WorldUnit>());
                UnitManager.Instance.MoveToTargetTile(this.City().parentTile, true, FeudGameManager.Instance.randomEncounters);
            }
        }
        if (hasCity)
        {
            Debug.Log("Found City: " + City().cityName);
            //FeudGameManager.Instance.ViewCity(true, this);
            cityObject.GetComponent<WorldCity>().wiggler?.Wiggle();
        }


        /*
        if (isHidden)
        {
            UnitManager.Instance.ClearTileSelectMode();
            SI_CameraController.Instance.DeselectSelection();

            //particle
            //sound maybe 
            //wiggle somwhere
        }


        if (UnitManager.Instance.tileSelectMode)
        {
            if (UnitManager.Instance.startTile == this)
            {
                UnitManager.Instance.ClearTileSelectMode();
            }
            else if (UnitManager.Instance.IsTileValidMove(this))
            {
                UnitManager.Instance.MoveToTargetTile(this);
                return;
            }
            else
            {
                UnitManager.Instance.ClearTileSelectMode();
            }
        }

        if (!isRepeatSelection)
        {
            if (occupied && associatedUnit != null)
            {
                associatedUnit.Select();
                UnitManager.Instance.SelectUnit(associatedUnit);
                //audio whatever
                return;
            }
            else
            {
                SI_CameraController.Instance.repeatSelection = true;
                UnitManager.Instance.ClearTileSelectMode();
                //ShowHighlight(false);

                if (hasCity)
                {
                    Debug.Log("Found City: " + City().cityName);
                    FeudGameManager.Instance.ViewCity(true, this);
                    cityObject.GetComponent<WorldCity>().wiggler?.Wiggle();
                }
            }
        }
        else
        {
            SI_CameraController.Instance.repeatSelection = true;
            UnitManager.Instance.ClearTileSelectMode();
            ShowHighlight(false);
            //particle;
            //sound;

            //UIManager.Instance.ShowHexView(this);
        }
            */
    }


    public void Deselect()
    {
        HideHighlight();
    }

    public void UnitIn(WorldUnit newUnit)
    {
        occupied = true;
        associatedUnit = newUnit;
    }

    public void UnitOut()
    {
        occupied = false;
        associatedUnit = null;
    }

    public void ShowHighlight(bool combat)
    {
        highlight.SetActive(true);
    }

    public void HideHighlight()
    {
        highlight.SetActive(false);
    }

    public void CreateRoad()
    {
        hasRoad = true;
        movePenalty = 0;
        baseSprite.color = FeudGameManager.Instance.colors.roadColor;
    }

    public void SpawnCity(string newName, GameObject cityPrefab)
    {
        cityObject = Instantiate(cityPrefab, this.transform);
        cityObject.GetComponent<WorldCity>().SetUp(newName, CityType.VILLAGE, this);
        isCityOrigin = true;
        //baseSprite.color = FeudGameManager.Instance.colors.unclaimedCityColor;

        List<WorldTile> foundCityTiles = MapGenerator.Instance.GetTileListWithinRadius(this, 1, true);
        foreach (WorldTile tile in foundCityTiles)
        {
            tile.cityObject = this.cityObject;
            tile.baseSprite.color = FeudGameManager.Instance.colors.unclaimedCityColor;
        }

        cityObject.GetComponent<WorldCity>().cityTiles = foundCityTiles;
        cityObject.GetComponent<WorldCity>().AssignToCivilization(cityObject.GetComponent<WorldCity>().civIndex);
        hasRoad = true;
    }

    public WorldCity City()
    {
        return cityObject.GetComponent<WorldCity>();
    }

    public bool CanBeWalked(bool isEndTile = false)
    {
        if (isHidden || occupied)
        {
            return false;
        }

        if (isEndTile && occupied)
        {
            return false;
        }
        /*

        switch (type)
        {
            case TileType.LAND:
                return true;
            case TileType.WATER:
                return false;
        }*/

        return true;
    }


}

public enum TileType
{
    WATER,
    LAND
}
