using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace IPInfoProviderLibrary
{
    public class IPInfoProvider : IIPInfoProvider
    {
        // store my personal APIKEY in order to use it on the requests -- PROBABLY NOT THE BEST WAY TO IMPLEMENT NEEDS CHAGE LATER??
        private readonly string ApiKey = "ebb1edbfca362294a4a79f64fa5a72c7";

        public IPDetails GetDetails(string ip)
        {
            string url = "http://api.ipstack.com/" + ip + $"?access_key={ApiKey}";
            var request = WebRequest.Create(url);
            try
            {
                using WebResponse wr = request.GetResponse();
                using Stream stream = wr.GetResponseStream();
                using StreamReader reader = new StreamReader(stream);
                string json = reader.ReadToEnd();
                var jobj = JObject.Parse(json);

                // ensure that the conversion will return actuall numbers instead of null. This is made because the Project specifically mentioned that I have to use the interface as is.
                // A better way I think was to just have lat and lon as double? in the interface
                double lat = 0.0;
                Double.TryParse(jobj["latitude"].ToString(), out lat);
                
                double lon = 0.0;
                Double.TryParse(jobj["longitude"].ToString(), out lon);

                IP ipd = new IP()
                {
                    City = (string)jobj["city"],
                    Country = (string)jobj["country_name"],
                    Continent = (string)jobj["continent_name"],
                    Latitude = lat,
                    Longitude = lon
                };

                return ipd;
            }
            catch (Exception ex)
            {
                throw new IPServiceNotAvailableException(ex.Message);
            }
        }
    }
}
