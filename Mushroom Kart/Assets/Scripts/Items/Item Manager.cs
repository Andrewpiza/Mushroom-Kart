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
    GreenShell,
    RedShell,
    Coin
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
    private const float ITEMSLOT_CHANGE_TIME = 0.828f/4;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void UseItem(Racer racer, ItemType item, ItemDirection dir, float speed)
    {
        if (speed < 5) speed = 8;
        if (item == ItemType.Nothing || item == ItemType.GettingItem) return;
        switch (item)
        {
            case ItemType.Coin:
                racer.ChangeCoins(2);
                break;
            case ItemType.Mushroom:
                racer.Boost(12);
                break;
            case ItemType.Banana:
                if (dir == ItemDirection.Backward) SpawnItem(racer, itemGameObjects[0], -racer.transform.right * (speed*8));
                else
                {
                    SpawnItem(racer, itemGameObjects[0], racer.transform.right * (speed*40));
                }
                break;
            case ItemType.GreenShell:
                if (dir == ItemDirection.Backward) SpawnItem(racer, itemGameObjects[1], -racer.transform.right * (speed*25));
                else
                {
                    SpawnItem(racer, itemGameObjects[1], racer.transform.right * (speed*25));
                }
                break;
            case ItemType.RedShell:
                if (dir == ItemDirection.Backward) SpawnItem(racer, itemGameObjects[2], -racer.transform.right * (speed*25));
                else
                {
                    SpawnItem(racer, itemGameObjects[2], racer.transform.right * (speed*25));
                }
                break;
        }

        if (racer.GetItemSlots()[1] != ItemType.Nothing && racer.GetItemSlots()[1] != ItemType.GettingItem)
        {
            if (racer.IsPlayer())
            {
                PlayerRacer r = racer.gameObject.GetComponent<PlayerRacer>();
                r.GetItemSlotImage(0).sprite = r.GetItemSlotImage(1).sprite;
                r.GetItemSlotImage(1).sprite = emptyItemSlot;
            }
            
            racer.SetItem(racer.GetItemSlots()[1], 0);
            racer.SetItem(ItemType.Nothing, 1);
        }
        else
        {
            if (racer.IsPlayer())racer.GetComponent<PlayerRacer>().GetItemSlotImage(0).sprite = emptyItemSlot;
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
        
        racer.SetItem(ItemType.GettingItem, itemIndex);

        if (racer.IsPlayer())
        {
            PlayerRacer r = racer.gameObject.GetComponent<PlayerRacer>();
            Image itemSlot = r.GetItemSlotImage(itemIndex);
            Sprite[] sprites = GetAllSprites();

            int spriteIndex = Random.Range(0, sprites.Length);

            int playSound = 3;

            for (float i = 0; i < TIME_TO_GET_ITEM; i += ITEMSLOT_CHANGE_TIME)
            {
                itemSlot.sprite = sprites[spriteIndex];
                spriteIndex++;
                if (spriteIndex >= sprites.Length) spriteIndex = 0;

                if (playSound == 3){
                    SoundManager.Instance.PlaySound("Getting Item",0.19f);
                    playSound = 0;
                }
                else
                {
                    playSound++;
                }
                if (itemIndex == 1 && racer.GetItemSlots()[0] == ItemType.Nothing)
                {
                    r.GetItemSlotImage(1).sprite = emptyItemSlot;
                    racer.SetItem(ItemType.GettingItem, 0);
                    racer.SetItem(ItemType.Nothing, 1);
                    itemIndex = 0;
                    itemSlot = r.GetItemSlotImage(itemIndex);

                }
                yield return new WaitForSeconds(ITEMSLOT_CHANGE_TIME);
            }

            SoundManager.Instance.PlaySound("Got Item",0.18f);

            r.GetItemSlotImage(itemIndex).sprite = item.itemSlotSprite;
        }
        else
        {
            for (float i = 0; i < TIME_TO_GET_ITEM; i += ITEMSLOT_CHANGE_TIME)
            {

                if (itemIndex == 1 && racer.GetItemSlots()[0] == ItemType.Nothing)
                {
                    racer.SetItem(ItemType.GettingItem, 0);
                    racer.SetItem(ItemType.Nothing, 1);
                    itemIndex = 0;
                }
                yield return new WaitForSeconds(ITEMSLOT_CHANGE_TIME);
            }
        }
        
        racer.SetItem(item.itemType, itemIndex);
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
