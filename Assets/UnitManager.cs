using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
  
    public WorldUnit selectedUnit;

    public bool tileSelectMode;
    public WorldTile startTile;

    public List<WorldTile> walkList;

    public float moveAnimationLenght = .5f;
    public bool runningMoveSequence = false;
    private void Awake()
    {
        Instance = this;
    }


    public void ClearTileSelectMode()
    {
        ClearFoundTiles();
        tileSelectMode = false;
        selectedUnit = null;
    }

    public void ClearFoundTiles()
    {
        foreach (WorldTile tile in walkList)
        {
            tile.HideHighlight();
        }
    }

    public bool IsTileValidMove(WorldTile tile)
    {
        if (walkList.Contains(tile) && selectedUnit.movePoints > 0) { return true; }
        else { return false; }
    }

    public void SpawnUnitAt(WorldTile targetHex, GameObject unitPrefab)
    {
       
       
    }

    public void SelectUnit(WorldUnit newUnit)
    {
       
        if (selectedUnit != null)
        {
            selectedUnit.Deselect();
        }

        ClearTileSelectMode();

        selectedUnit = newUnit;


        if (selectedUnit.movePoints > 0 && !selectedUnit.hasMoved)
        {
            walkList = GetWalkableTiles(newUnit);

            if (walkList.Count == 0)
            {
                selectedUnit.noValidMove = true;
            }
            else
            {
                foreach (WorldTile tile in walkList)
                {
                    tile.ShowHighlight(false);
                }
            }
        }

        if (selectedUnit.attackCharges > 0 && !selectedUnit.hasAttacked)
        {
            //selectedAttackList = GetAttackableHexes(newUnit);
            /*
            if (selectedAttackList.Count > 0)
            {
                foreach (WorldTile hex in selectedAttackList)
                {
                    hex.ShowHighlight(true);
                }

                selectedUnit.noAttackHexInRange = false;
            }
            else
            {
                selectedUnit.noAttackHexInRange = true;
            }*/
        }

        selectedUnit.ValidateActions();

        if (selectedUnit.isInteractable)
        {
            if (!selectedUnit.noAttackTileInRange || !selectedUnit.noTileInRange)
            {
                tileSelectMode = true;
            }
               
        }

    }

    public void MoveToTargetTile(WorldTile targetTile)
    {
        ClearFoundTiles();
        tileSelectMode = false;
        runningMoveSequence = true;
        StartCoroutine(MoveSequence(targetTile));
    }

    IEnumerator MoveSequence(WorldTile targetTile)
    {
        List<WorldTile> path = UnitManager.Instance.FindPath(selectedUnit, selectedUnit.parentTile, targetTile);

        if (path == null)
        {
            path = UnitManager.Instance.FindPath(selectedUnit, selectedUnit.parentTile, targetTile, true);
        }

        if (path == null)
        {
            Debug.LogWarning("Tried to move to tile with no path. Aborted");
            selectedUnit.assignedPathTarget = null;
            yield break;
        }

        selectedUnit.Deselect();
        selectedUnit.parentTile.UnitOut();

        tileSelectMode = false;

        WorldTile tempPathParent = selectedUnit.parentTile;
        //particle maybe

        selectedUnit.hasMoved = true;
        selectedUnit.movePoints--;

        //maybe animation here 

        foreach (WorldTile pathStep in path)
        {
            if (pathStep == selectedUnit.parentTile)
            {
                continue;
            }

            //selectedUnit.UpdateDirection(tempPathParent, pathStep, false);
            selectedUnit.oldPosition = tempPathParent.PositionVector;
            selectedUnit.newPosition = pathStep.PositionVector;

            if (pathStep.type == TileType.WATER)
            {
                // SI_AudioManager.Instance.Play(SI_AudioManager.Instance.selectSeaSound);
            }
            else
            {
                //SI_AudioManager.Instance.Play(SI_AudioManager.Instance.unitMove);
            }

            if (moveAnimationLenght > 0)
            {
                float elapsedTime = 0;

                while (elapsedTime < moveAnimationLenght)
                {
                    selectedUnit.transform.position = Vector3.Lerp(selectedUnit.oldPosition, selectedUnit.newPosition, (elapsedTime / moveAnimationLenght));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }


            if (pathStep == targetTile)
            {
                selectedUnit.parentTile = pathStep;
                selectedUnit.parentTile.UnitIn(selectedUnit);

                selectedUnit.posX = selectedUnit.parentTile.posX;
                selectedUnit.posY = selectedUnit.parentTile.posY;

                //selectedUnit.visualAnim.SetTrigger("Idle");
                selectedUnit.MoveCompleted();
                SelectUnit(selectedUnit);
               
                runningMoveSequence = false;
                yield break;

            }

            //pathStep.SpawnParticle(UnitManager.Instance.unitWalkParticle);
            //pathStep.Wiggle();
            tempPathParent = pathStep;
        }
    }

    public List<WorldTile> GetWalkableTiles(WorldUnit targetUnit, int customRange = -1)
    {
        int range = targetUnit.walkRange;
        startTile = targetUnit.parentTile;

        int roadModifier = targetUnit.roadModifier; // this should probably be in the unit reference, but alas, don't think we're gonna change it. 
        bool ignoreRoads = false;

        if (customRange != -1)
        {
            range = customRange;
            roadModifier = 0;
            ignoreRoads = true;
        }

        List<WorldTile> tilesInGeneralRange = MapGenerator.Instance.GetTileListWithinRadius(startTile, range);

        if (tilesInGeneralRange.Contains(startTile))
        {
            tilesInGeneralRange.Remove(startTile);
        }

        List<WorldTile> hexesToRemove = new List<WorldTile>();

        foreach (WorldTile tile in tilesInGeneralRange)
        {
            if (!tile.CanBeWalked())
            {
                hexesToRemove.Add(tile);
                continue;
            }
        }

        foreach (WorldTile tile in hexesToRemove)
        {
            if (tilesInGeneralRange.Contains(tile))
            {
                tilesInGeneralRange.Remove(tile);
            }
        }

        List<WorldTile> checkedHexes = new List<WorldTile>();

        foreach (WorldTile tile in tilesInGeneralRange)
        {
            List<WorldTile> pahtToTile = FindPath(targetUnit, targetUnit.parentTile, tile);

            if (pahtToTile != null)
            {
                checkedHexes.Add(tile);
            }
        }

        if (targetUnit.parentTile.hasRoad && !ignoreRoads)
        {
            List<WorldTile> roadConnectedHexes = GetRoadPaths(targetUnit, checkedHexes);

            if (roadConnectedHexes != null)
            {
                foreach (WorldTile hex in roadConnectedHexes)
                {
                    checkedHexes.Add(hex);
                }
            }
        }

        return checkedHexes;
    }

    public List<WorldTile> FindPath(WorldUnit unit, WorldTile start, WorldTile end, bool roadCheck = false)
    {
        List<WorldTile> openSet = new List<WorldTile>();
        List<WorldTile> closedSet = new List<WorldTile>();

        openSet.Add(start);
        bool startedFromSea = false;

        int maxDistance = unit.walkRange;

        if (roadCheck)
        {
            maxDistance += unit.roadModifier;
        }

        if (start.type == TileType.WATER)
        {
            startedFromSea = true;
            if (roadCheck)
            {
                return null;
            }
        }

        start.gCost = 0;
        start.hCost = 0;

        while (openSet.Count > 0)
        {
            WorldTile current = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < current.fCost ||
                    openSet[i].fCost == current.fCost && openSet[i].hCost < openSet[i].hCost)
                {
                    current = openSet[i];
                }
            }

            openSet.Remove(current);

            closedSet.Add(current);

            if (current == end)
            {
                List<WorldTile> path = new List<WorldTile>();
                WorldTile retractCurrent = end;

                while (retractCurrent != start)
                {
                    path.Add(retractCurrent);
                    retractCurrent = retractCurrent.pathParent;
                }

                path.Reverse();
                return path;
            }

            bool shouldSkipAdj = false;

            if (!startedFromSea)
            {
                if (current.type == TileType.WATER)
                {
                    shouldSkipAdj = true;
                }
            }
            else
            {
                if (current.type != TileType.WATER)
                {
                    shouldSkipAdj = true;
                }
            }

            if (roadCheck)
            {
                if (!current.hasRoad)
                {
                    shouldSkipAdj = true;
                }
            }

            if (!shouldSkipAdj)
            {
                foreach (WorldTile adj in current.adjacent)
                {

                    if (closedSet.Contains(adj) || !adj.CanBeWalked(adj == end)) //can't afford to enter
                    {
                        continue;
                    }

                    //distance from the starting tile
                    int distanceFromSource = MapGenerator.Instance.GetDistance(start, adj);

                    if (distanceFromSource > maxDistance)
                    {
                        continue;
                    }

                    //distance from this tile to the next 
                    int distanceCost = MapGenerator.Instance.GetDistance(current, adj);
                    int movementCostToAdj = current.gCost + distanceCost;

                    if (movementCostToAdj <= maxDistance)
                    {
                        if (movementCostToAdj < adj.gCost || !openSet.Contains(adj))
                        {
                            adj.gCost = movementCostToAdj;
                            int distancCostforH = MapGenerator.Instance.GetDistance(adj, end);
                            adj.hCost = distancCostforH;
                            adj.pathParent = current;

                            if (!openSet.Contains(adj))
                            {
                                openSet.Add(adj);
                            }
                        }
                    }

                }
            }
        }

        return null;
    }


    public List<WorldTile> GetRoadPaths(WorldUnit targetUnit, List<WorldTile> checkHexes)
    {
        //call this after the normal GetWalkHexes to cross Reference;
        int range = targetUnit.walkRange + targetUnit.roadModifier;
        startTile = targetUnit.parentTile;

        if (!startTile.hasRoad)
        {
            return null;
        }

        //get the range we'd have if everything was connected with a raod
        //possible optimization here to have a function to return us only the hexes at the specific range. So if road adds +1, we check for unit.range + roadrange only
        List<WorldTile> hexesInGeneralRange = MapGenerator.Instance.GetTileListWithinRadius(startTile, range);
        //remove the start hex
        if (hexesInGeneralRange.Contains(startTile))
        {
            hexesInGeneralRange.Remove(startTile);
        }

        //remove the hexes we can already reach and have validated in previous step
        List<WorldTile> crossReferenceHexes = new List<WorldTile>();

        foreach (WorldTile hex in hexesInGeneralRange)
        {
            if (!checkHexes.Contains(hex))
            {
                crossReferenceHexes.Add(hex);
            }
        }

        //Try to find paths for each new hex;
        List<WorldTile> hexesWithPath = new List<WorldTile>();
        foreach (WorldTile hex in crossReferenceHexes)
        {
            List<WorldTile> pathToHex = FindPath(targetUnit, targetUnit.parentTile, hex, true);
            if (pathToHex != null)
            {
                hexesWithPath.Add(hex);
            }
        }

        return hexesWithPath;

    }
}
