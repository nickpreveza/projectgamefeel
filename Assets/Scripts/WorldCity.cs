using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldCity : MonoBehaviour
{
    public WorldTile parentTile;
    public string cityName;
    public CityType cityType;

    public bool playerOwned = false;

    public bool hasPort;

    public Wiggler wiggler;

    public List<WorldCity> alliedCities = new List<WorldCity> ();
    public List<WorldCity> enemyCities = new List<WorldCity> ();

    public List<Item> equipmentToSell = new List<Item>();
    public List<Item> unitsToSell = new List<Item>();
    public List<Item> questsToSell = new List<Item>(); //not really selling per se but you get it 

    public List<WorldTile> cityTiles = new List<WorldTile> ();
    public List<WorldTile> adjPerimeter = new List<WorldTile>();

    public TextMeshPro cityNameText;
    public int civIndex;

    public bool hasPlayerSeenLeader;

    private void Start()
    {
        wiggler = GetComponent<Wiggler>();
    }
    public void SetUp(string _cityName, CityType _cityType, WorldTile _parentTile)
    {
        cityType = _cityType;
        cityName = _cityName;
        parentTile = _parentTile;

        hasPort = false;
        adjPerimeter = MapGenerator.Instance.GetAdjTilePerimeterInRadius(parentTile, 2);

        cityNameText.text = "???";

        civIndex = Random.Range(1, FeudGameManager.Instance.gameCivilizations.Length);

        AssignToCivilization(civIndex);

        foreach (WorldTile tile in parentTile.adjacent)
        {
            if(tile.type == TileType.WATER)
            {
                hasPort = true;
            }
        }
    }

    public void SelectCity()
    {
        foreach(WorldTile tile in adjPerimeter)
        {
            tile.ShowHighlight(false);
        }
    }

    public void DeselectCity()
    {
        foreach (WorldTile tile in adjPerimeter)
        {
            tile.HideHighlight();
        }
    }

    void UpdateTileColors(Color color)
    {
        foreach(WorldTile tile in cityTiles)
        {
            tile.baseSprite.color = color;
        }
    }

    public void AssignToCivilization(int _civIndex)
    {
        // civReference = _civ;
        // UpdateTileColors(civReference.tileColor);
        civIndex = _civIndex;
        if (civIndex > 0)
        {
            if (hasPlayerSeenLeader)
            {
                UpdateTileColors(FeudGameManager.Instance.gameCivilizations[civIndex].tileColor);
            }
        }
        else
        {
            UpdateTileColors(FeudGameManager.Instance.gameCivilizations[civIndex].tileColor);
        }
      

    }

    public void LeaderRevealed()
    {
        hasPlayerSeenLeader = true;
        UpdateTileColors(FeudGameManager.Instance.gameCivilizations[civIndex].tileColor);
    }

    public void RevealCity()
    {
        cityNameText.text = cityName;
        //maybe more;
    }
}

public enum CityType
{
    VILLAGE,
    FORT,
    CASTLE
}
