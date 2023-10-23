using VolgaIT.Model.Model;

namespace VolgaIT.Model.Entities
{
    public class RentEntity : BaseEntity
    {

        public long TransportId { get; set; }
        public virtual TransportEntity Transport { get; set; }

        public long UserId { get; set; }
        public virtual UserEntity User { get; set; }

        public string TimeStart { get; set; } = string.Empty; // в ISO 8601
        public string? TimeEnd { get; set; } = string.Empty; // в ISO 8601

        public double PriceOfUnit { get; set; }
        public string PriceType { get; set; } = string.Empty;
        public double? FinalPrice { get; set; }

    }
}
