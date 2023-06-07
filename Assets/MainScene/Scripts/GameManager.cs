using NaughtyAttributes;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager>
{
    public const int MAX_LEVEL_INDEX = 15;

    public float CountDown = 5;
    int asyncSceneIndex = 1;
    public bool taptic = true;

    public int deadEnemyCount;

    public List<Enemy> allEnemiesList = new List<Enemy>();

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

        switch (_gamestate)
        {
            case GAMESTATE.Start:
                StartCoroutine(OnGameStart());
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
    void Update()
    {
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
        if (Input.anyKeyDown && Gamestate == GAMESTATE.Start)
            Gamestate = GAMESTATE.Ingame;
    }
    #region States
    private IEnumerator OnGameStart()
    {
        asyncSceneIndex = PlayerPrefs.GetInt("SaveScene", asyncSceneIndex);

        if (SceneManager.sceneCount > 2)
        {
            yield break;
        }

        yield return SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        switch (_gamestate)
        {
            case GAMESTATE.Start:
                StartCoroutine(SpawnManager.Instance.SetSpawner());
                deadEnemyCount = 0;
                if (Joystick.Instance != null)
                {
                    Joystick.Instance.UseOnStart();
                }
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
        StartCoroutine(OnGameStart());
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
        StartCoroutine(OnGameStart());
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