using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Nothing,
    GettingItem,
    Mushroom,
    Banana,
    GreenShell
}

public enum ItemDirection
{
    Foward,
    Backward 
}

public class ItemManager : MonoBehaviour
{
    [SerializeField] private Sprite emptyItemSlot;
    public static ItemManager Instance;

    public ItemPlacements itemPlacements;

    [SerializeField] private GameObject[] itemGameObjects;

    private const float TIME_TO_GET_ITEM = 2;
    private const float ITEMSLOT_CHANGE_TIME = 0.1f;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void UseItem(Racer racer, ItemType item, ItemDirection dir)
    {
        if (item == ItemType.Nothing || item == ItemType.GettingItem) return;
        switch (item)
        {
            case ItemType.Mushroom:
                racer.Boost(12);
                break;
            case ItemType.Banana:
                if (dir == ItemDirection.Backward) SpawnItem(racer, itemGameObjects[0], -racer.transform.right * 75f);
                else
                {
                    SpawnItem(racer, itemGameObjects[0], racer.transform.right * 600);
                }
                break;
            case ItemType.GreenShell:
                if (dir == ItemDirection.Backward) SpawnItem(racer, itemGameObjects[1], -racer.transform.right * 250);
                else
                {
                    SpawnItem(racer, itemGameObjects[1], racer.transform.right * 250);
                }
                break;
        }

        if (racer.GetItemSlots()[1] != ItemType.Nothing && racer.GetItemSlots()[1] != ItemType.GettingItem)
        {
            racer.GetItemSlotImage(0).sprite = racer.GetItemSlotImage(1).sprite;
            racer.GetItemSlotImage(1).sprite = emptyItemSlot;
            racer.SetItem(racer.GetItemSlots()[1], 0);
            racer.SetItem(ItemType.Nothing, 1);
        }
        else
        {
            racer.GetItemSlotImage(0).sprite = emptyItemSlot;
            racer.GetItemSlots()[0] = ItemType.Nothing;
        }
    }
    
    private void SpawnItem(Racer racer,GameObject itemObject,Vector3 dir)
    {
        GameObject item = Instantiate(itemObject, racer.transform.position, Quaternion.identity);

        item.GetComponent<Rigidbody2D>().AddForce(dir);

        item.GetComponent<Obstacle>().SetOwner(racer.gameObject);
    }

    public void GiveItem(Racer racer)
    {
        if (racer.GetItemSlots()[1] != ItemType.Nothing) return;
        // Get Item
        ItemObject item = GetItem(1);

        StartCoroutine(WaitToGiveItemToPlayer(racer, item));
    }

    private IEnumerator WaitToGiveItemToPlayer(Racer racer, ItemObject item)
    {
        int itemIndex = 0;
        if (racer.GetItemSlots()[0] != ItemType.Nothing) itemIndex = 1;

        Image itemSlot = racer.GetItemSlotImage(itemIndex);
        Sprite[] sprites = GetAllSprites();

        int spriteIndex = Random.Range(0, sprites.Length);

        racer.SetItem(ItemType.GettingItem, itemIndex);

        for (float i = 0; i < TIME_TO_GET_ITEM; i += ITEMSLOT_CHANGE_TIME)
        {
            itemSlot.sprite = sprites[spriteIndex];
            spriteIndex++;
            if (spriteIndex >= sprites.Length) spriteIndex = 0;
            
            if (itemIndex == 1 && racer.GetItemSlots()[0] == ItemType.Nothing)
            {
                racer.GetItemSlotImage(1).sprite = emptyItemSlot;
                racer.SetItem(ItemType.GettingItem, 0);
                racer.SetItem(ItemType.Nothing, 1);
                itemIndex = 0;
                itemSlot = racer.GetItemSlotImage(itemIndex);
                
            }
            yield return new WaitForSeconds(ITEMSLOT_CHANGE_TIME);
        }

        racer.SetItem(item.itemType, itemIndex);
        racer.GetItemSlotImage(itemIndex).sprite = item.itemSlotSprite;
        yield break;
    }

    private ItemObject GetItem(float distanceToFirst)
    {
        List<ItemChance> items = new List<ItemChance>();

        foreach (ItemChance item in itemPlacements.items)
        {
            if (distanceToFirst >= item.minDistance && distanceToFirst <= item.maxDistance) items.Add(item);
        }

        int totalWeight = 0;
        foreach (ItemChance item in items)
        {
            totalWeight += item.weight;
        }

        int counter = 0;
        int randomValue = Random.Range(0, totalWeight);

        foreach (ItemChance item in items)
        {
            counter += item.weight;
            if (counter > randomValue)
            {
                return item.item;
            }
        }

        return itemPlacements.items[0].item;
    }
    
    private Sprite[] GetAllSprites()
    {
        Sprite[] sprites = new Sprite[itemPlacements.items.Length];
        for(int i = 0;i < sprites.Length;i++){
            sprites[i] = itemPlacements.items[i].item.itemSlotSprite;
        }
        return sprites;  
    }
}
