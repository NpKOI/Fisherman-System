using System.ComponentModel.DataAnnotations;

namespace FisherMan.ASP.Models.Domain
{
    public class Ship
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Международен номер")]
        [StringLength(50)]
        public string InternationalNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Позивна")]
        [StringLength(50)]
        public string CallSign { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Маркировка")]
        [StringLength(50)]
        public string Marking { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Собственик")]
        [StringLength(150)]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "ЕГН на собственика")]
        [StringLength(10)]
        public string OwnerEGN { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Капитан")]
        [StringLength(150)]
        public string CaptainName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Дължина (м)")]
        [Range(0.1, 1000)]
        public decimal Length { get; set; }

        [Required]
        [Display(Name = "Ширина (м)")]
        [Range(0.1, 1000)]
        public decimal Width { get; set; }

        [Required]
        [Display(Name = "Тонаж (т)")]
        [Range(0.1, 100000)]
        public decimal Tonnage { get; set; }

        [Required]
        [Display(Name = "Газене (м)")]
        [Range(0.1, 100)]
        public decimal Draft { get; set; }

        [Required]
        [Display(Name = "Мощност двигател (кW)")]
        [Range(0.1, 100000)]
        public decimal EnginePower { get; set; }

        [Required]
        [Display(Name = "Тип двигател")]
        [StringLength(100)]
        public string EngineType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Тип гориво")]
        [StringLength(50)]
        public string FuelType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Среден разход гориво (л/ч)")]
        [Range(0.1, 10000)]
        public decimal FuelConsumptionPerHour { get; set; }

        public ICollection<FishingPermit> Permits { get; set; } = new List<FishingPermit>();
    }
}
