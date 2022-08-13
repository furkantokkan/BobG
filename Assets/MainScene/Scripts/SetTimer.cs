using System;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviorHook : MonoBehaviour
{
  public Action onUpdate;

  private void Update()
  {
    if (onUpdate != null) onUpdate();
  }
}
public class SetTimer 
{
  private Action _action;
  private float _timer;
  private bool _isDestroyed;
  private string _timerName;
  private GameObject _gameObject;
  private static List<SetTimer> _activeTimerList;
  SetTimer(Action action, float timer, string timerName, GameObject gameObject)
  {
    _action = action;
    _timer = timer;
    _timerName = timerName;
    _gameObject = gameObject;
    _isDestroyed = false;
  }

  public static SetTimer Create(Action action, float timer , string timerName = null)
  {
    if (_activeTimerList == null)
      _activeTimerList = new List<SetTimer>();
    
    foreach (var VARIABLE in _activeTimerList)
    {
      if (VARIABLE._timerName == timerName) return null;
    }
    GameObject gameObject = new GameObject("SetTimer", typeof(MonoBehaviorHook));
    SetTimer setTimer = new SetTimer(action, timer , timerName , gameObject);
    gameObject.GetComponent<MonoBehaviorHook>().onUpdate = setTimer.Update;
    _activeTimerList.Add(setTimer);
    return setTimer;
  }
  public static void StopTimer(string timerName)
  {
    for (int i = 0; i < _activeTimerList.Count; i++)
    {
      if (_activeTimerList[i]._timerName == timerName)
      {
        _activeTimerList[i].DestroySelf();
        i--;
      }
    }
  }

  public void Update()
  {
    if (!_isDestroyed)
    {
      _timer -= Time.deltaTime;
      if (_timer < 0)
      {
        _action();
        DestroySelf();
      }
    }
  }

  void DestroySelf()
  {
    _isDestroyed = true;
    UnityEngine.Object.Destroy(_gameObject);
    _activeTimerList.Remove(this);
  }
}
