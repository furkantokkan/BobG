using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

namespace Happy.Analytics
{
    public partial class HappyAnalytics : MonoBehaviour
    {
        public static HappyAnalytics Instance;

        private enum LevelEvents
        {
            LevelStarted,
            LevelFailed,
            LevelCompleted
        }

        private IEnumerable<MethodInfo> allMethods;
        // Start is called before the first frame update

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        void Start()
        {
            DontDestroyOnLoad(gameObject);
            allMethods = typeof(HappyAnalytics).GetRuntimeMethods();
            InitializeSDK();
        }

        private void InitializeSDK()
        {
            RunMethods("InitializeSDK_");
        }

        private void RunMethods(string indicatorString, object[] parameters = null)
        {
            var allIndicatedMethods = allMethods.Where(o => o.Name.Contains(indicatorString));

            foreach (var item in allIndicatedMethods)
            {
                item.Invoke(this, parameters);
            }
        }

        public static void LevelStartEvent(int levelNumber)
        {
            Instance.LevelEvent(LevelEvents.LevelStarted, levelNumber);
        }

        public static void LevelFailEvent(int levelNumber)
        {
            Instance.LevelEvent(LevelEvents.LevelFailed, levelNumber);
        }

        public static void LevelCompleteEvent(int levelNumber)
        {
            Instance.LevelEvent(LevelEvents.LevelCompleted, levelNumber);
        }

        private void LevelEvent(LevelEvents type, int levelNumber)
        {
            var parameters = new object[] { type, levelNumber };
            RunMethods("LevelEvent_", parameters);
        }

        public static void SendEvent(string eventName, int levelNumber)
        {
            var parameters = new object[] { eventName, levelNumber };
            Instance.RunMethods("SendEvent_", parameters);
        }
    }

}
