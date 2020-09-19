using Android.Content.Res;
using Plugin.Media;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Image = Xamarin.Forms.Image;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Azure.Search.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static EasyRead.MicrosoftVisionJsonResult;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Nancy.Json;
using Google.Cloud.Translate.V3;
using Google.Api.Gax.ResourceNames;
using GoogleTranslateNet;

namespace EasyRead
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppPage : ContentPage
    {
        Image image = new Image();
        static string subscriptionKey = Settings.SubscriptionKey;
        static string endpoint = "https://easyread.cognitiveservices.azure.com/";
        static string uriBase = endpoint + "vision/v2.1/ocr";
        static string Gkey = Settings.GKey;
        public AppPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            App.Current.MainPage = new MenuSelection();
            return true;
        }

        private async void pickPhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Large
                });
                if (file == null)
                    return;
                //MakeOCRRequest(file.Path);
                var imagefilestream = ImagetoBasestr(file.Path);
                GoogleVision googleVision = new GoogleVision(Settings.GKey);
                var result = await googleVision.RequestAnotate(imagefilestream);
                imgSelected.Source = Xamarin.Forms.ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();  
                    return stream;
                });
                ShowResult(result);
                file.Dispose();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void ShowResult(string result)
        {
            if (Settings.mSelection == Settings.TEXT_DETECTION)
            {
                EnableLabels();
                if (result != null && result != "" && !result.Contains("(null)"))
                {
                    this.lblResult.Text = result;
                    TranslateWords(result);
                }
                else
                {
                    lblLang.IsVisible = false;
                    lblTranslated.IsVisible = false;
                    this.lblResult.Text = "Unable to Identify Texts";
                }

            }
            else if (Settings.mSelection == Settings.LOGO_DETECTION)
            {
                if (this.lblResult.Text != "")
                {
                    this.lblResult.Text = "";
                }
                this.lblImageText.Text = "Logo Name: ";
                this.lblImageText.IsVisible = true;
                if (result != null && result != "" && !result.Contains("(null)"))
                {
                    this.lblResult.Text = result;
                }
                else
                {
                    this.lblResult.Text = "Unable to Identify Logo";
                }
            }
            else if (Settings.mSelection == Settings.LANDMARK_DETECTION)
            {
                if (this.lblResult.Text != "")
                {
                    this.lblResult.Text = "";
                }
                this.lblImageText.Text = "Place Name: ";
                this.lblImageText.IsVisible = true;
                if (result != null && result != "" && !result.Contains("(null)"))
                {
                    this.lblResult.Text = result;
                }
                else
                {
                    this.lblResult.Text = "Unable to Identify Landmark";
                }
            }
            else if (Settings.mSelection == Settings.LABEL_DETECTION)
            {
                if (this.lblResult.Text != "")
                {
                    this.lblResult.Text = "";
                }
                this.lblImageText.Text = "Picture Details: ";
                this.lblImageText.IsVisible = true;
                if (result != null && result != "" && !result.Contains("(null)"))
                {
                    this.lblResult.Text = result;
                }
                else
                {
                    this.lblResult.Text = "Unable to Identify Details";
                }
            }
            else if (Settings.mSelection == Settings.SAFE_SEARCH_DETECTION)
            {
                if (this.lblResult.Text != "")
                {
                    this.lblResult.Text = "";
                }
                this.lblImageText.Text = "Picture Details: ";
                this.lblImageText.IsVisible = true;
                if (result != null && result != "" && !result.Contains("(null)"))
                {
                    this.lblResult.Text = result;
                }
                else
                {
                    this.lblResult.Text = "Unable to Identify Details";
                }
            }
        }

        private string ImagetoBasestr(string Path)
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(Path);
            return Convert.ToBase64String(imageArray);
        }

        private async void takePhoto_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", "No camera available.", "OK");
                    return;
                }
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "xamarin.jpg",
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Large,
                    DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear
                });
                if (file == null)
                    return;
                //MakeOCRRequest(file.Path);
                var imagefilestream = ImagetoBasestr(file.Path);
                GoogleVision googleVision = new GoogleVision(Settings.GKey);
                var result = await googleVision.RequestAnotate(imagefilestream);
                imgSelected.Source = Xamarin.Forms.ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
                ShowResult(result);
                file.Dispose();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task MakeOCRRequest(string imageFilePath)
        {
            try
            {
                string textresponse = "";
                lblImageText.IsVisible = true;
                lblLang.IsVisible = true;
                lblTranslated.IsVisible = true;

                if (lblResult.Text != "")
                {
                    lblResult.Text = "";
                    TranslatedTextLabel.Text = "";
                    DetectedLanguageLabel.Text = "";
                }
                HttpClient client = new HttpClient();

                // Request headers.
                client.DefaultRequestHeaders.Add(
                    "Ocp-Apim-Subscription-Key", subscriptionKey);

                // Request parameters. 
                // The language parameter doesn't specify a language, so the 
                // method detects it automatically.
                // The detectOrientation parameter is set to true, so the method detects and
                // and corrects text orientation before detecting text.
                string requestParameters = "language=unk&detectOrientation=true";

                // Assemble the URI for the REST API method.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Read the contents of the specified local image
                // into a byte array.
                byte[] byteData = GetImageAsByteArray(imageFilePath);

                // Add the byte array as an octet stream to the request body.
                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // This example uses the "application/octet-stream" content type.
                    // The other content types you can use are "application/json"
                    // and "multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Asynchronously call the REST API method.
                    response = await client.PostAsync(uri, content);
                }

                // Asynchronously get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(contentString);

                if (myDeserializedClass != null && myDeserializedClass.regions.Count > 0)
                {
                    if (myDeserializedClass.regions[0] != null && myDeserializedClass.regions[0].lines.Count > 0)
                    {
                        foreach (var text in myDeserializedClass.regions[0].lines)
                        {
                            var result = text.words;
                            if (result.Count > 0)
                            {
                                foreach (var word in result)
                                {

                                    lblResult.Text += word.text + "\t";

                                }
                            }
                        }
                        textresponse = lblResult.Text;
                        TranslateWords(textresponse);
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Error", e.Message, "OK");
            }
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            // Open a read-only file stream for the specified file.
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                // Read the file's contents into a byte array.
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        private void EnableLabels()
        {
            lblImageText.IsVisible = true;
            lblLang.IsVisible = true;
            lblTranslated.IsVisible = true;

            if (lblResult.Text != "")
            {
                lblResult.Text = "";
                TranslatedTextLabel.Text = "";
                DetectedLanguageLabel.Text = "";
            }
        }

        private void TranslateWords(string text)
        {

            GoogleTranslate google = new GoogleTranslate(Gkey);
            google.PrettyPrint = true;
            if(text.Length > 2000)
            {
                google.LargeQuery = true;
            }
            var language = google.DetectLanguage(text);
            if (language.Count >= 1)
            {
                var lang = language[0].Language;
                if (lang == "en")
                {
                    DetectedLanguageLabel.Text = "English";
                    TranslatedTextLabel.Text = text;
                }
                else
                {

                    DetectedLanguageLabel.Text = LanguagesName.FindLanguageName(lang);

                }
                var result = google.Translate(Language.Automatic, Language.English, text);
                if (result.Count > 0)
                {
                    TranslatedTextLabel.Text = result[0].TranslatedText;
                }
            }
        }
    }
}