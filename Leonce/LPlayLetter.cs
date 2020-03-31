using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LeonceAll.Leonce
{
    class LPlayLetter
    {
        private LAppSettings localSetting_m;
        private ILogger log;


        public LPlayLetter()
        {

            log = MetroLog.LogManagerFactory.DefaultLogManager.GetLogger<LPlayLetter>();
            localSetting_m = new LAppSettings();
        }

        public async Task PlayLetter(string key_p)
        {
            String key = key_p.ToString().ToUpper();


            switch (key)
            {
                case "A":
                case "E":
                case "I":
                case "J":
                case "O":
                case "Q":
                case "U":
                case "W":
                case "X":
                case "Y":
                    break;
                case "B":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "BEUH";
                    break;
                case "C":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "SE";
                    break;
                case "D":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "DE";
                    break;
                case "F":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "FEU";
                    break;
                case "G":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "GUEUX";
                    break;
                case "H":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "H";
                    break;
                case "K":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "QUEUE";
                    break;
                case "L":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "LE";
                    break;
                case "M":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "ME";
                    break;
                case "N":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "NE";
                    break;
                case "P":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "PEUX";
                    break;
                case "R":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "RE";
                    break;
                case "S":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "SE";
                    break;
                case "T":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "TE";
                    break;
                case "V":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "VOEUX";
                    break;
                case "Z":
                    if (localSetting_m.ShowConsonneForteSetting)
                        key = "PEPPA PIG";
                    break;
                default:
                    key = "";
                    break;
            }


            //     if (PlayLetterButton.IsOn)
            if (localSetting_m.ShowClavierVocalSetting)
                await this.speachAsync(key);
        }


        public async Task speachAsync(string texte)
        {
            MediaElement mediaElement = new MediaElement();
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            Windows.Media.SpeechSynthesis.SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(texte);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();

        }
    }
}
