
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Color : BaseEntity
    {
        public Color()
        {
            Products = new List<Product>();
        }
 

        [Display(Name ="Color")]
        public string Title { get; set; }

        [Display(Name ="Color Hex Code")]
        public string HexCode { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}