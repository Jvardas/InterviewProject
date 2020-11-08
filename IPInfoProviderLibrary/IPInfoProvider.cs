using Newtonsoft.Json.Linq;
using System;
using System.Linq;
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
                Double.TryParse(jobj["latitude"].ToString(), out double lat);
                Double.TryParse(jobj["longitude"].ToString(), out double lon);

                IP ipd = new IP()
                {
                    City = (string)jobj["city"],
                    Country = (string)jobj["country_name"],
                    Continent = (string)jobj["continent_name"],
                    Latitude = lat,
                    Longitude = lon
                };

                // if all the details that I am getting back from IPStack are null then I am not interested of getting this entry in my DB
                // that's why I am throwing an exception here
                if (ipd.GetType().GetProperties().All(x => x.GetValue(ipd) is null && x.Name != "Latitude" && x.Name != "Longitude"))
                {
                    throw new IPServiceNotAvailableException("IP is not found");
                }

                return ipd;
            }
            catch (Exception ex)
            {
                throw new IPServiceNotAvailableException(ex.Message);
            }
        }
    }
}
