namespace IPInfoWebAPI.DataTransferObjects
{
    public class IpDetailDTO
    {
        public string Ip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
