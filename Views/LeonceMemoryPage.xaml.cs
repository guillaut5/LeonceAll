﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LeonceAll.Leonce;
using LeonceAll.Models;
using Windows.ApplicationModel;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace LeonceAll.Views
{
    public sealed partial class LeonceMemoryPage : Page, INotifyPropertyChanged
    {


        private int gameCount = 0;
        private int gameTotal = 10;
        private int score = 0;
        private int imageWordsChoices = 3;
        private MediaElement player;


        private LImageMotReadMatching _imageSelected;
        public LImageMotReadMatching imageSelected
        {
            get { return _imageSelected; }
            set
            {
                _imageSelected = value;
                NotifyPropertyChanged("imageSelected");
            }
        }

        private readonly ObservableCollection<LImageMotReadMatching> wordCandidates = new ObservableCollection<LImageMotReadMatching>();


        private readonly ObservableCollection<LImagesMots> Images = new ObservableCollection<LImagesMots>();


        public String GameProgress
        {
            get
            {
                return "Essai n°" + gameCount;// + " / " + gameTotal;
            }
            set
            {
                NotifyPropertyChanged();
            }

        }

        public String ScoreActuel
        {
            get
            {
                return score + " points.";
            }

        }

        public LeonceMemoryPage()
        {

            player = new MediaElement();
            InitializeComponent();
        }


        private void prepareGame()
        {
            gameCount = 0;
            selectOneImage();
        }

        private void selectOneImage()
        {
            Images.Shuffle();
            LImagesMots tmpSeclected = Images[0];
            imageSelected = new LImageMotReadMatching(tmpSeclected.ImageSource, tmpSeclected.ImageText, true);
            wordCandidates.Clear();
            wordCandidates.Add(imageSelected);

            for (int i = 1; i < imageWordsChoices; i++)
            {
                tmpSeclected = Images[i];
                LImageMotReadMatching falseWordSelected = new LImageMotReadMatching(tmpSeclected.ImageSource, tmpSeclected.ImageText, false);
                wordCandidates.Add(falseWordSelected);
            }
            wordCandidates.Shuffle();



        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                   AppViewBackButtonVisibility.Collapsed;

            // Remove this when replaced with XAML bindings

            if (Images.Count == 0)
            {
                await GetItemsAsync();
            }


            prepareGame();
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

        public async static Task<LImagesMots> LoadImageInfo(StorageFile file)
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
                LImagesMots info = new LImagesMots(
                    properties, file, bitmapImage, file.Name,
                    file.DisplayName, file.DisplayType);

                return info;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            NotifyPropertyChanged(propertyName);
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private async void textBoxClicked(object sender, RoutedEventArgs e)
        {
            LImageMotReadMatching motChoisi = (LImageMotReadMatching)(sender as FrameworkElement).DataContext;
            await setMotChoisiSelected(motChoisi);
        }


        private async void textBoxClicked(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            LImageMotReadMatching motChoisi = (LImageMotReadMatching)(sender as FrameworkElement).DataContext;
            await setMotChoisiSelected(motChoisi);



        }


        private async Task setMotChoisiSelected(LImageMotReadMatching motChoisi)
        {
            gameCount++;

            motChoisi.setSelected();

            if (!motChoisi.isCorrect)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                ElementSoundPlayer.Play(ElementSoundKind.GoBack);
                NotifyPropertyChanged("GameProgress");
                await Task.Delay(1000);
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;


            }
            else
            {
                score++;
                NotifyPropertyChanged("ScoreActuel");
                NotifyPropertyChanged("GameProgress");

                await playApplauseAsync();
                selectOneImage();
            }
        }

        private async Task playApplauseAsync()
        {
            StorageFolder appInstalledFolder = Package.Current.InstalledLocation;
            StorageFolder folder = await appInstalledFolder.GetFolderAsync("Assets\\sounds");
            if (folder != null)
            {
                var file = await folder.GetFileAsync("applause10.mp3");
                if (file != null)
                {
                    var stream = await file.OpenReadAsync();
                    player.SetSource(stream, file.ContentType);
                    player.Volume = 100;
                    player.Play();
                    await Task.Delay(2000);
                }
            }
        }

        private void Button_Click_ReinitGame(object sender, RoutedEventArgs e)
        {
            selectOneImage();

        }

    }



}