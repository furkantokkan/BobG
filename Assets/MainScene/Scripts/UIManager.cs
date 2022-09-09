using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject StartP, InGameP, NextP, GameOverP;
    TextMeshProUGUI m_CoinText, m_LevelText;
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
            m_Coin += value;
            // m_CoinText.text = m_Coin.ToString();
        }
    }
    void Start()
    {
        m_LevelText = InGameP.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_CoinText = InGameP.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        m_Settings = InGameP.transform.GetChild(2).GetChild(0).gameObject;
        m_LevelText.text = "LEVEL " + PlayerPrefs.GetInt("Level", 1);
        m_Coin = PlayerPrefs.GetInt("Coin", 0);
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
        switch (name)
        {
            case "Income":
                ProgressController.Instance.UpdateLevel(
                    ProgressController.Instance.incomeLevel += 1, Stat.INCOME);
                break;
            case "Power":
                ProgressController.Instance.UpdateLevel(
                     ProgressController.Instance.powerLevel += 1, Stat.POWER);
                break;
            case "Armor":
                ProgressController.Instance.UpdateLevel(
                    ProgressController.Instance.armorLevel += 1, Stat.ARMOR);
                break;
            case "Speed":
                ProgressController.Instance.UpdateLevel(
                     ProgressController.Instance.speedLevel += 1, Stat.SPEED);
                break;
            case "Health":
                ProgressController.Instance.UpdateLevel(
                     ProgressController.Instance.healthLevel += 1, Stat.HEALTH);
                break;
        }
    }
}
