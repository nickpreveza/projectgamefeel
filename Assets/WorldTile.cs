using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    GameObject unitInside;

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
        if (hasCity)
        {
            Debug.Log("Found City: " + City().cityName);
            FeudGameManager.Instance.ViewCity(true, this);
            cityObject.GetComponent<WorldCity>().wiggler?.Wiggle();
        }
    }

    public void Deselect()
    {

    }

    public void UnitIn()
    {

    }

    public void UnitOut()
    {

    }

    public void SpawnCity(string newName, GameObject cityPrefab)
    {
        cityObject = Instantiate(cityPrefab, this.transform);
        cityObject.GetComponent<WorldCity>().SetUp(newName, CityType.VILLAGE, this);
    }

    public WorldCity City()
    {
        return cityObject.GetComponent<WorldCity>();
    }

}

public enum TileType
{
    WATER,
    LAND
}
