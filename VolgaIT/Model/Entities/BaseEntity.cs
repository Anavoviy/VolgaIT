using VolgaIT.Model.Interfaces;

namespace VolgaIT.Model.Entities
{
    public class BaseEntity : IEntity
    {
        public long Id { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
