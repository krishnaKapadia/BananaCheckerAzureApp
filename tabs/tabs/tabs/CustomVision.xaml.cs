using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using tabs.DataModles;
using Tabs.Model;
using Xamarin.Forms;

namespace tabs
{
    public partial class CustomVision : ContentPage
    {
        public CustomVision()
        {
            InitializeComponent();
        }

        private async void LoadCamera(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            image.Source = ImageSource.FromStream(() =>
            {
                return file.GetStream();
            });

            await MakePredictionRequest(file);

            file.Dispose();
        }

        static byte[] GetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task MakePredictionRequest(MediaFile file)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Prediction-Key", "7409d41bb51444dc9e914c092a8965e3");

            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/47ad4ca9-566d-4a09-a0d2-9d255b739d78/image";

            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    EvaluationModel responseModel = JsonConvert.DeserializeObject<EvaluationModel>(responseString);

                    double max = 0;
                    String color = "";
                    bool isBanana = false;

                    foreach (var c in responseModel.Predictions)
                    {
                        //Easier to read an int value than a double
                        max = responseModel.Predictions.Max(m => m.Probability);
                        TagLabel.Text = (max >= 0.5) ? "Banana" : "Not a Banana";
                       
                        //Extracting color based off tags and probability
                        if (c.Tag.Equals("green banana"))
                        {
                            color = "green";
                        }
                        else if (c.Tag.Equals("yellow banana"))
                        {
                            color = "yellow";
                        }

                        if(max >= 0.5) PredictionLabel.Text = c.Tag + " " + color;

                        if (max >= 0.5) isBanana = true;
                    }

                    //Creates a new bananaInformation object and send it to the database
                    bananaInformation banana = new bananaInformation(isBanana, color);
                    await AzureManager.AzureManagerInstance.createNewBanana(banana);

                }

                //Get rid of file once we have finished using it
                file.Dispose();
            }

        }
    }
}