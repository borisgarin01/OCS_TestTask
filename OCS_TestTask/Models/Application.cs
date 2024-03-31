using System.ComponentModel.DataAnnotations;

namespace OCS_TestTask.Models
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid AuthorId { get; set; }

        [Required]
        public int? Activity { get; set; }

        [Required]
        [MaxLength(99)]
        [MinLength(1)]
        public string Name { get; set; }


        public DateTime CreationTimeStamp { get; set; } = DateTime.Now;

        [MaxLength(299)]
        [MinLength(1)]
        public string Description { get; set; }

        [MaxLength(999)]
        [MinLength(1)]
        public string Outline { get; set; }
    }
}
