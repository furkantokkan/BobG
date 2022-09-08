using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Progressin", menuName = "Stats/New Progression", order = 0)]
public class Progression : ScriptableObject
{
    [SerializeField] private ProgressionCharacterClass[] characterClasses = null;

    Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;
    Dictionary<CharacterClass, Dictionary<int, int[]>> prefabLookupTable = null;

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
    public int GetWeaponID(int level, CharacterClass newCharacterClass)
    {
        BuildLookup();
        int[] prefabs = prefabLookupTable[newCharacterClass][level];
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
        if (lookupTable != null && prefabLookupTable != null)
        {
            return;
        }

        lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();
        prefabLookupTable = new Dictionary<CharacterClass, Dictionary<int, int[]>>();

        foreach (ProgressionCharacterClass progressionClass in characterClasses)
        {
            var statLookupTable = new Dictionary<Stat, float[]>();
            var levelLookupTable = new Dictionary<int, int[]>();

            foreach (ProgressionStat progressionStat in progressionClass.stats)
            {
                statLookupTable[progressionStat.stat] = progressionStat.levels;
                for (int i = 0; i < progressionStat.levels.Length; i++)
                {
                    levelLookupTable[i] = progressionStat.weaponID;
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
