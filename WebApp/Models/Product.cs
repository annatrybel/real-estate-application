﻿using Microsoft.AspNetCore.Mvc;
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
        
        [Range(0, double.MaxValue)] 
        public decimal Price { get; set; }
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();


        [Display(Name = "Category Type")]
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Display(Name = "Listings Type")]
        public int? ListingsTypeId { get; set; }
        [ForeignKey("ListingsTypeId")]
        public virtual ListingsType? ListingsType { get; set; }
        public string? City { get; set; }
        public int? SquareMeters { get; set; }
        public MarketType Market { get; set; } 

        public BuildingType Building { get; set; }
        public DateTime DateAdded { get; set; }
        public ItemStatus Status { get; set; }


        public Product()
        {
            DateAdded = DateTime.Now;
        }

        public enum MarketType
        {
            [Display(Name = "Primary Market")]
            PrimaryMarket,

            [Display(Name = "Secondary Market")]
            SecondaryMarket
        }

        public enum BuildingType
        {
            [Display(Name = "Apartment Block")]
            ApartmentBlock,

            [Display(Name = "Tenement House")]
            TenementHouse,

            [Display(Name = "Townhouse")]
            Townhouse
        }

        public enum ItemStatus
        {
            [Display(Name = "Active")]
            Active,

            [Display(Name = "Reserved")]
            Reserved,

            [Display(Name = "Sold")]
            Sold
        }
    }
    
}
