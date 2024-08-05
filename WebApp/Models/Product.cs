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

        [Display(Name = "Listings Type")]
        public int? ListingsTypeId { get; set; }
        [ForeignKey("ListingsTypeId")]
        public virtual ListingsType? ListingsType { get; set; }

        public string? Address { get; set; } 
        public string? City { get; set; }
        public int? SquareMeters { get; set; }
        public DateTime DateAdded { get; set; }


        public Product()
        {
            DateAdded = DateTime.Now;
        }
    }
}
