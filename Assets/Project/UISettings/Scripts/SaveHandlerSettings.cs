using Project.Scripts.Utilities;

namespace Project.UISettings.Scripts
{
    public class SaveHandlerSettings : SaveHandler<GameSettings,SaveHandlerSettings>
    {
        protected override string GetFilePath()
        {
            return "SettingsSave.save";
        }
    }

    public class GameSettings
    {
        public float masterVolume = 0.8f;
        public float musicVolume = 0.8f;
        public float effectVolume = 0.8f;
    }
}