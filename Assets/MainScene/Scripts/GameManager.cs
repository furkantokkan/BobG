using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : Singleton<GameManager>
{
    public float CountDown = 2f;
    int asyncSceneIndex = 1;
    public bool taptic = true;
    #region GameState
    public enum GAMESTATE
    {
        Start,
        Ingame,
        Finish,
        GameOver
    }
    [OnValueChanged("OnValueChanged")]
    [SerializeField]GAMESTATE _gamestate;
    public GAMESTATE Gamestate
    {
        get { return _gamestate;}
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
        switch (_gamestate)
        {
            case GAMESTATE.Start:
                GameStart();
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
        }
        if (Input.anyKeyDown && Gamestate == GAMESTATE.Start)
            Gamestate = GAMESTATE.Ingame;
    }
    #region States
    
    void GameStart()
    {
        asyncSceneIndex = PlayerPrefs.GetInt("SaveScene",asyncSceneIndex);
        if(SceneManager.sceneCount < 2)
            SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
    }
    void GameIngame()
    {
   
    }
    void GameFinish()
    {
        CountDown -= Time.deltaTime;
        UIManager.Instance.fillImage.fillAmount = Mathf.Lerp(UIManager.Instance.fillImage.fillAmount,
            UIManager.Instance._fill, Time.deltaTime*.9f);
    }
    void GameOver()
    {
        CountDown -= Time.deltaTime;
    }
    public void RestartButton()
    {
        SceneManager.UnloadSceneAsync(asyncSceneIndex);
        SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
        Gamestate = GAMESTATE.Start;
        CountDown = 2;
    }
    public void NextLevelButton()
    {
        if (SceneManager.sceneCountInBuildSettings == asyncSceneIndex + 1)
        {
            SceneManager.UnloadSceneAsync(asyncSceneIndex);
            asyncSceneIndex = 1;
            SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
        }
        else
        {
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(asyncSceneIndex);
                asyncSceneIndex += 1;                
            }

            SceneManager.LoadSceneAsync(asyncSceneIndex, LoadSceneMode.Additive);
        }
        UIManager.Instance.SetLevel();
        PlayerPrefs.SetInt("SaveScene",asyncSceneIndex);
        Gamestate = GAMESTATE.Start;
        CountDown = 2;
    }
    
    #endregion
    void OnValueChanged()
    {
        Gamestate = _gamestate;
    }
}