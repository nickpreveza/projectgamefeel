using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public bool tileSelectMode;
    public WorldUnit selectedUnit;

    private void Awake()
    {
        Instance = this;
    }

}
