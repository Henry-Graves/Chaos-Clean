using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPopupManager : MonoBehaviour
{
    public GameObject sprinklerPanel;
    public GameObject conePanel;
    public GameObject soapBombPanel;
    public GameObject roombaPanel;
    public GameObject wetfloorPanel;
    public GameObject coneUpgradePanel;
    public GameObject soapUpgradeBombPanel;
    public GameObject roombaUpgradePanel;
    public GameObject wetfloorUpgradePanel;
    public GameObject brokeBoyPopup;


    bool wasUpgradeable = false; 

    private void Start()
    {
        //Get the script from the player manager
        wasUpgradeable = false;
    }


    public void RecieveInput(int equipType)
    {
        Debug.Log(equipType);
    }


    public void TellEmToGetTheirMoneyUpNotTheirFunnyUp()
    {
        StartCoroutine(brokePopUp()); 
    }

    protected IEnumerator brokePopUp()
    {
        brokeBoyPopup.SetActive(true);
        yield return new WaitForSeconds(5f);
        brokeBoyPopup.SetActive(false);
    }


    public void enablePanel(int equipType, bool isUpgrade = false, bool shouldShow = true)
    {
        Debug.Log("EQUIP TYPE DELEGATE TEST " + equipType);
        if (equipType == (int)EquiptmentGlobals.EQUIP_ENUM.SPRINKLER)
        {
            sprinklerPanel.SetActive(shouldShow);
        }
        else if (equipType == (int)EquiptmentGlobals.EQUIP_ENUM.SOAPBOMB)
        {
            if (isUpgrade)
                soapUpgradeBombPanel.SetActive(shouldShow);
            else
                soapBombPanel.SetActive(shouldShow);
        }
        else if (equipType == (int)EquiptmentGlobals.EQUIP_ENUM.CONE)
        {
            if (isUpgrade)
                coneUpgradePanel.SetActive(shouldShow);
            else
                conePanel.SetActive(shouldShow);
        }
        else if (equipType == (int)EquiptmentGlobals.EQUIP_ENUM.WETFLOOR)
        {
            if (isUpgrade)
                wetfloorUpgradePanel.SetActive(shouldShow);
            else
                wetfloorPanel.SetActive(shouldShow);
        }
        else if (equipType == (int)EquiptmentGlobals.EQUIP_ENUM.ROOMBA)
        {
            if (isUpgrade)
                roombaUpgradePanel.SetActive(shouldShow);
            else
                roombaPanel.SetActive(shouldShow);
        }

        wasUpgradeable = isUpgrade;

    }

    public void disablePanel(int equipType, bool isUpgrade = false)
    {
        enablePanel(equipType, isUpgrade, false); 
    }

}
