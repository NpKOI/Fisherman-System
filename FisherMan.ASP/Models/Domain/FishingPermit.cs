using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FisherMan.ASP.Models.Domain
{
    public class FishingPermit
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Кораб")]
        public int ShipId { get; set; }
        public Ship? Ship { get; set; }

        [Required]
        [Display(Name = "Дата на издаване")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [Required]
        [Display(Name = "Дата на изтичане")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Отнето")]
        public bool IsRevoked { get; set; }

        [Display(Name = "Дата на отнемане")]
        [DataType(DataType.Date)]
        public DateTime? RevokedDate { get; set; }

        [Required]
        [Display(Name = "Капитан")]
        [StringLength(150)]
        public string CaptainName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Риболовни уреди")]
        [StringLength(500)]
        public string FishingTools { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Активно")]
        public bool IsActive => !IsRevoked && ExpiryDate >= DateTime.Today;

        public ICollection<FishingTrip> FishingTrips { get; set; } = new List<FishingTrip>();
    }
}
