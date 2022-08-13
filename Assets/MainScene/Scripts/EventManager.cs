using System;

public class EventManager : Singleton<EventManager>
{
    public event Action OnMoved;
    void Update()
    {
        if (GameManager.Instance.Gamestate == GameManager.GAMESTATE.Ingame)
        {
            OnMoved?.Invoke();
        }
    }
}
