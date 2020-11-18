using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

internal sealed class SettingPage : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Button buttonOpen;
    [SerializeField] private Button buttonClose;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private SoundChannelSettings backgroundSettings;
    [SerializeField] private SoundChannelSettings soundEffectsSettings;

    [Serializable]
    private sealed class SoundChannelSettings
    {
        public string mixerGroupName;
        public Slider slider;
        public Text textObject;
        public string prefixText;

        private UnityAction<float> _listener;

        public void RegisterSliderListener(AudioMixer mixer)
        {
            if (_listener == null)
            {
                _listener = value =>
                {
                    var volume = Mathf.Log(value) * 20; 
                    mixer.SetFloat(mixerGroupName, volume);
                    SetText(this, volume);
                };
            }

            slider.onValueChanged.AddListener(_listener);
        }

        public void RemoveSliderListener()
        {
            if (_listener != null)
            {
                slider.onValueChanged.RemoveListener(_listener);
            }
        }
    }

    private void Start()
    {
        var audioMixerSnapshot = audioMixer.FindSnapshot("Snapshot - Copy");
        audioMixerSnapshot.TransitionTo(0.0001f);
        root.SetActive(false);
    }

    private void OnEnable()
    {
        OnEnableUpdateMixerGroupSettings(backgroundSettings);
        OnEnableUpdateMixerGroupSettings(soundEffectsSettings);
        buttonOpen.onClick.AddListener(OpenOnClick);
        buttonClose.onClick.AddListener(CloseOnClick);
        backgroundSettings.RegisterSliderListener(audioMixer);
        soundEffectsSettings.RegisterSliderListener(audioMixer);
    }

    private void OnEnableUpdateMixerGroupSettings(SoundChannelSettings soundChannel)
    {
        audioMixer.GetFloat(soundChannel.mixerGroupName, out var volume);
        SetText(soundChannel, volume);
        var value = Mathf.Exp(volume / 20);
        soundChannel.slider.value = value;
    }

    private void OnDisable()
    {
        buttonOpen.onClick.RemoveListener(OpenOnClick);
        buttonClose.onClick.RemoveListener(CloseOnClick);
        backgroundSettings.RemoveSliderListener();
        soundEffectsSettings.RemoveSliderListener();
    }

    private void OpenOnClick()
    {
        root.SetActive(true);
    }

    private void CloseOnClick()
    {
        root.SetActive(false);
    }

    private static void SetText(SoundChannelSettings soundChannel, float value)
    {
        soundChannel.textObject.text = $"{soundChannel.prefixText} {value:F0}";
    }
}