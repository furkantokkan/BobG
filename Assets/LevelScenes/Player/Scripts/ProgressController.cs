using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressController : MonoBehaviour
{
    [Header("Attactments")]
    [SerializeField] private GameObject weaponRoot;
    [SerializeField] private GameObject armorRoot;
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

    public event Action<Stat, int> onLevelUp;

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

        weaponRoot.GetComponent<Root>().ActivateObject(progression.GetPrefabID(powerLevel, characterClass, Stat.POWER));
        armorRoot.GetComponent<Root>().ActivateObject(progression.GetPrefabID(armorLevel, characterClass, Stat.ARMOR));
    }
    public void UpdateLevel(int newLevel, Stat newStat)
    {
        if (gameObject.tag != "Player")
        {
            return;
        }
        if (progression == null)
        {
            return;
        }
        if (newLevel > GameManager.MAX_LEVEL_INDEX)
        {
            print("Return");
            return;
        }

        switch (newStat)
        {
            case Stat.INCOME:
                incomeLevel = newLevel;
                PlayerPrefs.SetInt("IncomeLevel", newLevel);
                break;
            case Stat.POWER:
                print("After Power" + newLevel);
                powerLevel = newLevel;
                weaponRoot.GetComponent<Root>().ActivateObject(progression.GetPrefabID(newLevel, characterClass, newStat));
                PlayerPrefs.SetInt("PowerLevel", newLevel);
                break;
            case Stat.ARMOR:
                armorLevel = newLevel;
                armorRoot.GetComponent<Root>().ActivateObject(progression.GetPrefabID(newLevel, characterClass, newStat));
                PlayerPrefs.SetInt("ArmorLevel", newLevel);
                break;
            case Stat.SPEED:
                speedLevel = newLevel;
                PlayerPrefs.SetInt("SpeedLevel", newLevel);
                break;
            case Stat.HEALTH:
                healthLevel = newLevel;
                PlayerPrefs.SetInt("HealthLevel", newLevel);
                break;
        }

        UIManager.Instance.UpdateUpgradeUI();
        onLevelUp?.Invoke(newStat, newLevel);
        LevelUpEffect();
    }

    public int GetStat(Stat newStat)
    {
        int level = 0;

        switch (newStat)
        {
            case Stat.INCOME:
                level = incomeLevel;
                break;
            case Stat.POWER:
                level = powerLevel;
                break;
            case Stat.ARMOR:
                level = armorLevel;
                break;
            case Stat.SPEED:
                level = speedLevel;
                break;
            case Stat.HEALTH:
                level = healthLevel;
                break;
        }

        return (int)progression.GetStat(newStat, characterClass, level);
    }

    private void LevelUpEffect()
    {
        if (levelUpParticule == null)
        {
            return;
        }
        Instantiate(levelUpParticule, transform);
    }
}
