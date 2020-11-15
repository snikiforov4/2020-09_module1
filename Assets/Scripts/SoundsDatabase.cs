using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundsDatabase", menuName = "Sounds Database")]
public class SoundsDatabase : ScriptableObject
{
    [SerializeField] private List<SoundData> soundsDatabase = new List<SoundData>();

    [Serializable]
    public class SoundData
    {
        public string name;
        public AudioClip clip;
    }

    public AudioClip GetClip(string nameClip)
    {
        return soundsDatabase.Where(dataSound => dataSound.name == nameClip).
            Select(dataSound => dataSound.clip).FirstOrDefault();
    }
}