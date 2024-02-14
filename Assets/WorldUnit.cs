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

    public int maxMovePoints;
    public int maxAttackCharges;

    public int walkRange = 2;
    public int roadModifier = 1;

    public int attackCharges;
    public int movePoints;

    public WorldTile parentTile;
    public bool noValidMove;

    //leftover, might use later
    bool canAttackAfterMove = false;
    bool canMoveAfterAttack = false;

    public bool noTileInRange;
    public bool noAttackTileInRange;

    public bool buttonActionPossible;

    public WorldTile assignedPathTarget;

    public List<WorldTile> citiesInRange = new List<WorldTile> ();

    public SpriteRenderer weaponSprite;
    public SpriteRenderer shieldSprite;
    [SerializeField] Animator animator;
    private void Start()
    {
        attackCharges = maxAttackCharges;
        movePoints = maxMovePoints;
    }

    public void MoveCompleted()
    {
        movePoints = maxMovePoints;
        hasMoved = false;
    }
    public void Select()
    {
        parentTile.ShowHighlight(false);
    }

    public void Deselect()
    {
        parentTile.HideHighlight();
    }

    public void ValidateActions()
    {
        if (hasMoved)
        {
            if (!canAttackAfterMove)
            {
                attackCharges = 0;
            }
        }

        if (hasAttacked)
        {
            if (!canMoveAfterAttack)
            {
                movePoints = 0;
                noTileInRange = true;
            }
        }

        bool interactabilityCheckForWalk = false;
        bool interactabilityCheckForAttack = false;

        if (movePoints > 0)
        {
            if (!noTileInRange)
            {
                interactabilityCheckForWalk = true;
            }
        }

        if (attackCharges > 0)
        {
            if (!noAttackTileInRange)
            {
                interactabilityCheckForAttack = true;
            }
        }

        isInteractable = false;

        if (interactabilityCheckForAttack || interactabilityCheckForWalk)
        {
            isInteractable = true;
        }

        if (hasMoved || hasAttacked)
        {
            buttonActionPossible = false;
        }

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        //you can further optimize this ez
        if (isInteractable)
        {
            
        }
        else
        {
           
        }
    }

    public void ArenaSpawn(WorldTile startParent)
    {
        parentTile = startParent;

        oldPosition = newPosition = this.transform.position;

        //setup stuff

        posX = parentTile.posX;
        posY = parentTile.posY;

        parentTile.UnitIn(this);

        attackCharges = maxAttackCharges;
        movePoints = maxMovePoints;

    }

    public void SpawnSetup(WorldTile startParent)
    {
        parentTile = startParent;

        oldPosition = newPosition = this.transform.position;

        //setup stuff

        posX = parentTile.posX;
        posY = parentTile.posY;

        parentTile.UnitIn(this);

        attackCharges = maxAttackCharges;
        movePoints = maxMovePoints;

        citiesInRange = new List<WorldTile>(parentTile.connectedCities);
        //color time here 
        UnitManager.Instance.ClearTileSelectMode();
        UnitManager.Instance.SelectUnit(this);
    }
}

public enum UnitState
{
    IDLE,
    WALKINGTOWARDS,
    ATTACKING
}
