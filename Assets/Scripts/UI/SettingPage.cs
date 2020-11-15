using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

internal sealed class SettingPage : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Button _buttonOpen;
    [SerializeField] private Button _buttonClose;
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _text;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private string _name;

    private void Start()
    {
        var audioMixerSnapshot = _audioMixer.FindSnapshot("Snapshot - Copy");
        audioMixerSnapshot.TransitionTo(0.0001f);
        _root.SetActive(false);
    }

    private void OnEnable()
    {
        _audioMixer.GetFloat(_name, out var value);
        SetText(value);
        _slider.value = value;
        _buttonOpen.onClick.AddListener(OpenOnClick);
        _buttonClose.onClick.AddListener(CloseOnClick);
        _slider.onValueChanged.AddListener(SliderValueChanged);
    }

    private void OnDisable()
    {
        _buttonOpen.onClick.RemoveListener(OpenOnClick);
        _buttonClose.onClick.RemoveListener(CloseOnClick);
        _slider.onValueChanged.RemoveListener(SliderValueChanged);
    }

    private void SliderValueChanged(float value)
    {
        _audioMixer.SetFloat(_name, value);
        SetText(value);
    }

    private void OpenOnClick()
    {
        _root.SetActive(true);
    }

    private void CloseOnClick()
    {
        _root.SetActive(false);
    }

    private void SetText(float value)
    {
        _text.text = $"Фоновая музыка {value:F0}";
    }
}
