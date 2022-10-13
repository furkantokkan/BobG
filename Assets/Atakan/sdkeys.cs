using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Don't forget to import the HomaBelly namespace
using HomaGames.HomaBelly;

public class sdkeys : MonoBehaviour
{
    public void Awake()
    {
        if (!HomaBelly.Instance.IsInitialized)
        {
            // Listen event for initialization
            Events.onInitialized += OnInitialized;
        }
        else
        {
            // Homa Belly already initialized
        }
    }
		
    private void OnDisable()
    {
        Events.onInitialized -= OnInitialized;
    }

    private void OnInitialized()
    {
        // Homa Belly initialized, call any Homa Belly method
    }
}
