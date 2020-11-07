namespace IPInfoProviderLibrary
{
    interface IIPInfoProvider
    {
        IPDetails GetDetails(string ip);
    }

    public interface IPDetails
    {
        string City { get; set; }
        string Country { get; set; }
        string Continent { get; set; }
        string Latitude { get; set; }
        string Longtitude { get; set; }
    }
}
