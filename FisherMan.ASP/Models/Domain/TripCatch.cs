using System.ComponentModel.DataAnnotations;

namespace FisherMan.ASP.Models.Domain
{
    public class TripCatch
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Излет")]
        public int TripId { get; set; }
        public FishingTrip? Trip { get; set; }

        [Required]
        [Display(Name = "Вид риба")]
        [StringLength(100)]
        public string FishType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Тегло (кг)")]
        [Range(0.01, 100000)]
        public decimal WeightKg { get; set; }
    }
}
