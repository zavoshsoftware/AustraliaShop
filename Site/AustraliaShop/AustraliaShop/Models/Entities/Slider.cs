using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Slider:BaseEntity
    {
        [Required]
        public int Order { get; set; }

        [Display(Name = "Image")]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        public string Title { get; set; }

        public string Summery { get; set; }

        public string LinkTitle { get; set; }

        public string LandingPage { get; set; }
    }
}