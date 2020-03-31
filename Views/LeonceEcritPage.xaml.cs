using F23.StringSimilarity;
using LeonceAll.Leonce;
using LeonceAll.Models;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace LeonceAll.Views
{
    public sealed partial class LeonceEcritPage : Page, INotifyPropertyChanged
    {

        public ObservableCollection<ImageFileInfo> Images => images;
        private readonly ObservableCollection<ImageFileInfo> images = new ObservableCollection<ImageFileInfo>();
        private ILogger log;
        private LAppSettings localSetting_m;
        private LPlayLetter letterPlayer_m;
        public event PropertyChangedEventHandler PropertyChanged;

        public LeonceEcritPage()
        {
            this.InitializeComponent();

            log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
            localSetting_m = new LAppSettings();
            letterPlayer_m = new LPlayLetter();
            log.Trace("This is a trace message.");
        }


        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                   AppViewBackButtonVisibility.Collapsed;

            // Remove this when replaced with XAML bindings
            flipView.ItemsSource = Images;

            if (Images.Count == 0)
            {
                await GetItemsAsync();
            }
            base.OnNavigatedTo(e);

        }


        private async Task GetItemsAsync()
        {
            // https://docs.microsoft.com/uwp/api/windows.ui.xaml.controls.image#Windows_UI_Xaml_Controls_Image_Source
            // See "Using a stream source to show images from the Pictures library".
            // This code is modified to get images from the app folder.

            // Get the app folder where the images are stored.
            StorageFolder appInstalledFolder = Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets\\img");

            // Get and process files in folder
            IReadOnlyList<StorageFile> fileList = await assets.GetFilesAsync();
            foreach (StorageFile file in fileList)
            {
                // Limit to only png or jpg files.
                if (file.ContentType == "image/png" || file.ContentType == "image/jpeg")
                {
                    Images.Add(await LoadImageInfo(file));
                }
            }
            Images.Shuffle();
        }

        public async static Task<ImageFileInfo> LoadImageInfo(StorageFile file)
        {
            // Open a stream for the selected file.
            // The 'using' block ensures the stream is disposed
            // after the image is loaded.
            using (IRandomAccessStream fileStream = await file.OpenReadAsync())
            {
                // Create a bitmap to be the image source.
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(fileStream);

                var properties = await file.Properties.GetImagePropertiesAsync();
                ImageFileInfo info = new ImageFileInfo(
                    properties, file, bitmapImage, file.Name,
                    file.DisplayName, file.DisplayType);

                return info;
            }
        }


        private async void ButtonValid_Click(object sender, RoutedEventArgs e)
        {
            string inputString = myInputTextBox.Text;
            ImageFileInfo selectedImage = Images[flipView.SelectedIndex];

            string[] lines = inputString.Split('\r');
            string voiceString = inputString;
            double ratingResult = 0.0;
            foreach (var L in lines)
            {
                var l = new NormalizedLevenshtein();
                double distance = l.Similarity(L, selectedImage.ImageText);
                // distance est entre 0 [PERFECT] et 1 
                if (distance > ratingResult)
                    ratingResult = distance;

                log.Trace("Compare " + L + ", " + selectedImage.ImageText);
                log.Trace("results =" + l.Similarity(L, selectedImage.ImageText));
                if (L == selectedImage.ImageText)
                {
                    voiceString = L;
                }
            }

            // transformation du result 0..1 en 0..5 (5 est 5 etoile)
            log.Trace("ratingResults =" + ratingResult + "  ---");
            log.Trace("5*ratingResult =" + 5 * (ratingResult) + "  ---");

            ratingResult = 5 * ratingResult;
            int ratingResultInt = (int)ratingResult;
            log.Trace(" (int)ratingResult =" + ratingResult + "  ---");

            string bravoText = "";
            switch (ratingResultInt)
            {
                case 5:
                    bravoText = " BRAVO !!, ";
                    break;
                case 4:
                    bravoText = " PRESQUE !!";
                    break;
                case 2:
                case 3:
                    bravoText = " ESSAYE ENCORE !!";
                    break;
                case 0:
                case 1:
                    bravoText = " RECOMMENCE !!";
                    myInputTextBox.Text = "";
                    break;
                default:
                    bravoText = " DEFAULT !!";
                    break;

            }
            Flyout tmpFlyout = Resources["MyFlyout"] as Flyout;
            MyRating.Value = ratingResultInt;
            MyResultText.Text = bravoText;
            voiceString = string.Concat(bravoText + ",", voiceString);
            myInputTextBox.Focus(FocusState.Programmatic);
            myInputTextBox.IsTextScaleFactorEnabled = false;

            await letterPlayer_m.speachAsync(voiceString);
            tmpFlyout.ShowAt(myInputTextBox);
            // rend le focus a la text box
            myInputTextBox.Focus(FocusState.Programmatic);

        }

        private async void MyTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            String key = e.Key.ToString().ToUpper();
            await letterPlayer_m.PlayLetter(key);

        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            myInputTextBox.Focus(FocusState.Programmatic);
        }




        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            myInputTextBox.Text = "";
            myInputTextBox.Focus(FocusState.Programmatic);
        }

        private void FlipView_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            myInputTextBox.Text = "";
            myInputTextBox.Focus(FocusState.Programmatic);
        }
    }

    public static class ShuffleExtension
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }


}
