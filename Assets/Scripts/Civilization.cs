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

    public List<ItemType> wantedItems = new List<ItemType>();
    public List<ItemType> hatedItems = new List<ItemType>();
    public List<int> knownCivs = new List<int>();

    public int gold;
    public float Discount
    {
        get
        {
            return baseDiscount* trustforPlayer;
        }
    }

    public List<Item> ownedItems = new List<Item>();
    public List<Item> sellableItems = new List<Item>();
    public List<Item> itemToGivesAfterTrader = new List<Item>();
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


}
[System.Serializable]

public enum ItemType
{
    DAGGER,
    SWORD,
    LONGSWORD,
    SPEAR,
    BOW,
    CROSSBOW,
    SHIELD,
    POTION,
    SCROLL,
    UNIT

}

public enum RelationshipStatus
{
    HOSTILE,
    NEUTRAL,
    FRIENDLY
}
