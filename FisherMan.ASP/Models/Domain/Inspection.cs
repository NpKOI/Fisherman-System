using System.ComponentModel.DataAnnotations;

namespace FisherMan.ASP.Models.Domain
{
    public enum InspectionType
    {
        [Display(Name = "Риболовен кораб")]
        FishingShip,
        [Display(Name = "Хладилен камион")]
        RefrigeratedTruck,
        [Display(Name = "Магазин")]
        Store
    }

    public class Inspection
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Инспектор")]
        public int InspectorId { get; set; }
        public Inspector? Inspector { get; set; }

        [Required]
        [Display(Name = "Дата на инспекция")]
        [DataType(DataType.DateTime)]
        public DateTime InspectionDate { get; set; }

        [Required]
        [Display(Name = "Тип на инспекция")]
        public InspectionType Type { get; set; }

        [Required]
        [Display(Name = "Проверен обект")]
        [StringLength(200)]
        public string InspectedObject { get; set; } = string.Empty;

        [Display(Name = "Бележки")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        [Display(Name = "Нарушение")]
        public bool HasViolation { get; set; }

        [Display(Name = "Сума на глобата (лв)")]
        [Range(0, 1000000)]
        public decimal? FineAmount { get; set; }

        [Display(Name = "Номер на акт")]
        [StringLength(50)]
        public string? ActNumber { get; set; }
    }
}
