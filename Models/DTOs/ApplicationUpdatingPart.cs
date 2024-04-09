using System.ComponentModel.DataAnnotations;

namespace OCS_TestTask.Models.DTOs
{
    public sealed class ApplicationUpdatingPart
    {
        [Required]
        public ActivityEnum Activity { get; init; }

        [Required]
        [MaxLength(99)]
        [MinLength(1)]
        public string Name { get; init; }

        [MaxLength(299)]
        [MinLength(1)]
        public string Description { get; init; }

        [MaxLength(999)]
        [MinLength(1)]
        public string Outline { get; init; }
    }
}
