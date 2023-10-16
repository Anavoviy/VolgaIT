using System.ComponentModel.DataAnnotations;
using VolgaIT.Model.Interfaces;

namespace VolgaIT.Model.Entities
{
    public class BaseEntity : IEntity
    {
        [Key, Required]
        public long Id { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
