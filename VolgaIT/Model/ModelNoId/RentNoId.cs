namespace VolgaIT.Model.Model
{
    public class RentNoId
    {
        public long TransportId { get; set; }

        public long UserId { get; set; }

        public string TimeStart { get; set; } = string.Empty; // в ISO 8601
        public string? TimeEnd { get; set; } = string.Empty; // в ISO 8601

        public double PriceOfUnit { get; set; }
        public string PriceType { get; set; } = string.Empty;
        public double? FinalPrice { get; set; }
    }
}
