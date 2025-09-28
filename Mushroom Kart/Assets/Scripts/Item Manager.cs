using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Nothing,
    Mushroom
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    private const float TIME_TO_GET_ITEM = 2;
    private const float ITEMSLOT_CHANGE_TIME = 0.1f;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void UseItem(Racer racer, ItemType item)
    {
        switch (item)
        {
            case ItemType.Mushroom:
                racer.Boost(12);
                break;
        }
    }

    public void GiveItem(Racer racer)
    {
        // Get Item
        ItemType item = GetItem();

        StartCoroutine(WaitToGiveItemToPlayer(racer, item));
    }

    public Sprite[] itemSlots;

    private IEnumerator WaitToGiveItemToPlayer(Racer racer, ItemType item)
    {
        int itemIndex = 0;

        Image itemSlot = racer.GetItemSlotImage(itemIndex);
        Sprite[] sprites = itemSlots;
        int spriteIndex = Random.Range(0, sprites.Length);

        for (float i = 0; i < TIME_TO_GET_ITEM; i += ITEMSLOT_CHANGE_TIME)
        {
            itemSlot.sprite = sprites[spriteIndex];
            spriteIndex++;
            if (spriteIndex >= sprites.Length) spriteIndex = 0;

            if (itemIndex == 1 && racer.GetItemSlots()[0] == ItemType.Nothing)
            {
                itemIndex = 0;
                itemSlot = racer.GetItemSlotImage(itemIndex);
            }

            yield return new WaitForSeconds(ITEMSLOT_CHANGE_TIME);
        }

        racer.SetItem(item, itemIndex);
        yield break;
    }

    public ItemType GetItem()
    {
        return ItemType.Mushroom;
    }
}
