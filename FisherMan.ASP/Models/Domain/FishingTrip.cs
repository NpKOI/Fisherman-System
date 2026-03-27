using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FisherMan.ASP.Models.Domain
{
    public class FishingTrip
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Разрешително")]
        public int PermitId { get; set; }
        public FishingPermit? Permit { get; set; }

        [Required]
        [Display(Name = "Начало на риболова")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Начална локация")]
        [StringLength(200)]
        public string StartLocation { get; set; } = string.Empty;

        [Display(Name = "Край на риболова")]
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Риболовни уреди")]
        [StringLength(500)]
        public string FishingTools { get; set; } = string.Empty;

        [Display(Name = "Продължителност (часове)")]
        [NotMapped]
        public decimal? DurationHours =>
            EndDate.HasValue ? (decimal)(EndDate.Value - StartDate).TotalHours : null;

        [Required]
        [Display(Name = "Общ улов (кг)")]
        [Range(0, 1000000)]
        public decimal TotalCatchKg { get; set; }

        [Display(Name = "Изразходено гориво (л)")]
        [Range(0, 1000000)]
        public decimal? FuelConsumed { get; set; }

        public ICollection<TripCatch> Catches { get; set; } = new List<TripCatch>();
    }
}
