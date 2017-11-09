using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace beerbot.BeerData
{
    public class SearchBeerResponse
    {
        public int currentPage { get; set; }
        public int numberOfPages { get; set; }
        public int totalResults { get; set; }
        public List<BeerData> data { get; set; }
        public string status { get; set; }
    }
}