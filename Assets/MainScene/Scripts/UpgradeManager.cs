using System;
using TMPro;
using UnityEngine;
public class UpgradeManager : Singleton<UpgradeManager>
{
    [SerializeField]float MoneyUpgrade, DamageUpgrade;

    //[SerializeField]
    //private TextMeshProUGUI moneyLevelTxt,
    //    damageLevelTxt,
    //    airdropLevelTxt,
    //    moneyCostTxt,
    //    damageCostTxt,
    //    airdropCostTxt;

    private int _moneylevel,
        _airdroplevel,
        _damagelevel,
        _moneycost,
        _airdropcost,
        _damagecost,
        _moneyamount;
    
    public int MoneyAmount
    {
        get
        {
            return _moneyamount;
        }
        set
        {
            _moneyamount = value;
            PlayerPrefs.SetInt("MoneyAmount",_moneyamount);
        }
    }
    
    int MoneyCost
    {
        get
        {
            return _moneycost;
        }
        set
        {
            _moneycost = value;
            //moneyCostTxt.text = _moneycost.ToString();
            PlayerPrefs.SetInt("MoneyCost", _moneycost);
        }
    }
    
    int MoneyLevel
    {
        get
        {
            return _moneylevel;
        }
        set
        {
            _moneylevel = value;
            //moneyLevelTxt.text = "Level " + _moneylevel;
            PlayerPrefs.SetInt("MoneyLevel", _moneylevel);
        }
    }

    int AirDropCost
    {
        get
        {
            return _airdropcost;
        }
        set
        {
            _airdropcost = value;
            //airdropCostTxt.text = _airdropcost.ToString();
            PlayerPrefs.SetInt("AirDropCost", _airdropcost);
        }
    }
    
    int AirDropLevel
    {
        get
        {
            return _airdroplevel;
        }
        set
        {
            _airdroplevel = value;
            //airdropLevelTxt.text = "Level " + _airdroplevel;
            PlayerPrefs.SetInt("AirDropLevel", _airdroplevel);
        }
    }

    int DamageCost
    {
        get
        {
            return _damagecost;
        }
        set
        {
            _damagecost = value;
            //damageCostTxt.text = _damagecost.ToString();
            PlayerPrefs.SetInt("DamageCost", _damagecost);
        }
    }
    
    int DamageLevel
    {
        get
        {
            return _damagelevel;
        }
        set
        {
            _damagelevel = value;
            //damageLevelTxt.text = "Level " + _damagelevel;
            PlayerPrefs.SetInt("DamageLevel", _damagelevel);
        }
    }

    private LootBoxManager _lootBoxManager;
    
    private void Awake()
    {
        _lootBoxManager = GetComponent<LootBoxManager>();
    }

    void Start()
    {
        UIManager.Instance.Coin = 200;
        MoneyCost = PlayerPrefs.GetInt("MoneyCost", 50);
        AirDropCost = PlayerPrefs.GetInt("AirDropCost", 50);
        DamageCost = PlayerPrefs.GetInt("DamageCost", 50);
        MoneyLevel = PlayerPrefs.GetInt("MoneyLevel",1);
        AirDropLevel = PlayerPrefs.GetInt("AirDropLevel", 1);
        DamageLevel = PlayerPrefs.GetInt("DamageLevel", 1);
        MoneyAmount = PlayerPrefs.GetInt("MoneyAmount", 2);
        PlayerPrefs.GetInt("Damage", 100);
    }

    public void BuyMoney()
    {
        //if(MoneyCost > UIManager.Instance.Coin)
        //    return;
        UIManager.Instance.Coin += -MoneyCost;
        MoneyCost += MoneyCost/MoneyLevel;
        MoneyLevel += 1;
        MoneyAmount += (int)MoneyUpgrade;
    }
    
    public void BuyAirDrop()
    {
        //if(MoneyCost > UIManager.Instance.Coin)
        //    return;
        UIManager.Instance.Coin += AirDropCost;
        AirDropCost += AirDropCost/AirDropLevel;
        AirDropLevel += 1;
        _lootBoxManager.LootBoxStage();
    }
    
    public void BuyDamage()
    {
        //if(MoneyCost > UIManager.Instance.Coin)
        //    return;
        UIManager.Instance.Coin += -DamageCost;
        DamageCost += (DamageCost / DamageLevel);
        DamageLevel += 1;
        PlayerPrefs.SetFloat("Damage", PlayerPrefs.GetFloat("Damage", 100) + DamageUpgrade);
    }
}