namespace VolgaIT.Model.Entities
{
    public class RentEntity
    {

        public long TransportId { get; set; }
        public long UserId { get; set; }

        public string TimeStart { get; set; } // в ISO 8601
        public string? TimeEnd { get; set; } // в ISO 8601

        public double PriceOfUnit { get; set; }
        public string PriceType { get; set; }
        public double? FinalPrice { get; set; }

    }
}
