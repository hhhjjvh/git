using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour,ISaveManager
{
    public static PlayerManager instance;
    public player1 player;

    public int currency;
    public int Coin;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }
    public void addCoin(int coin)
    {
        Coin += coin;
        AudioManager.instance.PlaySFX(29, null);
        player.entityFX.CreatePopUpText("+"+coin.ToString(), new Color(255f / 255f, 129f / 255f, 12f / 255f));
        
    }
    public void removeCoin(int coin)
    {
        Coin -= coin;
        AudioManager.instance.PlaySFX(18, null);
        player.entityFX.CreatePopUpText("-" + coin.ToString(), new Color(255f / 255f, 0f / 255f, 0f / 255f));
    }
    public bool HaveEnoughMoney(int amount)
    {
       if(amount> currency)
        {
            AudioManager.instance.PlaySFX(17, null);
            return false;
        }
       currency -= amount;
        AudioManager.instance.PlaySFX(18, null);
        return true;
    }
    public bool HaveEnoughCoin(int amount)
    {
        if (amount > Coin)
        {
            AudioManager.instance.PlaySFX(17, null);
            return false;
        }
        Coin -= amount;
        AudioManager.instance.PlaySFX(18, null);
        return true;
    }
    public void AddMoney(int amount)
    {
        currency += amount;

    }

    public int GetCurrency()
    {
        return currency;
    }
    public int GetCoin()
    {
        return Coin;
    }

    public void LoadData(GameData data)
    {
       // Debug.Log("load");
        currency = data.currency;
        Coin = data.coin;
    }

    public void SaveData(ref GameData data)
    {
       // Debug.Log("save");
       data.currency = currency;
       data.coin = Coin;
    }
}
