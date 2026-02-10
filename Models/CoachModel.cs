using System.ComponentModel.DataAnnotations;

namespace CoachBookingApp.Models
{
    // Enligt mitt ER: Coach - Id (PK), Name, Title, Specialty, Bio
    // + lägger till createdAt och createdBy
    public class Coach
    {
        public int Id { get; set;}

        [Required]
        [StringLength(100, ErrorMessage = "Namnet kan inte vara mer än 100 tecken.")]
        [Display(Name = "Namn")]
        public string? Name { get; set;}

        [Required]
        [StringLength(100, ErrorMessage = "Titeln kan inte vara mer än 100 tecken.")]
        [Display(Name = "Titel")]
        public string? Title { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Expertis kan inte vara mer än 100 tecken.")]
        [Display(Name = "Expertis")]
        public string? Specialty { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Biografin kan inte vara mer än 1000 tecken.")]
        [Display(Name = "Biografi")]
        public string? Bio { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}