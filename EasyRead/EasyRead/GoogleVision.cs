using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Image = Google.Cloud.Vision.V1.Image;

namespace EasyRead
{
    public class GoogleVision
    {
        private string apiKey;
        private string responseStr;
        private string result;

        public GoogleVision(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<string> RequestAnotate(string base64Img)
        {
            try
            {
                HttpResponseMessage response;
                using (var client = new HttpClient())
                {

                    string myJson = $@"{{
                    ""requests"":[
                        {{
                            ""image"":{{
                                ""content"": ""{base64Img}""
                            }},
                            ""features"":[
                                {{
                                    ""maxResults"": ""50"",   
                                    ""type"": ""{Settings.TEXT_DETECTION}""
                                }}
                            ]
                        }}
                    ]
                }}";

                    string requestUri = "https://vision.googleapis.com/v1/images:annotate?key=";
                    requestUri += apiKey;
                    response = await client.PostAsync(
                        requestUri,
                        new StringContent(myJson, Encoding.UTF8, "application/json"));
                    
                    responseStr = await response.Content.ReadAsStringAsync();
                    Settings.mSelection = Settings.TEXT_DETECTION;
                    result = ConvertResponseToResult(responseStr);
                    
                    return result;
                }
            }
            catch(Exception ex)
            {
                result = ex.Message;
                return null;
            }
        }

        private string ConvertResponseToResult(string responseStr)
        {
            switch(Settings.mSelection)
            {
                case "TEXT_DETECTION" :
                    JObject Tjson = JObject.Parse(responseStr);
                    var Tvalues = (JArray)Tjson["responses"];
                    foreach (var respons in Tvalues)
                    {
                        var textAnnotations = (JArray)respons["textAnnotations"];
                        foreach (var Annotations in textAnnotations)
                        {
                            if (Annotations.HasValues)
                            {
                                result = Annotations["description"].ToString();
                                break;
                            }
                        }
                    }
                    break;

                case "LOGO_DETECTION":
                    JObject Ljson = JObject.Parse(responseStr);
                    var Lvalues = (JArray)Ljson["responses"];
                    foreach (var respons in Lvalues)
                    {
                        var logoAnnotations = (JArray)respons["logoAnnotations"];
                        foreach (var Annotations in logoAnnotations)
                        {
                            if (Annotations.HasValues)
                            {
                                result = Annotations["description"].ToString();
                                break;
                            }
                        }
                    }
                    break;
                case "LANDMARK_DETECTION":
                    JObject Lajson = JObject.Parse(responseStr);
                    var Lavalues = (JArray)Lajson["responses"];
                    foreach (var respons in Lavalues)
                    {
                        var landmarkAnnotations = (JArray)respons["landmarkAnnotations"];
                        foreach (var Annotations in landmarkAnnotations)
                        {
                            if (Annotations.HasValues)
                            {
                                result = Annotations["description"].ToString();
                                break;
                            }
                        }
                    }
                    break;
                case "LABEL_DETECTION":
                    JObject Labjson = JObject.Parse(responseStr);
                    var Labvalues = (JArray)Labjson["responses"];
                    foreach (var respons in Labvalues)
                    {
                        var labelAnnotations = (JArray)respons["labelAnnotations"];
                        foreach (var Annotations in labelAnnotations)
                        {
                            if (Annotations.HasValues)
                            {
                                result += Annotations["description"].ToString() + Environment.NewLine;
                                
                            }
                        }
                    }
                    break;
                case "SAFE_SEARCH_DETECTION":
                    JObject Sjson = JObject.Parse(responseStr);
                    var Svalues = (JArray)Sjson["responses"];
                    foreach (var respons in Svalues)
                    {
                        result = respons["safeSearchAnnotation"].ToString();
                        break;
                    }
                    break;
            }
            return result;
        }
    }
}
