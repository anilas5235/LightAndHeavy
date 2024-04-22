using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Scripts.Settings
{
    public class AudioSlider : MonoBehaviour
    {
        public event Action<PointerEventData> OnPointerUp;  
            
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string channelName;
        [SerializeField] private Image iconImage;

        [SerializeField] private Sprite activeIcon;
        [SerializeField] private Color activeColor = Color.white;
        [Space(5),SerializeField] private Sprite inactiveIcon;
        [SerializeField] private Color inactiveColor = Color.white;

        private Slider _mySlider;
        private EventTrigger _sliderEventTrigger;

        public float dBValue { get; private set; }
        
        public float SliderValue
        {
            get => _mySlider.value;
            set
            {
                if(!_mySlider) InitAudioSlider();
                _mySlider.value = value;
                SliderValueChanged(value);
            }
        }

        private void Awake()
        {
           InitAudioSlider();
        }

        private void InitAudioSlider()
        {
            _mySlider = GetComponentInChildren<Slider>();
            _mySlider.onValueChanged.AddListener(SliderValueChanged);
            _sliderEventTrigger = _mySlider.gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.EndDrag,
            };
            entry.callback.AddListener((data) => { OnPointerUp?.Invoke((PointerEventData)data); });
            _sliderEventTrigger.triggers.Add(entry);
        }

        private void SliderValueChanged(float arg0)
        {
            dBValue = ConvertSliderValueTodB(arg0);
            UpdateAudioSlider();
        }

        private void UpdateAudioSlider()
        {
            if (iconImage)
            {
                if(activeIcon && inactiveIcon) iconImage.sprite = _mySlider.value > 0 ? activeIcon : inactiveIcon;
                iconImage.color = _mySlider.value > 0 ? activeColor : inactiveColor;
            }
            mixer.SetFloat(channelName, dBValue);
        }
        
        private static float ConvertSliderValueTodB(float sliderValue)
        {
            return Mathf.Clamp(Mathf.Log10(sliderValue) * 20f,-80f,0f);
        }

        private static float ConvertDBToSliderValue(float dBValue)
        {
            return Mathf.Clamp(Mathf.Pow(10, (dBValue) / 20f),0f,1f);
        }
    }
}
