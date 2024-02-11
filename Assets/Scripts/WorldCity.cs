using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCity : MonoBehaviour
{
    public WorldTile parentTile;
    public string cityName;
    public CivilizationScriptable civReference;
    public CityType cityType;

    public bool playerOwned = false;

    public bool hasPort;

    public float friendlinessLevel = 0; //0-1
    public float powerLevel = 0 ; //0-1

    public Wiggler wiggler;

    public List<WorldCity> alliedCities = new List<WorldCity> ();
    public List<WorldCity> enemyCities = new List<WorldCity> ();

    public List<Item> equipmentToSell = new List<Item>();
    public List<Item> unitsToSell = new List<Item>();
    public List<Item> questsToSell = new List<Item>(); //not really selling per se but you get it 

    public List<WorldTile> cityTiles = new List<WorldTile> ();

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

        foreach(WorldTile tile in parentTile.adjacent)
        {
            if(tile.type == TileType.WATER)
            {
                hasPort = true;
            }
        }

        switch (cityType)
        {
            case CityType.VILLAGE:
                powerLevel = 0.3f;
                friendlinessLevel = 0.5f;
                break;
            case CityType.FORT:
                powerLevel = 0.6f;
                friendlinessLevel = 0.3f;
                break;
            case CityType.CASTLE:
                powerLevel = 1f;
                friendlinessLevel = 0.1f;
                break;
        }
    }

    void UpdateTileColors(Color color)
    {
        foreach(WorldTile tile in cityTiles)
        {
            tile.baseSprite.color = color;
        }
    }

    public void AssignToCivilization()
    {
        // civReference = _civ;
        // UpdateTileColors(civReference.tileColor);

        UpdateTileColors(FeudGameManager.Instance.colors.playerOwnedColor);

    }
}

public enum CityType
{
    VILLAGE,
    FORT,
    CASTLE
}
