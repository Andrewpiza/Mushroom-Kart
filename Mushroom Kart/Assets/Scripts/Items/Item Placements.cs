using UnityEngine;

[CreateAssetMenu(fileName = "Item Placement", menuName = "ScriptableObjects/Item Placement")]
public class ItemPlacements : ScriptableObject
{
    public ItemChance[] items;
}


[System.Serializable]
public class ItemChance
{
    public ItemObject item;
    public int weight = 1;
    // 0-100
    public float minDistance = 0;
    public float maxDistance = 100;
    public float minTime = 0;
}
    

