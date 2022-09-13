using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Progressin", menuName = "Stats/New Progression", order = 0)]
public class Progression : ScriptableObject
{
    [SerializeField] private ProgressionCharacterClass[] characterClasses = null;

    private struct KeyValues
    {
       public int levelID;
       public Stat statID;
    }

    Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;
    Dictionary<CharacterClass, Dictionary<KeyValues, int[]>> prefabLookupTable = null;

    public float GetStat(Stat newStat, CharacterClass newCharacterClass, int level)
    {
        #region OLD LOOK UP
        //foreach (ProgressionCharacterClass progressionClass in characterClasses)
        //{
        //    if (progressionClass.characterClass != newCharacterClass)
        //    {
        //        continue;
        //    }

        //    foreach (ProgressionStat progressionStat in progressionClass.stats)
        //    {
        //        if (progressionStat.stat != newStat)
        //        {
        //            continue;
        //        }
        //        if (progressionStat.levels.Length < level)
        //        {
        //            continue;
        //        }

        //        return progressionStat.levels[level - 1];
        //    }
        //}
        //return 0f;
        #endregion
        BuildLookup();

        float[] levels = lookupTable[newCharacterClass][newStat];
        if (levels.Length < level)
        {
            return 0;
        }

        return levels[level - 1];
    }
    public int GetPrefabID(int level, CharacterClass newCharacterClass, Stat newStat)
    {
        BuildLookup();

        KeyValues key = new KeyValues();
        key.levelID = level;
        key.statID = newStat;

        int[] prefabs = prefabLookupTable[newCharacterClass][key];
        if (prefabs.Length < level)
        {
            Debug.LogWarning("Object is null");
            return 0;
        }

        return prefabs[level - 1];
    }
    public int GetLevelLength(Stat newStat, CharacterClass newCharacterClass)
    {
        BuildLookup();
        float[] levels = lookupTable[newCharacterClass][newStat];
        return levels.Length;
    }

    private void BuildLookup()
    {
        if (lookupTable != null || prefabLookupTable != null)
        {
            return;
        }

        lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
        prefabLookupTable = new Dictionary<CharacterClass, Dictionary<KeyValues, int[]>>();

        foreach (ProgressionCharacterClass progressionClass in characterClasses)
        {
            var statLookupTable = new Dictionary<Stat, float[]>();
            var levelLookupTable = new Dictionary<KeyValues, int[]>();

            foreach (ProgressionStat progressionStat in progressionClass.stats)
            {
                statLookupTable[progressionStat.stat] = progressionStat.levels;
                for (int i = 0; i < progressionStat.weaponID.Length; i++)
                {
                    KeyValues newKey = new KeyValues();
                    newKey.levelID = i;
                    newKey.statID = progressionStat.stat;

                    levelLookupTable[newKey] = progressionStat.weaponID;
                    Debug.Log("Index: " + i + " / WeaponID: " + progressionStat.weaponID[i]);
                }
            }



            lookupTable[progressionClass.characterClass] = statLookupTable;
            prefabLookupTable[progressionClass.characterClass] = levelLookupTable;
        }
    }

    [System.Serializable]
    class ProgressionCharacterClass
    {
        public CharacterClass characterClass;
        public ProgressionStat[] stats;
    }

    [System.Serializable]
    class ProgressionStat
    {
        public Stat stat;
        public int[] weaponID;
        public float[] levels;
    }
}
