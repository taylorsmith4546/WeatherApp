using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Core.Domain;

namespace WeatherApp.Core.Services
{
    public class WeatherService
    {
        private static string apikey = "bb20f776a3e42b44";
        private static string locUrl = "";
        public static string message = "";

        public static string GetMessage()
        {
            return message;
        }

        public static ConditionsResult GetWeatherFor(string query)
        {
            string[] queries = query.Split(',');
            if (queries.Count() > 1)
            {
                locUrl = "/" + queries[1] + "/" + queries[0];
            }
            else
            {
                locUrl = "/" + queries[0];
            }

            using (WebClient wc = new WebClient())

            {
                string json = wc.DownloadString($"http://api.wunderground.com/api/{apikey}/conditions/q{locUrl}.json");
                var o = JObject.Parse(json);

                if (o["current_observation"] != null)

                {

                    var result = JsonConvert.DeserializeObject<ConditionsResult>(json);
                    if (!File.Exists(result.current_observation.icon + ".gif"))
                    {
                        using (var wcc = new WebClient())
                        {
                            byte[] bytes = wcc.DownloadData(result.current_observation.icon_url);
                            File.WriteAllBytes(result.current_observation.icon + ".gif", bytes);
                        }
                    }

                    return result;
                }
                else if (o["response"]["error"] != null)
                {
                    message = "There are no cities that match your search criteria.";
                    return null;
                }
                else
                {
                    message = "There are multiple cities that match your search criteria, please be more specific.";
                    return null;
                }
            }
        }
    }
}
