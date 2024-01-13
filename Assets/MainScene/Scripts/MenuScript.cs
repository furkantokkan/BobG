using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
   public TextMeshProUGUI Cash;
   public TextMeshProUGUI[] Skill_Level_Text;

   [SerializeField] Progression progression = null;
   [SerializeField] private GameObject weaponRoot;
   [SerializeField] private GameObject armorRoot;
   
   private void OnEnable()
   {
      Cash.text = PlayerPrefs.GetInt("Coin", 0).ToString();
      Skill_Level_Text[0].text = "Level " + PlayerPrefs.GetInt("IncomeLevel", 1);
      Skill_Level_Text[1].text = "Level " + PlayerPrefs.GetInt("PowerLevel", 1);
      Skill_Level_Text[2].text = "Level " + PlayerPrefs.GetInt("ArmorLevel", 1);
      Skill_Level_Text[3].text = "Level " + PlayerPrefs.GetInt("SpeedLevel", 1);
      Skill_Level_Text[4].text = "Level " + PlayerPrefs.GetInt("HealthLevel", 1);
      
      weaponRoot.GetComponent<Root>().ActivateObject(progression.GetPrefabID(PlayerPrefs.GetInt("PowerLevel", 1), CharacterClass.Player, Stat.POWER));
      armorRoot.GetComponent<Root>().ActivateObject(progression.GetPrefabID(PlayerPrefs.GetInt("ArmorLevel", 1), CharacterClass.Player, Stat.ARMOR));
   }
}
