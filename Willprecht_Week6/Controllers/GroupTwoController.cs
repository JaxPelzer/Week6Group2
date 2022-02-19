using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using Newtonsoft.Json;
using Newtonsoft;
using ServiceStack;
using ServiceStack.Text;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Week6_Group2.Controllers
{
    [Route("api/Group2")]
    [ApiController]
    public class GroupTwoController : ControllerBase
    {

        [Route("GetBrewries")]
        [HttpGet]
        public ActionResult GetBrewries(string Zip_Code)
        {
            HttpClient client = new HttpClient();
            dynamic? obj = new ExpandoObject();
            string result;
            int brewpub = 0;
            int micro = 0;
            int large = 0;

            try
            {
                HttpResponseMessage response = client.GetAsync("https://api.openbrewerydb.org/breweries?by_postal="+ Zip_Code).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var list = JsonConvert.DeserializeObject<List<Breweries>>(result);

            var brewriesList = new List<string>();
            var typeList = new List<string>();

            foreach(var brew in list)
            {
                brewriesList.Add(brew.Name);
                typeList.Add(brew.Brewery_type);
            }

            foreach(var type in typeList)
            {
                if (type == "brewpub")
                {
                    brewpub++;
                }
                else if (type == "micro")
                {
                    micro++;
                } else
                {
                    large++;
                }
            }

            int numberOfBrewries = brewriesList.Count;

            var json = new JsonObject();

            json.Add("Number of Brewries In Area", numberOfBrewries.ToString());
            json.Add("Number of Pub Brewries", brewpub.ToString());
            json.Add("Number of Micro Brewries", micro.ToString());
            json.Add("Number of Large Brewries", large.ToString());

            return Ok(json);

        }
        

        [Route("GetMarkets")]
        [HttpGet]
        public ActionResult<List<RootObject>> GetMarkets()
        {
            HttpClient client = new HttpClient();
            dynamic? obj = new ExpandoObject();
            string result;
            double pricer;
            try
            {
                HttpResponseMessage response = client.GetAsync("https://www.cryptingup.com/api/markets").Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;

                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var list = JsonConvert.DeserializeObject<RootObject>(result);
            var coinList = new List<object>();
            coinList.Add(list);

            foreach (var item in list.Markets)
            {
                pricer = Convert.ToDouble(item.Price);
                item.Price = pricer.ToString("0.00");
            }



            return Ok(coinList);

        }
    }

    public class Breweries
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Brewery_type { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Postal_code { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Website_url { get; set; }
    }

    public class RootObject
    {
        public List<Markets>? Markets { get; set; }
    }

    public class Markets
    {
        public string? Exchange_id { get; set; }
        public string? Symbol { get; set; }
        public string? Base_asset { get; set; }
        public string? Price { get; set; }
        public string? Status { get; set; }
        public string? Updated_at { get; set; }
    }


}