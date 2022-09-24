using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject StartP, InGameP, NextP, GameOverP;
    TextMeshProUGUI m_CoinText, m_LevelText;
    [SerializeField] TextMeshProUGUI m_Enemy_Left;
    [SerializeField] Sprite MuteOn, MuteOff, TapticOn, TapticOff;
    public Image fillImage;
    GameObject m_Settings;
    int m_Coin;
    [HideInInspector]
    public float _fill = 0;
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
            UpdateUpgradeUI();
        }
    }

    public GameObject upgradePanel;

    [SerializeField] private int powerCost;
    [SerializeField] private int incomeCost;
    [SerializeField] private int armorCost;
    [SerializeField] private int speedCost;
    [SerializeField] private int healthCost;

    void Start()
    {
        m_LevelText = InGameP.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_CoinText = InGameP.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        m_Settings = InGameP.transform.GetChild(2).GetChild(0).gameObject;
        m_LevelText.text = "LEVEL " + PlayerPrefs.GetInt("Level", 1);
        m_Coin = PlayerPrefs.GetInt("Coin", 0);
        UpdateUpgradeUI();
    }
    private void Update()
    {
        m_Enemy_Left.text = "LEFT: " + (SpawnManager.Instance.enemySpawnCount - GameManager.Instance.deadEnemyCount).ToString();
    }
    public void UpdateUpgradeUI()
    {
        ProgressController progress = GameObject.FindGameObjectWithTag("Player").GetComponent<ProgressController>();
        UpgradePanel panel = upgradePanel.GetComponent<UpgradePanel>();

        panel.incomeLevel.text = "level " + progress.incomeLevel;
        panel.powerLevel.text = "level " + progress.powerLevel;
        panel.armorLevel.text = "level " + progress.armorLevel;
        panel.speedLevel.text = "level " + progress.speedLevel;
        panel.healthLevel.text = "level " + progress.healthLevel;

        incomeCost = 50 * progress.incomeLevel;
        powerCost = 50 * progress.powerLevel;
        armorCost = 50 * progress.armorLevel;
        speedCost = 50 * progress.speedLevel;
        healthCost = 50 * progress.healthLevel;

        panel.incomeAmount.text = incomeCost.ToString();
        panel.powerAmount.text = powerCost.ToString();
        panel.armorAmount.text = armorCost.ToString();
        panel.speedAmount.text = speedCost.ToString();
        panel.healthAmount.text = healthCost.ToString();

        if (m_Coin < incomeCost)
        {
            panel.incomeButton.interactable = false;
        }
        else
        {
            panel.incomeButton.interactable = true;
        }

        if (m_Coin < powerCost)
        {
            panel.powerButton.interactable = false;
        }
        else
        {
            panel.powerButton.interactable = true;
        }

        if (m_Coin < armorCost)
        {
            panel.armorButton.interactable = false;
        }
        else
        {
            panel.armorButton.interactable = true;
        }

        if (m_Coin < speedCost)
        {
            panel.speedButton.interactable = false;
        }
        else
        {
            panel.speedButton.interactable = true;
        }

        if (m_Coin < healthCost)
        {
            panel.healthButton.interactable = false;
        }
        else
        {
            panel.healthButton.interactable = true;
        }

    }
    public void PanelController(GameManager.GAMESTATE currentPanel)
    {
        StartP.SetActive(false);
        NextP.SetActive(false);
        GameOverP.SetActive(false);
        InGameP.SetActive(false);
        switch (currentPanel)
        {
            case GameManager.GAMESTATE.Start:
                StartP.SetActive(true);
                break;
            case GameManager.GAMESTATE.Ingame:
                InGameP.SetActive(true);
                break;
            case GameManager.GAMESTATE.GameOver:
                GameOverP.SetActive(true);
                break;
            case GameManager.GAMESTATE.Finish:
                ImageFiller();
                NextP.SetActive(true);
                break;
        }
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
    void ImageFiller()
    {
        _fill = _fill >= 1 ? 0 : _fill += .2f;
    }

    public void OnClickUpgradeButton(string name)
    {
        ProgressController progress = GameObject.FindGameObjectWithTag("Player").GetComponent<ProgressController>();

        switch (name)
        {
            case "Income":
                Coin -= incomeCost;
                progress.UpdateLevel(
                    progress.incomeLevel + 1, Stat.INCOME);
                break;
            case "Power":
                Coin -= powerCost;
                progress.UpdateLevel(
                     progress.powerLevel + 1, Stat.POWER);
                break;
            case "Armor":
                Coin -= armorCost;
                progress.UpdateLevel(
                    progress.armorLevel + 1, Stat.ARMOR);
                break;
            case "Speed":
                Coin -= speedCost;
                progress.UpdateLevel(
                     progress.speedLevel + 1, Stat.SPEED);
                break;
            case "Health":
                Coin -= healthCost;
                progress.UpdateLevel(
                    progress.healthLevel + 1, Stat.HEALTH);
                break;
        }
    }
}
