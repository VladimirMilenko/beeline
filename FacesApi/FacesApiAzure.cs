using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FacesApi
{
    public class ApiWorker : IFacesApiClient
    {
        private string API_KEY;
        public ApiWorker(string apiKey)
        {
            API_KEY = apiKey;
        }
        async public Task<OperationResult> CreatePerson(string name, string groupId)
        {
            var opResult = new OperationResult();
            opResult.data = new ExpandoObject();
            opResult.result = false;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                    .Add("Ocp-Apim-Subscription-Key", API_KEY);
                var content = new StringContent(JsonConvert.SerializeObject(new { name = name }));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync("https://westus.api.cognitive.microsoft.com/face/v1.0/persongroups/" + groupId + "/persons", content);
                if (response.IsSuccessStatusCode)
                {
                    var respObject = JsonConvert.DeserializeObject<CreatePersonResponse>(await response.Content.ReadAsStringAsync());
                    opResult.result = true;
                    opResult.data.personId = respObject.personId;
                    return opResult;
                }
                else
                {
                    opResult.data.error = "API Error";

                    return opResult;
                }
            }
        }
        async public Task<OperationResult> AddPersonFace(string groupId, string personId, byte[] imageBytes)
        {
            var opResult = new OperationResult();
            opResult.data = new ExpandoObject();
            opResult.result = false;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                    .Add("Ocp-Apim-Subscription-Key", API_KEY);
                var content = new ByteArrayContent(imageBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync("https://westus.api.cognitive.microsoft.com/face/v1.0/persongroups/" + groupId + "/persons/" + personId + "/persistedFaces", content);
                if (response.IsSuccessStatusCode)
                {
                    var respObject = JsonConvert.DeserializeObject<AddFaceResponse>(await response.Content.ReadAsStringAsync());
                    opResult.result = true;
                    opResult.data.persistedFaceId = respObject.persistedFaceId;
                    return opResult;
                }
                else
                {
                    opResult.data.error = "API Error";

                    return opResult;
                }
            }
        }
        async public Task<OperationResult> Identify(string groupId, string faceId)
        {
            var opResult = new OperationResult();
            opResult.data = new ExpandoObject();
            opResult.result = false;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                    .Add("Ocp-Apim-Subscription-Key", API_KEY);
                var content = new StringContent(JsonConvert.SerializeObject(new { personGroupId = groupId, faceIds = new string[] { faceId } }));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync("https://westus.api.cognitive.microsoft.com/face/v1.0/identify", content);
                var respString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(respString);
                if (response.IsSuccessStatusCode)
                {
                    var respObject = JsonConvert.DeserializeObject<FaceIdentificationResponse[]>(respString);
                    if (respObject.Length > 0 && respObject[0].candidates.Length > 0)
                    {
                        var candidate = respObject[0].candidates[0];
                        if (candidate.confidence > 0.6)
                        {
                            opResult.result = true;
                            opResult.data.personId = candidate.personId;
                            return opResult;
                        }
                    }
                    opResult.data.error = "No account faces found";
                    return opResult;
                }
                else
                {
                    opResult.data.error = "API Error";
                    return opResult;
                }
            }
        }
        async public Task<OperationResult> DetectFace(byte[] imageBytes)
        {
            var opResult = new OperationResult();
            opResult.data = new ExpandoObject();
            opResult.result = false;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                    .Add("Ocp-Apim-Subscription-Key", API_KEY);
                var content = new ByteArrayContent(imageBytes);
                File.WriteAllBytes("test2.png", imageBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync("https://westus.api.cognitive.microsoft.com/face/v1.0/detect", content);
                if (response.IsSuccessStatusCode)
                {
                    var respObject = JsonConvert.DeserializeObject<FaceDetectionResponse[]>(await response.Content.ReadAsStringAsync());
                    if (respObject.Length > 0)
                    {
                        opResult.result = true;
                        opResult.data.faceId = respObject[0].faceId;
                        opResult.data.faceRectangle = respObject[0].faceRectangle;
                        return opResult;
                    }
                    opResult.data.error = "No account faces found";
                    return opResult;

                }
                else
                {
                    opResult.data.error = "API Error";

                    return opResult;
                }
            }
        }

        async public Task<OperationResult> TrainGroup(string groupId)
        {
            var opResult = new OperationResult();
            opResult.data = new ExpandoObject();
            opResult.result = false;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders
                    .Add("Ocp-Apim-Subscription-Key", API_KEY);
                var response = await client.PostAsync("https://westus.api.cognitive.microsoft.com/face/v1.0/persongroups/" + groupId + "/train", new StringContent(""));
                if (response.IsSuccessStatusCode)
                {
                    opResult.result = true;

                    return opResult;

                }
                else
                {
                    opResult.result = false;
                    opResult.data.error = "API Error";
                    return opResult;

                }
            }
        }
    }
    public class OperationResult
    {
        public bool result;
        public dynamic data;
    }
    public class CreatePersonResponse
    {
        public string personId;
    }
    public class AddFaceResponse
    {
        public string persistedFaceId;
    }
    public class FaceIdentificationResponse
    {
        public string faceId;
        public IdentificationReponse[] candidates;
    }
    public class IdentificationReponse
    {
        public string personId;
        public double confidence;
    }
    public class FaceDetectionResponse
    {
        public string faceId;
        public FaceRectangle faceRectangle;
    }
    public class FaceRectangle
    {
        public string width;
        public string height;
        public string left;
        public string top;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
