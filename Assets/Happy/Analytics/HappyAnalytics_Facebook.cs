using Facebook.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Facebook integration

namespace Happy.Analytics
{
    public partial class HappyAnalytics : MonoBehaviour
    {
        private void InitializeSDK_Facebook()
        {
            FB.Init(OnFBInitComplete);
        }

        private void OnFBInitComplete()
        {
            Debug.Log("HappySDK: Facebook Initialize Complete!");
        }
    } 
}

