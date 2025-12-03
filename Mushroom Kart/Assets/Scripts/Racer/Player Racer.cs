using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerRacer : Racer
{
    private TextMeshProUGUI coinText;
    private TextMeshProUGUI lapText;
    private TextMeshProUGUI placementText;
    private Image[] itemSlotImages;

    void Awake()
    {
        coinText = GameObject.Find("Coin Text").GetComponent<TextMeshProUGUI>();

        lapText = GameObject.Find("Lap Text").GetComponent<TextMeshProUGUI>();

        placementText = GameObject.Find("Placement Text").GetComponent<TextMeshProUGUI>();

        itemSlotImages = new Image[2];
        itemSlotImages[0] = GameObject.Find("Item Slot").GetComponent<Image>();
        itemSlotImages[1] = GameObject.Find("Item Slot 2").GetComponent<Image>();
    }

    void Update()
    {
        lapText.text = lapsDone + 1 + "/" + PlacementManager.instance.maxLaps;
        placementText.text = placement + "";
        

        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && canTrick)
        {
            SoundManager.Instance.PlaySound("Trick",0.7f);
            canTrick = false;
            Boost(1.8f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseItem(ItemDirection.Backward);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            UseItem(ItemDirection.Foward);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (driftAngle == 0)
            {
                if (Vector2.SignedAngle(transform.right, rb.linearVelocity.normalized) > 0.25) driftAngle = 1;
                if (Vector2.SignedAngle(transform.right, rb.linearVelocity.normalized) < -0.25) driftAngle = -1;
            }
            else
            {
                isDrifting = true;
            }
        }
        else if (isDrifting == true)
        {
            isDrifting = false;
            driftAngle = 0;
            Boost(driftCharge);
            driftCharge = 0;
        }

        UpdateRacer();
        Move(new Vector2(xMove, yMove));
    }

    public override void ChangeCoins(int n)
    {
        base.ChangeCoins(n);

        coinText.text = amountOfCoins + "";
    }

    public Image GetItemSlotImage(int index)
    {
        return itemSlotImages[index];
    }
    
    public override bool IsPlayer()
    {
        return true;
    }
}
