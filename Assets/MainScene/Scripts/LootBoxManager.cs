using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBoxManager : Singleton<LootBoxManager>
{
    [SerializeField] private GameObject lootboxUI;

    void PauseGameForLootBox()
    {
        
    }

    void ResumeGame()
    {
        lootboxUI.SetActive(false);
    }

    public  void LootBoxStage()
    {
        // air drop||loot da çağır.
        PauseGameForLootBox();
        lootboxUI.SetActive(true);
    }

    public void LootBox1()
    {
        //functionality
        ResumeGame();
    }
    
    public void LootBox2()
    {
        ResumeGame();
    }
    
    public void LootBox3()
    {
        ResumeGame();
    }
}
