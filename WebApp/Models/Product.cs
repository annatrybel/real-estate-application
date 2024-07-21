using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string Description { get; set; }
        [Range(0, int.MaxValue)]
        public double Price { get; set; }
        public string? Image { get; set; }

        [Display(Name = "Category Type")]
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Display(Name = "Application Type")]
        public int? ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual ApplicationType? ApplicationType { get; set; }
    }
}
