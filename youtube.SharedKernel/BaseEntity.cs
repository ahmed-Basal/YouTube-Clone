using System.ComponentModel.DataAnnotations;

namespace youtube.SharedKernel
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }

    // Backward-compatibility alias for previous typo
    public abstract class BaseEntitiy : BaseEntity
    {
    }
}
