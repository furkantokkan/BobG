using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressController : MonoBehaviour
{
    [Range(1, 99)]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] CharacterClass characterClass;
    [SerializeField] Progression progression = null;
    [SerializeField] private GameObject levelUpParticule;


    public int incomeLevel;
    public int powerLevel;
    public int armorLevel;
    public int speedLevel;
    public int healthLevel;

    public event Action onLevelUp;

    private void Awake()
    {
        if (gameObject.tag == "Player")
        {
            if (PlayerPrefs.HasKey("IncomeLevel"))
            {
                incomeLevel = PlayerPrefs.GetInt("IncomeLevel");
            }
            else
            {
                incomeLevel = startingLevel;
            }

            if (PlayerPrefs.HasKey("PowerLevel"))
            {
                powerLevel = PlayerPrefs.GetInt("PowerLevel");
            }
            else
            {
                powerLevel = startingLevel;
            }

            if (PlayerPrefs.HasKey("ArmorLevel"))
            {
                armorLevel = PlayerPrefs.GetInt("ArmorLevel");
            }
            else
            {
                armorLevel = startingLevel;
            }

            if (PlayerPrefs.HasKey("SpeedLevel"))
            {
                speedLevel = PlayerPrefs.GetInt("SpeedLevel");
            }
            else
            {
                speedLevel = startingLevel;
            }

            if (PlayerPrefs.HasKey("HealthLevel"))
            {
                healthLevel = PlayerPrefs.GetInt("HealthLevel");
            }
            else
            {
                healthLevel = startingLevel;
            }

        }
        else
        {
            incomeLevel = startingLevel;
            powerLevel = startingLevel;
            armorLevel = startingLevel;
            speedLevel = startingLevel;
            healthLevel = startingLevel;
        }
    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    private void UpdateLevel(int newLevel, Stat newStat)
    {
        switch (newStat)
        {
            case Stat.INCOME:
                incomeLevel = newLevel;
                PlayerPrefs.SetInt("IncomeLevel", newLevel);
                break;
            case Stat.POWER:
                powerLevel = newLevel;
                PlayerPrefs.SetInt("PowerLevel", newLevel);
                break;
            case Stat.ARMOR:
                powerLevel = newLevel;
                PlayerPrefs.SetInt("ArmorLevel", newLevel);
                break;
            case Stat.SPEED:
                powerLevel = newLevel;
                PlayerPrefs.SetInt("SpeedLevel", newLevel);
                break;
            case Stat.HEALTH:
                powerLevel = newLevel;
                PlayerPrefs.SetInt("HealthLevel", newLevel);
                break;
        }

        onLevelUp?.Invoke();
        LevelUpEffect();
    }
    private void LevelUpEffect()
    {
        Instantiate(levelUpParticule, transform);
    }
}
