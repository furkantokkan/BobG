using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject InGameP, NextP, GameOverP, MenuP, ModeP;
    TextMeshProUGUI m_CoinText, m_LevelText;
    [SerializeField] TextMeshProUGUI m_Enemy_Left, m_FinishMoneyText;
    [SerializeField] Sprite MuteOn, MuteOff, TapticOn, TapticOff;
    private GameObject m_Settings;
    private int m_Coin;
    public GameObject upgradePanel;
    [SerializeField] private UpgradeCosts upgradeCosts;
    private UpgradeCosts currentCosts = new UpgradeCosts();
    public int Coin
    {
        get
        {
            return m_Coin;
        }
        set
        {
            m_Coin = value;
            m_CoinText.text = m_Coin.ToString();
        }
    }

    void Start()
    {
        InitializeUIElements();
    }

    private void InitializeUIElements()
    {
        m_LevelText = InGameP.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_CoinText = InGameP.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        m_Settings = InGameP.transform.GetChild(2).GetChild(0).gameObject;
        m_LevelText.text = "LEVEL " + PlayerPrefs.GetInt("Level", 1);
        m_Coin = PlayerPrefs.GetInt("Coin", 0);
    }
    private void Update()
    {
        m_Enemy_Left.text = "LEFT: " + (SpawnManager.Instance.enemySpawnCount - GameManager.Instance.deadEnemyCount);
    }
    public void UpdateUpgradeUI()
    {
        ProgressController progress = GameObject.FindGameObjectWithTag("Player").GetComponent<ProgressController>();
        UpgradePanel panel = upgradePanel.GetComponent<UpgradePanel>();

        UpdatePanelTexts(panel, progress);
        UpdatePanelCosts(panel, progress);
        UpdatePanelInteractivity(panel, progress);
    }

    private void UpdatePanelTexts(UpgradePanel panel, ProgressController progress)
    {
        panel.incomeLevel.text = "level " + progress.incomeLevel;
        panel.powerLevel.text = "level " + progress.powerLevel;
        panel.armorLevel.text = "level " + progress.armorLevel;
        panel.speedLevel.text = "level " + progress.speedLevel;
        panel.healthLevel.text = "level " + progress.healthLevel;
    }   
    private void UpdatePanelCosts(UpgradePanel panel, ProgressController progress)
    {
        currentCosts.income = upgradeCosts.income * progress.incomeLevel;
        currentCosts.power = upgradeCosts.power * progress.powerLevel;
        currentCosts.armor = upgradeCosts.armor * progress.armorLevel;
        currentCosts.speed = upgradeCosts.speed * progress.speedLevel;
        currentCosts.health = upgradeCosts.health * progress.healthLevel;

        panel.incomeAmount.text = currentCosts.income.ToString();
        panel.powerAmount.text = currentCosts.power.ToString();
        panel.armorAmount.text = currentCosts.armor.ToString();
        panel.speedAmount.text = currentCosts.speed.ToString();
        panel.healthAmount.text = currentCosts.health.ToString();
    }
    private void UpdatePanelInteractivity(UpgradePanel panel, ProgressController progress)
    {
        panel.incomeButton.interactable = m_Coin >= currentCosts.income && progress.incomeLevel < GameManager.MAX_LEVEL_INDEX;
        panel.powerButton.interactable = m_Coin >= currentCosts.power && progress.powerLevel < GameManager.MAX_LEVEL_INDEX;
        panel.armorButton.interactable = m_Coin >= currentCosts.armor && progress.armorLevel < GameManager.MAX_LEVEL_INDEX;
        panel.speedButton.interactable = m_Coin >= currentCosts.speed && progress.speedLevel < GameManager.MAX_LEVEL_INDEX;
        panel.healthButton.interactable = m_Coin >= currentCosts.health && progress.healthLevel < GameManager.MAX_LEVEL_INDEX;
    }
    public void PanelController(GameManager.GAMESTATE currentPanel)
    {
        NextP.SetActive(false);
        GameOverP.SetActive(false);
        InGameP.SetActive(false);
        ModeP.SetActive(false);
        switch (currentPanel)
        {
            case GameManager.GAMESTATE.Ingame:
                InGameP.SetActive(true);
                break;
            case GameManager.GAMESTATE.GameOver:
                GameOverP.SetActive(true);
                break;
            case GameManager.GAMESTATE.Finish:
                int earnedMoney = Random.Range(50, 100);
                m_FinishMoneyText.text = "+ " + (earnedMoney * PlayerPrefs.GetInt("Level", 1));
                Coin += earnedMoney * PlayerPrefs.GetInt("Level", 1);
                SetLevel();
                NextP.SetActive(true);
                break;
            case GameManager.GAMESTATE.Menu:
                MenuP.SetActive(true);
                break;
        }
    }

    public void ModePanel()
    {
        ModeP.SetActive(ModeP.activeInHierarchy ? false : true);
        MenuP.SetActive(MenuP.activeInHierarchy ? false : true);
    }
    
    public void SetLevel()
    {
        PlayerPrefs.SetInt("Coin", m_Coin);
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
        m_LevelText.text = "LEVEL " + PlayerPrefs.GetInt("Level", 1);
    }
    public void Settings()
    {
        if (m_Settings.activeInHierarchy)
            m_Settings.SetActive(false);
        else
            m_Settings.SetActive(true);
    }
    public void Mute()
    {
        var component = Camera.main.GetComponent<AudioListener>();
        component.enabled = !component.isActiveAndEnabled;
        m_Settings.transform.GetChild(1).GetComponent<Image>().sprite = IconChanger(MuteOn, MuteOff, component.isActiveAndEnabled);
    }
    public void Taptic()
    {
        GameManager.Instance.taptic = !GameManager.Instance.taptic;
        m_Settings.transform.GetChild(0).GetComponent<Image>().sprite = IconChanger(TapticOn, TapticOff, GameManager.Instance.taptic);
    }
    Sprite IconChanger(Sprite first, Sprite second, bool state)
    {
        return state ? first : second;
    }

    public void OnClickUpgradeButton(string name)
    {
        ProgressController progress = GameObject.FindGameObjectWithTag("Player").GetComponent<ProgressController>();

        switch (name)
        {
            case "Income":
                Coin -= currentCosts.income;
                progress.UpdateLevel(
                    progress.incomeLevel + 1, Stat.INCOME);
                break;
            case "Power":
                Coin -= currentCosts.power;
                progress.UpdateLevel(
                     progress.powerLevel + 1, Stat.POWER);
                break;
            case "Armor":
                Coin -= currentCosts.armor;
                progress.UpdateLevel(
                    progress.armorLevel + 1, Stat.ARMOR);
                break;
            case "Speed":
                Coin -= currentCosts.speed;
                progress.UpdateLevel(
                     progress.speedLevel + 1, Stat.SPEED);
                break;
            case "Health":
                Coin -= currentCosts.health;
                progress.UpdateLevel(
                    progress.healthLevel + 1, Stat.HEALTH);
                break;
        }
    }
}
[System.Serializable]
public class UpgradeCosts
{
    public int power;
    public int income;
    public int armor;
    public int speed;
    public int health;
}
