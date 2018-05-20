using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace beerbot.BeerData.Untappd
{
    public class BeerSearcher
    {
        const string clientId = "259781D3BE95A096199756A0603552304563749C";
        const string clientSecret = "DD000112673954ADB3DABDB651DBD785DBB164A7";

        public async Task<SearchBeerResponse> SearchBeer(string beerName)
        {
            string url = $"https://api.untappd.com/v4/search/beer?client_id={clientId}&client_secret={clientSecret}&q={beerName}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseContentString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Untappd.SearchBeerResponse>(responseContentString);
            return responseObject;
        }

        public async Task<GetBeerResponse> GetBeer(string beerId)
        {
            string url = $"http://api.brewerydb.com/v2/beer/{beerId}?key={clientSecret}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseContentString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GetBeerResponse>(responseContentString);
            return responseObject;
        }
    }
}