using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


using Windows.Storage;
using Windows.Storage.Streams;

namespace LeonceAll.Leonce
{
    // Use these extension methods to store and retrieve local and roaming app data
    // More details regarding storing and retrieving app data at https://docs.microsoft.com/windows/uwp/app-settings/store-and-retrieve-app-data
    public class LAppSettings : INotifyPropertyChanged
    {
        public bool ShowClavierVocalSetting
        {
            get
            {
                return ReadSettings(nameof(ShowClavierVocalSetting), true);
            }
            set
            {
                SaveSettings(nameof(ShowClavierVocalSetting), value);
                NotifyPropertyChanged();
            }
        }

        public bool ShowConsonneForteSetting
        {
            get
            {
                return ReadSettings(nameof(ShowConsonneForteSetting), true);
            }
            set
            {
                SaveSettings(nameof(ShowConsonneForteSetting), value);
                NotifyPropertyChanged();
            }
        }





        public ApplicationDataContainer LocalSettings { get; set; }

        public LAppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }

        private T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            if (null != defaultValue)
            {
                return defaultValue;
            }
            return default(T);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
