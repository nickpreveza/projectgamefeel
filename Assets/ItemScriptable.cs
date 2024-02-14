using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptable", menuName = "Scriptable Objects/ItemScriptable")]
public class ItemScriptable : ScriptableObject
{
    public Item item;
    public ItemScriptable weapon;
    public ItemScriptable shield;
}
