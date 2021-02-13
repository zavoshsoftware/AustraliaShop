using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Size:BaseEntity
    {
        public Size()
        {
            ProductSizes=new List<ProductSize>();
        }
        public int Order { get; set; }

        [Display(Name ="size")]
        public string Title { get; set; }

        public virtual ICollection<ProductSize> ProductSizes { get; set; }
    }
}