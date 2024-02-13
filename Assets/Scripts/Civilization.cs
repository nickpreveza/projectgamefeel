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

    public ItemScriptable[] storeItemsBase;
    public ItemScriptable[] giveItemsBase;
    public ItemScriptable[] startingItemsBase;

    public List<Item> storeItemsPool = new List<Item>(); //don't remove these 
    public List<Item> selectedStoreItems = new List<Item>(); //should be 4

    public WorldCity lastVisitedCity; 

    public int gold;

    public List<Item> ownedItems = new List<Item>();
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
    public string id;
    public Sprite icon;
    public int level;
    public int buyValue;
    public int sellValue;
    public ItemType type;

    public int strRequirment;
    public int conRequirment;
    public int intRequirment;

    public float attackSpeed;
    public int damageOrDefense;

    public void SetData(Item item)
    {
        id = item.id;
        icon = item.icon;
        level = item.level;
        buyValue = item.buyValue;
        sellValue = item.sellValue;
        ItemType type = item.type;

        strRequirment = item.strRequirment;
        conRequirment = item.conRequirment;
        intRequirment= item.intRequirment;

        attackSpeed = item.attackSpeed;
        damageOrDefense = item.damageOrDefense;

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
