﻿using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    internal class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(256)]
        public string ThumbnailimagePath { get; set; }

        [Required]
        [MaxLength(128)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1024)]
        public string Description { get; set; }
    }
}
