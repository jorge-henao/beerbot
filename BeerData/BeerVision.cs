using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace beerbot.BeerData
{
    public class BeerVision
    {

        public async Task<string> IdentifyBeer(byte[] picBytes)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", "880fe241a9814b65a93b15078943bc33");
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/a6461538-6159-4502-9b15-6ff74f9b4ece/image?iterationId=d463fadd-d3ee-4e20-9f9d-2858bb978b33";

            HttpResponseMessage response;
            using (var content = new ByteArrayContent(picBytes))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(url, content);
                var stringResponse = await response.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<PredictionResponse>(stringResponse);

                return TryGetBeerBrand(responseObject);
            }
        }

        private string TryGetBeerBrand(PredictionResponse predictionResponse)
        {
            const string brandKey = "brand : ", beerTagName = "beer";
            string result = "desconocida"; double topProbability = 0.05;

            if (predictionResponse == null)
                throw new ArgumentNullException(nameof(predictionResponse));

            if (predictionResponse.Predictions == null)
                throw new ArgumentNullException(nameof(predictionResponse.Predictions));

            var beerTag = predictionResponse.Predictions.Find(x => x.Tag == beerTagName);
            if (beerTag.Probability < 0.8)
                return null;

            foreach (var prediction in predictionResponse.Predictions)
            {
                if (prediction.Tag.Contains(brandKey) &&
                    prediction.Probability > topProbability)
                {
                    topProbability = prediction.Probability;
                    result = prediction.Tag.Replace(brandKey, string.Empty);
                }
            }

            return result;
        }

    }
}