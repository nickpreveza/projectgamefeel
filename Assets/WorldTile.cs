using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTile : MonoBehaviour
{
    public int posX;
    public int postY;
    public float debugHeight;

    public SpriteRenderer baseSprite;

    public void SetData(int x, int y, Color newColor, float elv)
    {
        debugHeight = elv;

        this.name = "Tile:" + x + "," + y;
        posX = x;
        postY = y;
        this.transform.position = new Vector2(x, y);
        baseSprite.color = newColor;
    }
}
