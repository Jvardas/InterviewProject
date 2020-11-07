namespace IPInfoProviderLibrary
{
    public interface IIPInfoProvider
    {
        IPDetails GetDetails(string ip);
    }

    public interface IPDetails
    {
        string City { get; set; }
        string Country { get; set; }
        string Continent { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}
