using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using static System.String;

[RequireComponent(typeof(AudioSource))]
internal sealed class CyclicAudioPlay : MonoBehaviour
{
    private AudioSource _audioSource;
    private IList<string> _tracks;
    private int _trackIdx;
    [SerializeField]
    private SoundsDatabase sounds;

    private void Start()
    {
        _tracks = new List<string>(sounds.GetAllClipNames().Randomize());
        Debug.Log($"Tracks will be playing in order: {Join(", ", _tracks.ToArray())}");
        _audioSource = GetComponent<AudioSource>();
        if (_tracks.Count > 0)
        {
            StartCoroutine(PlayNextTrack());
        }
    }

    private IEnumerator PlayNextTrack()
    {
        while (true)
        {
            if (_audioSource.isPlaying)
            {
                yield return new WaitForSeconds(1.0f);
                Debug.Log($"Wait 1s");
            }

            var trackName = _tracks[_trackIdx];
            _trackIdx = (_trackIdx + 1) % _tracks.Count;
            var nextClip = sounds.GetClip(trackName);
            Debug.Log($"Track starting to play track: {trackName} with duration: {(int)nextClip.length}sec");
            _audioSource.clip = nextClip;
            _audioSource.Play();
            yield return new WaitForSeconds(nextClip.length);
        }
    }
}
