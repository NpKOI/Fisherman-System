using System.ComponentModel.DataAnnotations;

namespace FisherMan.ASP.Models.Domain
{
    public class AmateurCatch
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Билет")]
        public int TicketId { get; set; }
        public FishingTicket? Ticket { get; set; }

        [Required]
        [Display(Name = "Дата на улов")]
        [DataType(DataType.Date)]
        public DateTime CatchDate { get; set; }

        [Required]
        [Display(Name = "Вид риба")]
        [StringLength(100)]
        public string FishType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Тегло (кг)")]
        [Range(0.01, 10000)]
        public decimal WeightKg { get; set; }

        [Required]
        [Display(Name = "Локация")]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;
    }
}
