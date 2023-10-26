namespace VolgaIT.Model.ModelUniqueDataTransfers
{
    public class UnicTransport
    {
        public bool CanBeRented { get; set; }
        public string TransportType { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double MinutePrice { get; set; } = 0;
        public double DayPrice { get; set; } = 0;
    }
}
