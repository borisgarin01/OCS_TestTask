using System.ComponentModel.DataAnnotations;

namespace OCS_TestTask.Models.Models
{
    public sealed class Application
    {
        [Key]
        public Guid Id { get; init; }
        [Required]
        public Guid AuthorId { get; init; }

        [Required]
        public int? Activity { get; init; }

        [Required]
        [MaxLength(99)]
        [MinLength(1)]
        public string Name { get; init; }


        public DateTime CreationTimeStamp { get; } = DateTime.Now;

        [MaxLength(299)]
        [MinLength(1)]
        public string Description { get; init; }

        [MaxLength(999)]
        [MinLength(1)]
        public string Outline { get; init; }
    }
}
