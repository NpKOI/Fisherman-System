using System.ComponentModel.DataAnnotations;

namespace FisherMan.ASP.Models.Domain
{
    public class Inspector
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Три имена")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Номер на значка")]
        [StringLength(50)]
        public string BadgeNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Телефон")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
    }
}
