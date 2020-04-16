using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media.Imaging;
/*
 *
 *Modele d'un image
 *
 *
 */

namespace LeonceAll.Models
{


    public class LImageMotReadMatching : INotifyPropertyChanged
    {

        public BitmapImage imgSrc;
        public string word = "";
        public Boolean isCorrect = false;
        public Windows.UI.Xaml.Visibility textVisibility = Windows.UI.Xaml.Visibility.Visible;
        public Windows.UI.Xaml.Visibility imageVisibility = Windows.UI.Xaml.Visibility.Collapsed;
        private Uri correctImg = new Uri("ms-appx:Assets/correct.png", UriKind.RelativeOrAbsolute);
        private Uri incorrectImg = new Uri("ms-appx:Assets/incorrect.png", UriKind.RelativeOrAbsolute);
        public BitmapImage image = null;

        public LImageMotReadMatching(BitmapImage img_p, string word_p, Boolean isCorrect_p)
        {
            imgSrc = img_p;
            word = word_p;
            isCorrect = isCorrect_p;
            image = new BitmapImage();
            if (isCorrect)
                image.UriSource = correctImg;
            else
                image.UriSource = incorrectImg;

        }

        public void setSelected()
        {
            textVisibility = Windows.UI.Xaml.Visibility.Collapsed;
            imageVisibility = Windows.UI.Xaml.Visibility.Visible;
            OnPropertyChanged("textVisibility");
            OnPropertyChanged("imageVisibility");

        }

        public void setUnSelected()
        {
            textVisibility = Windows.UI.Xaml.Visibility.Visible;
            imageVisibility = Windows.UI.Xaml.Visibility.Collapsed;
            OnPropertyChanged("textVisibility");
            OnPropertyChanged("imageVisibility");

        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }


}
