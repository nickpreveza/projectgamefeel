using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
public class WorldUnit : MonoBehaviour
{
    public bool lookUp;
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

    public bool hasTargetRight;
    public bool hasTargetLeft;
    public bool hasBeenTargetedLeft;
    public bool hasBeenTargetedRight;
    public bool playerGroup;
    [SerializeField] UnitState state = UnitState.IDLE;
    bool stateChanged = true;
    public int unitIndex;
    [SerializeField] WorldUnit targetUnit;
    [SerializeField] WorldTile targetTile;
    [SerializeField] List<WorldTile> pathToTarget = new List<WorldTile>();
    [SerializeField] bool searchingTarget;

    [SerializeField] WorldUnit leftAttacker;
    [SerializeField] WorldUnit rightAttacker;

    [SerializeField] int debugOverrideHealth = 20;
    [SerializeField] float internalAttackTimer;

    [SerializeField] float attacklenghtMultipler = 1f;


    float timeElapsedForMove = 0;
    bool moving;
    WorldTile prevTile;
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

    public void SetColors(int index)
    {
        SetUnitSpriteColor(FeudGameManager.Instance.GetCiv(index).mainColor);
    }
    public void SetUnitSpriteColor(Color color)
    {
        unitSprite.material.SetColor("_ColorChangeNewCol", color);
        unitSprite.material.SetColor("_ColorChangeNewCol2", color);
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

        posX = parentTile.posX;
        posY = parentTile.posY;

        parentTile.UnitIn(this);

        attackCharges = maxAttackCharges;
        movePoints = maxMovePoints;

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
        health = debugOverrideHealth;
        armor = item.conRequirment;

        if (hasShield)
        {
            armor += item.shield.damageOrDefense;
        }

        if (hasWeapon)
        {
            damage += item.weapon.damageOrDefense;
        }

        state = UnitState.IDLE;

    }

    public void Attack(WorldUnit targetUnit)
    {
        targetUnit.Damage(damage);
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            Death();
        }
    }

    void Death()
    {
        if (hasBeenTargetedLeft)
        {
            if (leftAttacker != null)
            {
                leftAttacker.hasTargetLeft = false;
            }
        }

        if (hasBeenTargetedRight)
        {
            if (rightAttacker != null)
            {
                rightAttacker.hasTargetRight = false;
            }
        }

        if (targetUnit != null)
        {
            if (hasTargetLeft)
            {
                targetUnit.hasBeenTargetedLeft = false;
            }
            else if(hasTargetRight)
            {
                targetUnit.hasBeenTargetedRight = false;
            }
        }
        arenaHandler.OnUnitKilled(this);
    }

    void FindTarget()
    {
        List<GameObject> enemyGroup = arenaHandler.GetEnemyGroup(playerGroup);

        foreach (GameObject enemy in enemyGroup)
        {
            WorldUnit enemyUnitData = enemy.GetComponent<WorldUnit>();
            if (enemyUnitData != null)
            {
                if (enemyUnitData.hasBeenTargetedLeft && enemyUnitData.hasBeenTargetedRight)
                {
                    continue;
                }

                if (!enemyUnitData.hasBeenTargetedLeft)
                {
                    List<WorldTile> _pathToTarget = arenaHandler.FindPath(this.parentTile, arenaHandler.arenaTiles[enemyUnitData.posX - 1, enemyUnitData.posY], false);

                    if (_pathToTarget != null)
                    {
                        pathToTarget = _pathToTarget;
                        targetTile = arenaHandler.arenaTiles[enemyUnitData.posX - 1, enemyUnitData.posY];
                        hasTargetLeft = true;

                        targetUnit = enemyUnitData;
                        targetUnit.hasBeenTargetedLeft = true;
                        targetUnit.leftAttacker = this;
           
                        searchingTarget = false;

                        targetUnit.targetUnit = this;
                        break;
                    }
                }

                if (!enemyUnitData.hasBeenTargetedRight)
                {
                    List<WorldTile> _pathToTarget = arenaHandler.FindPath(this.parentTile, arenaHandler.arenaTiles[enemyUnitData.posX + 1, enemyUnitData.posY], false);

                    if (_pathToTarget != null)
                    {
                        pathToTarget = _pathToTarget;
                        targetTile = arenaHandler.arenaTiles[enemyUnitData.posX + 1, enemyUnitData.posY];
                        hasTargetRight = true;
                      
                        targetUnit = enemyUnitData;
                        targetUnit.hasBeenTargetedRight = true;
                        targetUnit.rightAttacker = this;
                        searchingTarget = false;
                        break;
                    }
                }
            }
            else
            {
                continue;
            }
        }

        searchingTarget = false;
    }

    void InvalidateTarget()
    {
        hasTargetLeft = false;
        hasTargetLeft = false;
        targetUnit = null;
        targetTile = null;
        pathToTarget = null;
    }

    private void Update()
    {
        
        if (!lookUp)
        {
            return;
        }

        internalAttackTimer += Time.deltaTime * attacklenghtMultipler;

        switch (state)
        {
            case UnitState.IDLE:
                //if no target in range, idle, and search
                if (stateChanged)
                {
                    animator.SetTrigger("Idle");
                    stateChanged = false;
                }

                if (targetUnit != null)
                {
                    List<WorldTile> newPath = new List<WorldTile>();
                    if (hasTargetLeft)
                    {
                        newPath = arenaHandler.FindPath(this.parentTile, arenaHandler.arenaTiles[targetUnit.parentTile.posX - 1, targetUnit.parentTile.posY], false);
                        if (newPath != null)
                        {
                            pathToTarget = newPath;
                            targetTile = arenaHandler.arenaTiles[targetUnit.posX - 1, targetUnit.posY];
                        }
                        else
                        {
                            state = UnitState.IDLE;
                            stateChanged = true;
                            InvalidateTarget();
                            break;
                        }
                    }
                    else if (hasTargetRight)
                    {
                        newPath = arenaHandler.FindPath(this.parentTile, targetTile, false);

                        if (newPath != null)
                        {
                            pathToTarget = newPath;
                            targetTile = arenaHandler.arenaTiles[targetUnit.posX + 1, targetUnit.posY];
                        }
                        else
                        {
                            state = UnitState.IDLE;
                            stateChanged = true;
                            InvalidateTarget();
                            break;
                        }
                    }

                    //somewhere here to a validation of the enemy unit 
                    if (newPath != null)
                    {
                        pathToTarget = newPath;
                    }
                    else
                    {
                        state = UnitState.IDLE;
                        stateChanged = true;
                        break;
                    }
                }

                if (!hasTargetLeft && !hasTargetRight && !searchingTarget)
                {
                    searchingTarget = true;
                    FindTarget();
                    break;
                }

                if (hasTargetLeft || hasTargetRight)
                {
                    if (!searchingTarget && pathToTarget != null && pathToTarget.Count > 0)
                    {
                        state = UnitState.WALKINGTOWARDS;
                        stateChanged = true;
                    }
                    else
                    {
                        hasTargetLeft = false;
                        hasTargetRight = false;
      
                    }
                }
                break;
            case UnitState.ATTACKING:

                if (stateChanged)
                {
                   
                    stateChanged = false;
                }

                if (!hasTargetLeft && !hasTargetRight)
                {
                    state = UnitState.IDLE;
                    stateChanged = true;
                    break;
                }

                if (targetUnit == null || targetUnit.item.invalidated || !targetUnit.gameObject.activeSelf)
                {
                    InvalidateTarget();
                    state = UnitState.IDLE;
                    stateChanged = true;
                    break;
                }

                if (hasTargetLeft)
                {
                    if (targetUnit.parentTile.posX != parentTile.posX + 1)
                    {
                        state = UnitState.IDLE;
                        stateChanged = true;
                        InvalidateTarget();
                        break;
                    }
                }

                if (hasTargetRight)
                {
                    if (targetUnit.parentTile.posX != parentTile.posX - 1)
                    {
                        state = UnitState.IDLE;
                        stateChanged = true;
                        InvalidateTarget();
                        break;
                    }
                }

                if (internalAttackTimer > attackSpeed)
                {
                    Attack(targetUnit);
                    animator.SetTrigger("Attacking");
                    internalAttackTimer = 0;
                }
                //if target is dead and stop and switch to searching
                break;
            case UnitState.WALKINGTOWARDS:
             
                if (moving)
                {
                    if (timeElapsedForMove < moveSpeed)
                    {
                        transform.position = Vector3.Lerp(arenaHandler.arenaTiles[prevTile.posX, prevTile.posY].transform.position,
                                                              arenaHandler.arenaTiles[parentTile.posX, parentTile.posY].transform.position, (timeElapsedForMove / moveSpeed));

                        timeElapsedForMove += Time.deltaTime;
                    }
                    else
                    {
                        transform.position = arenaHandler.arenaTiles[parentTile.posX, parentTile.posY].transform.position;
                        timeElapsedForMove = 0;
                        moving = false;

                        List<WorldTile> newPath = new List<WorldTile>();
                        if (hasTargetLeft)
                        {
                            newPath = arenaHandler.FindPath(this.parentTile, arenaHandler.arenaTiles[targetUnit.parentTile.posX-1, targetUnit.parentTile.posY] , false);
                            if (newPath != null)
                            {
                                pathToTarget = newPath;
                                targetTile = arenaHandler.arenaTiles[targetUnit.posX - 1, targetUnit.posY];
                            }
                            else
                            {
                                state = UnitState.IDLE;
                                stateChanged = true;
                                InvalidateTarget();
                                break;
                            }
                        }
                        else if (hasTargetRight)
                        {
                            newPath = arenaHandler.FindPath(this.parentTile, targetTile, false);

                            if (newPath != null)
                            {
                                pathToTarget = newPath;
                                targetTile = arenaHandler.arenaTiles[targetUnit.posX + 1, targetUnit.posY];
                            }
                            else
                            {
                                state = UnitState.IDLE;
                                stateChanged = true;
                                InvalidateTarget();
                                break;
                            }
                        }
                       
                        //somewhere here to a validation of the enemy unit 
                        if (newPath != null)
                        {
                            pathToTarget = newPath;
                        }
                        else
                        {
                            state = UnitState.IDLE;
                            stateChanged = true;
                            break;
                        }
                    }
                }
                else
                {
                    timeElapsedForMove = 0;
                    if (!hasTargetLeft && !hasTargetRight)
                    {
                        state = UnitState.IDLE;
                        stateChanged = true;
                        break;
                    }

                    if (pathToTarget == null || pathToTarget.Count == 0)
                    {
                        state = UnitState.IDLE;
                        stateChanged = true;
                        break;
                    }

                    if (parentTile == targetTile)
                    {
                        state = UnitState.ATTACKING;
                        stateChanged = true;
                        moving = false;
                        pathToTarget.Clear();
                        break;
                    }

                    WorldTile pathStep = pathToTarget[0];

                    if (pathStep == parentTile)
                    {
                        pathToTarget.RemoveAt(0);
                        break;
                    }

                    parentTile.UnitOut();
                    prevTile = parentTile;
                    parentTile = pathStep;
                    parentTile.UnitIn(this);
                    posX = parentTile.posX;
                    posY = parentTile.posY;

                    pathToTarget.RemoveAt(0);
                    moving = true;

                }
                break;
        }
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

[System.Serializable]
public enum UnitState
{
    IDLE,
    SEARCHING,
    WALKINGTOWARDS,
    ATTACKING
}
