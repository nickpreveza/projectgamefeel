using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Civilization", menuName = "ScriptableObjects/Civilizations", order = 1)]
[System.Serializable]
public class CivilizationScriptable : ScriptableObject
{
    public string civName;
    public Sprite icon;
    public Color color;
    public Sprite[] citySprites; //0 village, 1 settlement, 2 castle 

    public Weapon[] weapons;
    public Unit[] units;
}

[System.Serializable]
public class Item
{
    public string id;
    public Sprite icon;
    public int level;
}

[System.Serializable]
public class Weapon : Item
{
    public int attackSpeed;
}

[System.Serializable]
public class Unit: Item
{
    public int movementSpeed;
}
