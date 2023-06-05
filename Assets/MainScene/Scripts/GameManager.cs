using NaughtyAttributes;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
public class GameManager : Singleton<GameManager>
{
    public const int MAX_LEVEL_INDEX = 15;

    public float CountDown = 5;
    int asyncSceneIndex = 1;
    public bool taptic = true;

    public int deadEnemyCount;

    public static List<GameObject> allEnemiesList = new List<GameObject>();

    public bool ZombieMode;

    #region GameState
    public enum GAMESTATE
    {
        Start,
        Ingame,
        Finish,
        GameOver,
        Empty
    }
    [OnValueChanged("OnValueChanged")]
    [SerializeField] GAMESTATE _gamestate;
    public GAMESTATE Gamestate
    {
        get { return _gamestate; }
        set
        {
            _gamestate = value;
            UIManager.Instance.PanelController(_gamestate);
        }
    }
    #endregion
    void Start()
    {
        Gamestate = GAMESTATE.Start;
    }
    void Update()
    {
        if (allEnemiesList.Count <= 0)
        {
            allEnemiesList.AddRange(FindObjectsOfType<Enemy>().Select(x => x.gameObject).ToList());
            allEnemiesList.AddRange(FindObjectsOfType<Zombie>().Select(x => x.gameObject).ToList());
        }
        switch (_gamestate)
        {
            case GAMESTATE.Start:
                break;
            case GAMESTATE.Ingame:
                GameIngame();
                break;
            case GAMESTATE.Finish:
                GameFinish();
                break;
            case GAMESTATE.GameOver:
                GameOver();
                break;
            case GAMESTATE.Empty:
                Empty();
                break;
        }
    }
    #region States
    public void OnGameStart()
    {
        if (SceneManager.sceneCount > 2) return;
        SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
        StartCoroutine(SpawnManager.Instance.SetSpawner());
        deadEnemyCount = 0;
        if (Joystick.Instance != null)
        {
            Joystick.Instance.UseOnStart();
        }
        Gamestate = GAMESTATE.Ingame;
    }

    void GameIngame()
    {

    }
    void GameFinish()
    {
        CountDown -= Time.deltaTime;
        UIManager.Instance.fillImage.fillAmount = Mathf.Lerp(UIManager.Instance.fillImage.fillAmount,
            UIManager.Instance._fill, Time.deltaTime * .9f);
    }
    void GameOver()
    {
        CountDown -= Time.deltaTime;
    }

    void Empty()
    {
        CountDown -= Time.deltaTime;
        if (CountDown <= 0)
            Gamestate = GAMESTATE.GameOver;
    }
    public void RestartButton()
    {
        Gamestate = GAMESTATE.Start;
        CountDown = 2;
        SceneManager.UnloadSceneAsync(asyncSceneIndex);
        OnGameStart();
    }
    public void NextLevelButton()
    {
        if (SceneManager.sceneCount > 1)
        {
            SceneManager.UnloadSceneAsync(asyncSceneIndex);
            asyncSceneIndex = 2;
        }
        UIManager.Instance.SetLevel();
        PlayerPrefs.SetInt("SaveScene", asyncSceneIndex);
        Gamestate = GAMESTATE.Start;
        CountDown = 2;
        OnGameStart();
    }
    #endregion
    void OnValueChanged()
    {
        Gamestate = _gamestate;
    }

    public void Zombie()
    {
        ZombieMode = true;
        OnGameStart();
    }
    public  void Normal()
    {
        ZombieMode = false;
        OnGameStart();
    }
}