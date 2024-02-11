using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldTile : MonoBehaviour
{
    public int posX;
    public int posY;
    public float elevation;
    public TileType type;
    public TerrainType terrain;
    public SpriteRenderer baseSprite;

    public List<WorldTile> adjacent = new List<WorldTile>();

    public GameObject cityObject;

    public bool occupied;
    public bool isHidden;

    public int gCost;
    public int hCost;
    public int penalty;
    public WorldTile pathParent;
    public bool hasRoad;

    [SerializeField] GameObject highlight;
    public WorldUnit associatedUnit;

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

    public void SpawnCity(string newName, GameObject cityPrefab)
    {
        cityObject = Instantiate(cityPrefab, this.transform);
        cityObject.GetComponent<WorldCity>().SetUp(newName, CityType.VILLAGE, this);

        baseSprite.color = FeudGameManager.Instance.colors.unclaimedCityColor;

        List<WorldTile> foundCityTiles = MapGenerator.Instance.GetTileListWithinRadius(this, 1);
        foreach (WorldTile tile in foundCityTiles)
        {
            tile.cityObject = this.cityObject;
            tile.baseSprite.color = FeudGameManager.Instance.colors.unclaimedCityColor;
        }

        cityObject.GetComponent<WorldCity>().cityTiles = foundCityTiles;
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


        switch (type)
        {
            case TileType.LAND:
                return true;
            case TileType.WATER:
                return false;
        }

        return false;
    }


}

public enum TileType
{
    WATER,
    LAND
}
