using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FisherMan.ASP.Models.Domain
{
    public enum TicketValidityType
    {
        [Display(Name = "Годишен")]
        Annual,
        [Display(Name = "6 месеца")]
        SixMonths,
        [Display(Name = "Месечен")]
        Monthly,
        [Display(Name = "Дневен")]
        Daily
    }

    public class FishingTicket
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Три имена")]
        [StringLength(150)]
        public string HolderName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ЕГН")]
        [StringLength(10)]
        public string HolderEGN { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Дата на раждане")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Инвалид")]
        public bool IsDisabled { get; set; }

        [Display(Name = "Номер на решение ТЕЛК")]
        [StringLength(50)]
        public string? DisabilityDecisionNumber { get; set; }

        [Display(Name = "Пенсионер")]
        public bool IsPensioner { get; set; }

        [Required]
        [Display(Name = "Тип билет")]
        public TicketValidityType ValidityType { get; set; }

        [Required]
        [Display(Name = "Дата на издаване")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required]
        [Display(Name = "Дата на изтичане")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [Display(Name = "Цена (лв)")]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [NotMapped]
        [Display(Name = "Валиден")]
        public bool IsValid => ExpiryDate >= DateTime.Today;

        [NotMapped]
        [Display(Name = "Под 14 години")]
        public bool IsUnder14 => (DateTime.Today - BirthDate).TotalDays / 365.25 < 14;

        public ICollection<AmateurCatch> Catches { get; set; } = new List<AmateurCatch>();
    }
}
