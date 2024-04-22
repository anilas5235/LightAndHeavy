using Project.Scripts.Utilities;

namespace Project.UISettings.Scripts
{
    public class SaveHandlerSettings : SaveHandler<GameSettings,SaveHandlerSettings>
    {
        protected override string GetFilePath()
        {
            return "SettingsSave.save";
        }

        protected override GameSettings GenerateNewSave()
        {
            return new GameSettings()
            {
                masterVolume = .8f,
                musicVolume = .8f,
                effectVolume = .8f
            };
        }
    }

    public class GameSettings
    {
        public float masterVolume;
        public float musicVolume;
        public float effectVolume;
    }
}