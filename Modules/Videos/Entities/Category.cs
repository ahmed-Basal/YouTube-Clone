using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using youtube.SharedKernel;

namespace youtube.Modules.Videos.Entities
{
    [Table("Categories")]
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
