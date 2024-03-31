using System.ComponentModel.DataAnnotations;

namespace OCS_TestTask.Models
{
    public class ApplicationUpdatingPart
    {
        [Required]
        public ActivityEnum Activity { get; set; }

        [Required]
        [MaxLength(99)]
        [MinLength(1)]
        public string Name { get; set; }

        [MaxLength(299)]
        [MinLength(1)]
        public string Description { get; set; }

        [MaxLength(999)]
        [MinLength(1)]
        public string Outline { get; set; }
    }
}
