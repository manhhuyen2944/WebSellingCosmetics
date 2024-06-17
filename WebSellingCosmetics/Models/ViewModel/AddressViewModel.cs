namespace WebSellingCosmetics.Models.ViewModel
{
    public class AddressViewModel
    {
        public int AddressId { get; set; }
        public string? Street { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public string? City { get; set; }
        public string? FormattedAddress { get { return $"{Street}, {Ward}, {District}, {City}"; } }
    }
}
