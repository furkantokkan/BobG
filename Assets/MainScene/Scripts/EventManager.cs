using System;

public class EventManager : SingletonPersistent<EventManager>
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
