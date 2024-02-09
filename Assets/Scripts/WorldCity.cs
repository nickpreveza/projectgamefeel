using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCity : MonoBehaviour
{
    public WorldTile parentTile;
    public string cityName;
    public CivilizationType civilizationType;
    public CityType cityType;

    public bool hasBarracks;
    public bool hasForge;
    public bool hasTavern;
    public bool hasPort;

    public Wiggler wiggler;

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
                hasBarracks = true;
                hasForge = true;
                hasTavern = true;
                break;
            case CityType.FORT:
                hasBarracks = true;
                hasForge = true;
                hasTavern = true;
                break;
            case CityType.CASTLE:
                hasBarracks = true;
                hasForge = true;
                hasTavern = true;
                break;
        }
    }
}

public enum CityType
{
    VILLAGE,
    FORT,
    CASTLE
}
