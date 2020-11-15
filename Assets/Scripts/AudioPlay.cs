using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
internal sealed class AudioPlay : MonoBehaviour
{
    private AudioSource _audioSource;

    #region SO

    [SerializeField] private List<DataSound> DataSounds = new List<DataSound>();

    [Serializable]
    private sealed class DataSound
    {
        public string Name;
        public AudioClip Clip;
    }

    public AudioClip GetClip(string nameClip)
    {
        return DataSounds.Where(dataSound => dataSound.Name == nameClip).
            Select(dataSound => dataSound.Clip).FirstOrDefault();
    }

    #endregion

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(string nameClip)
    {
        var audioClip = GetClip(nameClip);
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }
}
