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
    public SpriteRenderer unitSprite;
    [SerializeField] Animator animator;

    public Item item;
    public ArenaView arenaHandler;

    public float attackSpeed;
    public float moveSpeed;
    public int damage;
    public int health;
    public int armor;
    public bool hasWeapon;
    public bool hasShield;

    public bool hasTarget;
    public bool hasBeenTargeted;

    UnitState state = UnitState.IDLE;
    bool stateChanged = true;

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

        //pdateVisuals();
    }


    public void ArenaSpawn(ArenaView _handler, WorldTile startParent, Item _item)
    {
        if (_item.invalidated)
        {
            Debug.LogError("Spawned dead unit");
            return;
        }

        item = _item;
        arenaHandler = _handler;

       
        parentTile = startParent;

       

        //setup stuff

        posX = parentTile.posX;
        posY = parentTile.posY;

        parentTile.UnitIn(this);

        attackCharges = maxAttackCharges;
        movePoints = maxMovePoints;

        transform.position = new Vector3(posX, posY, 0);
        oldPosition = newPosition = this.transform.position;
        if (item.weapon != null)
        {
            weaponSprite.sprite = item.weapon.icon;
            hasWeapon = true;
        }
        else
        {
            weaponSprite.gameObject.SetActive(false);
            hasWeapon = false;
        }
        if (item.shield != null)
        {
            shieldSprite.sprite = item.shield.icon;
            hasShield = true;
        }
        else
        {
            shieldSprite.gameObject.SetActive(false);
            hasShield = false;
        }

        unitSprite.sprite = item.icon;

        moveSpeed = item.moveSpeed;
        attackSpeed = item.attackSpeed;

        damage = item.strRequirment;
        //dex speed 
        health = 5;
        armor = item.conRequirment;

        if (hasShield)
        {
            armor += item.shield.damageOrDefense;
        }

        if (hasWeapon)
        {
            damage += item.weapon.damageOrDefense;
        }

        StartCoroutine(ArenaBrain());

    }

    public IEnumerator ArenaBrain()
    {
        state = UnitState.IDLE;

        switch (state)
        {
            case UnitState.IDLE:
                //if no target in range, idle, and search
                if (stateChanged)
                {
                    animator.SetBool("ATTACKING", false);
                    animator.SetBool("IDLE", true);
                    stateChanged = false;
                   
                }

                if (hasTarget)
                {
                    state = UnitState.WALKINGTOWARDS;
                    stateChanged = true;
                    StartCoroutine(ArenaBrain());
                }
                break;
            case UnitState.ATTACKING:

                if (stateChanged)
                {
                    animator.SetBool("IDLE", false);
                    animator.SetBool("ATTACKING", true);
                    stateChanged = false;

                }


                if (!hasTarget)
                {
                    state = UnitState.IDLE;
                    stateChanged = true;
                    StartCoroutine(ArenaBrain());
                }
                //if target is dead and stop and switch to searching
                break;
            case UnitState.WALKINGTOWARDS:
                //if target is next tile then stop and switch to attacking
                break;
        }
        yield return new WaitForEndOfFrame();
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
    SEARCHING,
    WALKINGTOWARDS,
    ATTACKING
}
