using NaughtyAttributes;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager>
{
    public const int MAX_LEVEL_INDEX = 15;

    public float CountDown = 5;
    public bool taptic = true;

    public int deadEnemyCount;

    public List<GameObject> allEnemiesList = new List<GameObject>();

    public bool ZombieMode = false;
    
    public GameObject menuObjects;

    #region GameState
    public enum GAMESTATE
    {
        Start,
        Ingame,
        Finish,
        GameOver,
        Menu,
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

    async void Update()
    {
        await System.Threading.Tasks.Task.Delay(500);

        switch (_gamestate)
        {
            case GAMESTATE.Empty:
                Empty();
                break;
        }
    }

    #region States
    public void OnGameStart()
    {
        menuObjects.SetActive(false);
        Gamestate = GAMESTATE.Ingame;
        if (SceneManager.sceneCount > 2)
            return;

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        
        deadEnemyCount = 0;

        Joystick.Instance?.UseOnStart();
    }
    public void Menu()
    {
        Gamestate = GAMESTATE.Menu;
        menuObjects.SetActive(true);
        CountDown = 2;
        SceneManager.UnloadSceneAsync(1);
    }
    
    void Empty()
    {
        CountDown -= Time.deltaTime;
        if (CountDown <= 0) Gamestate = GAMESTATE.GameOver;
        allEnemiesList = new List<GameObject>();
    }

    public void RestartButton()
    {
        Gamestate = GAMESTATE.Start;
        CountDown = 2;
        SceneManager.UnloadSceneAsync(1);
        OnGameStart();
    }

    public void NextLevelButton()
    {
        if (SceneManager.sceneCount > 1)
            SceneManager.UnloadSceneAsync(1);

        Gamestate = GAMESTATE.Start;
        CountDown = 2;
        OnGameStart();
    }
    #endregion

    void OnValueChanged()
    {
        Gamestate = _gamestate;
    }

    [ContextMenu("DeleteALLKeys")]
    public void DeleteAllKeys()
    {
        PlayerPrefs.DeleteAll();
    }
}