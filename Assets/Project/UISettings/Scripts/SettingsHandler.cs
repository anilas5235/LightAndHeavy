using Project.Scripts.Utilities;
using Project.UISettings.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Scripts.Settings
{
    [DefaultExecutionOrder(1)]
    public class SettingsHandler : MajorUIComponent
    {
        [Header("AudioSliders"),SerializeField] private AudioSlider master;
        [SerializeField] private AudioSlider music;
        [SerializeField] private AudioSlider effects;
        
        private SaveHandlerSettings _saveHandler;
        private GameSettings _currentSettings;

        protected override void Awake()
        {
            base.Awake();
            _saveHandler = SaveHandlerSettings.Instance;
        }

        private void OnEnable()
        {
            _currentSettings = _saveHandler.CurrentSave;
            master.OnPointerUp += AnyAudioSliderChanged;
            music.OnPointerUp += AnyAudioSliderChanged;
            effects.OnPointerUp += AnyAudioSliderChanged; 
        }

        private void AnyAudioSliderChanged(PointerEventData obj)
        {
            SaveSliderValues();
        }

        private void OnDisable()
        {
            master.OnPointerUp -= AnyAudioSliderChanged;
            music.OnPointerUp -= AnyAudioSliderChanged;
            effects.OnPointerUp -= AnyAudioSliderChanged;
        }

        private void Start()
        {
             SetSliders();
        }

        public override void Activate()
        {
            base.Activate();
           SetSliders();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            SaveSliderValues();
        }
        private void SetSliders()
        {
            master.SliderValue = _currentSettings.masterVolume;
            music.SliderValue = _currentSettings.musicVolume;
            effects.SliderValue = _currentSettings.effectVolume;
        }

        private void SaveSliderValues()
        {
            _currentSettings.masterVolume = master.SliderValue;
            _currentSettings.musicVolume = music.SliderValue;
            _currentSettings.effectVolume = effects.SliderValue;
            _saveHandler.CurrentSave = _currentSettings;
            _saveHandler.SaveDataToFile();
        }
    }
}
