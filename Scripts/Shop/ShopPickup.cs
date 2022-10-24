using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopPickup : MonoBehaviour
{
    // Start is called before the first frame update
    public int cost = 250;
    public EquiptmentGlobals.EQUIP_ENUM equipType;
    public bool isUpgradable = false;
    public TextMeshPro textMeshPro;
    private AudioSource buySound;
    [SerializeField] private AudioClip cashRegister;
    private bool hasDeleted = false; 


    public GameObject popUpManager; 
    private ShopPopupManager shopPopupManager;

    private void Start()
    {

        //Buy 
        //Unlock
        //Upgrade 
        //Sprinkler has special behavior 
        hasDeleted = false;
        shopPopupManager = popUpManager.gameObject.GetComponent<ShopPopupManager>();

        //Lets populate with our intial text 
        buySound = gameObject.GetComponent<AudioSource>();
        buySound.clip = cashRegister;

        if (equipType == EquiptmentGlobals.EQUIP_ENUM.SPRINKLER)
        {
            textMeshPro.text = SprinklerText();
        }
        else
            textMeshPro.text = DefaultStartText();
    }


    public string SprinklerText()
    {
        return "BUY\n" + "SPRINKLER\n" + cost; 
    }


    public string MaxUpgradedText()
    {
        return equipType.ToString() + "\n MAX \n UPGRADED";  
    }

    public virtual string DefaultStartText()
    {
        return "UNLOCK\n" + equipType.ToString() + "\n" + cost;
    }

    public virtual string UpgradeText(int price = 250)
    {
        return "UPGRADE\n" + equipType.ToString() + "\n" + price;
    }

    public virtual void TryBuyItem()
    {
        if (ScoreManager.instance.canAfford(cost) && !isUpgradable)
        {
            buySound.Play();
            EquiptmentManager.instance.getEquipmentFromShop((int)equipType, false);
            ScoreManager.instance.DecreaseMoney(cost);
            if (equipType != EquiptmentGlobals.EQUIP_ENUM.SPRINKLER)
            {
                isUpgradable = true;
                //This is also where we would change the cost if we wanted.
                textMeshPro.text = UpgradeText();
                //Lets manual change window 
                shopPopupManager.disablePanel((int)equipType);
                shopPopupManager.enablePanel((int)equipType, true);


            }

        }
        else if (ScoreManager.instance.canAfford(cost))
        {
            bool shouldNotDelete = EquiptmentManager.instance.getEquipmentFromShop((int)equipType, true);
            
            if (!hasDeleted)
            {
                ScoreManager.instance.DecreaseMoney(cost);
                buySound.Play();
            }
            
            //Display something to let them know its upgraded? 
            //Maybe prefab swap or UI number
                
            if (!shouldNotDelete && !hasDeleted)
            {
                hasDeleted = true;

                textMeshPro.text = MaxUpgradedText(); 
                //lets delete the item and the spotlight. 
                Destroy(transform.GetChild(3).gameObject);
            }

        }
        else
        {
            shopPopupManager.TellEmToGetTheirMoneyUpNotTheirFunnyUp(); 
        }
        
    }
}
