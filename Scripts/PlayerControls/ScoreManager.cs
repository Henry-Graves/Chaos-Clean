using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;

    public static ScoreManager instance;

    public Text moneyText; 

    public Text scoreText;

    public int currency = 0; 



    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of player stats found!!");
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        moneyText.text = "$" + currency.ToString();
    }

    public int getScore()
    {
        return score;
    }


    public void IncreaseMoney(int money)
    {
        //We'll need to formulate a way to notify the UI 
            //Soapbomb explodes many mobs at once, how to handle that? 
        currency += money; 

        moneyText.text = "$" + currency.ToString();
    }

    public bool canAfford(int money)
    {
        if (currency - money < 0)
            return false;
        else 
            return true;
    }


    public void DecreaseMoney(int money)
    {
        if (!canAfford(money))
            currency = 0; 
        else
            currency -= money;

        moneyText.text = currency.ToString();
    }

    public void IncreaseScore(int reward, float multiplier = 1f)
    {
        //Please don't judge me lmao 
        score += (int)((float)reward * multiplier);
        
        scoreText.text = score.ToString();
    }

}
