using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace beerbot.BeerData
{
    public class BreweryDB
    {
        const string apiKey = "042759e69ee974492fbd13a749da7418";

        public async Task<SearchBeerResponse> SearchBeer(string beerName)
        {
            string url = $"http://api.brewerydb.com/v2/search?q={beerName}&type=beer&key={apiKey}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseContentString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<SearchBeerResponse>(responseContentString);
            return responseObject;
        }

        public async Task<GetBeerResponse> GetBeer(string beerId)
        {
            string url = $"http://api.brewerydb.com/v2/beer/{beerId}?key={apiKey}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseContentString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GetBeerResponse>(responseContentString);
            return responseObject;
        }
    }
}