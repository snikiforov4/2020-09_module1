using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
internal sealed class AudioPlay : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField]
    private SoundsDatabase sounds;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(string nameClip)
    {
        var audioClip = sounds.GetClip(nameClip);
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }
}
