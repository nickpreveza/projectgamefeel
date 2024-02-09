using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WorldUnit : MonoBehaviour
{
    public int posX;
    public int posY;

    Wiggler wiggler;

    //public WorldCity parentTile;

    public Vector2 oldPosition;
    public Vector2 newPosition;

    Vector3 currentVelocity;
    Vector3 currentRotationVelocity;
    float smoothTime = 0.5f;
    bool shouldMove;

    Queue<WorldTile> tilePath;

    public bool hasMoved;
    public bool hasAttacked;

    public bool isInteractable;
    public bool interactableButIgnored;

    public int movePoints;
    public int currentMovePoints;
}
