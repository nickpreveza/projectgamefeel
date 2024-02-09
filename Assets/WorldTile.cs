using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    public int posX;
    public int posY;
    public float elevation;
    public TileType type;

    public SpriteRenderer baseSprite;

    public List<WorldTile> adjacent = new List<WorldTile>();
    public List<WorldTile> walkableAdjacent = new List<WorldTile>();

    public void SetData(int x, int y, Color newColor, float _elevation, TileType _type)
    {
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

    }

    public void Deselect()
    {

    }


}

public enum TileType
{
    WATER,
    LAND
}
