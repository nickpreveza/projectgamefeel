using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Civilization
{
    public string name;
    public Sprite icon;
    public Sprite leaderImage;
    public string leaderName;
    public Color tileColor;
    public Color mainColor;
    public Color secondaryColor;
    public Sprite[] citySprites; //0 village, 1 settlement, 2 castle
    public float trustforPlayer = 0.5f;

    public float baseDiscount;

    public string leaderWelcomePositive;
    public string leaderWelcomeNeutral;
    public string leaderWelcomeNegative;

    public string leaderChallengePrompt;
    public string leaderGiftPrompt;
    public string leaderGiftΑccept;
    public string leaderGiftDecline;

    public List<ItemType> wantedItemsTypes = new List<ItemType>();
    public List<ItemType> hatedItemsTypes = new List<ItemType>();
    public List<int> knownCivs = new List<int>();

    [Header("ScriptableObjects")]
    public ItemScriptable[] storeItemsBase;
    public ItemScriptable[] giveItemsBase;
    public ItemScriptable[] startingItemsBase;
    public ItemScriptable[] storeUnitsBase;
    public ItemScriptable[] startingUnitsBase;

    [Header("Don't add here ")]
    public List<Item> storeItemsPool = new List<Item>(); //don't remove these 
    public List<Item> selectedStoreItems = new List<Item>(); //should be 4

    public List<Item> storeUnitsPool = new List<Item>(); //don't remove these 
    public List<Item> selectedStoreUnits = new List<Item>(); //should be 4

    public WorldCity lastVisitedCity; 

    public int gold;

    public List<Item> ownedItems = new List<Item>();
    public List<Item> formationUnits = new List<Item>();
    public List<Item> reserveUnits = new List<Item>();

    public List<Item> itemToGivesAfterTrader = new List<Item>();


    public float Discount
    {
        get
        {
            return baseDiscount* trustforPlayer;
        }
    }

  
}


[System.Serializable]
public class Item
{
    public bool invalidated;
    public string id;
    public Sprite icon;
    public int buyValue;
    public int sellValue;
    public ItemType type;

    public int strRequirment;
    public int conRequirment;
    public int dexRequirment;

    public float attackSpeed;
    public float moveSpeed;
    public int damageOrDefense;

    public Item weapon;
    public Item shield;

    public void SetData(ItemScriptable scripitable)
    {
        invalidated = scripitable.item.invalidated;
        id = scripitable.item.id;
        icon = scripitable.item.icon;
        buyValue = scripitable.item.buyValue;
        sellValue = scripitable.item.sellValue;
        ItemType type = scripitable.item.type;

        strRequirment = scripitable.item.strRequirment;
        conRequirment = scripitable.item.conRequirment;
        dexRequirment = scripitable.item.dexRequirment;

        moveSpeed = scripitable.item.moveSpeed;
        attackSpeed = scripitable.item.attackSpeed;
        damageOrDefense = scripitable.item.damageOrDefense;

        if (scripitable.weapon != null)
        {
            weapon = new Item();
            weapon.SetData(scripitable.weapon);
        }

        if (scripitable.shield != null)
        {
            shield = new Item();
            shield.SetData(scripitable.shield);
        }

    }
}
[System.Serializable]

public enum ItemType
{
    DAGGER,
    SWORD,
    LONGSWORD,
    AXE,
    SPEAR,
    BOW,
    CROSSBOW,
    SHIELD,
    POTION,
    SCROLL,
    ARMOR,
    UNIT

}

public enum RelationshipStatus
{
    HOSTILE,
    NEUTRAL,
    FRIENDLY
}

public enum StatType
{
    STR,
    CON,
    DEX
}
