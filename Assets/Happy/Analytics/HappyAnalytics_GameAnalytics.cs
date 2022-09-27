using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GameAnalytics integration

namespace Happy.Analytics
{
    public partial class HappyAnalytics : MonoBehaviour
    {
        private string LevelString(int levelNumber) => $"Level_{levelNumber.ToString().PadLeft(5, '0')}";

        private void InitializeSDK_GameAnalytics()
        {
            GameAnalytics.Initialize();
        }

        private void SendEvent_GameAnalytics(string eventName, int levelNumber)
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Undefined, eventName, LevelString(levelNumber));
        }

        private void LevelEvent_GameAnalytics(LevelEvents type, int levelNumber)
        {
            switch (type)
            {
                case LevelEvents.LevelStarted:
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, LevelString(levelNumber));
                    break;
                case LevelEvents.LevelFailed:
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, LevelString(levelNumber));
                    break;
                case LevelEvents.LevelCompleted:
                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, LevelString(levelNumber));
                    break;
                default:
                    break;
            }

            Debug.Log($"LevelEvent_GameAnalytics: {type} Level: {levelNumber}");
        }
    }  
}

