using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
/*
 *
 *Modele d'un image
 *
 *
 */

namespace LeonceAll.Models
{




    public class LImagesMots : INotifyPropertyChanged
    {
        public LImagesMots(ImageProperties properties, StorageFile imageFile,
            BitmapImage src, string imageName, string name, string type)
        {
            ImageSource = src;
            ImageName = name;
            ImageFile = imageFile;
            ImageText = imageName;
            if (imageName.Contains("_") == true)
            {
                string imageText = imageName.Split(new char[] { '_' }, 2)[1];
                imageText = imageText.Replace('_', ' ');
                imageText = imageText.Split(new char[] { '.' }, 2)[0];

                ImageText = imageText.ToUpper();
            }
        }
            



        public StorageFile ImageFile { get; }

        public string ImageText { get; set; }


        public string ImageName { get; set; }

        private BitmapImage _imageSource = null;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

      

        private bool _needsSaved = false;
        

        public bool NeedsSaved
        {
            get => _needsSaved;
            set => SetProperty(ref _needsSaved, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetEditingProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }
    }
}
