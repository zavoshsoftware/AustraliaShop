using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class ProductImage:BaseEntity
    {
        public int Order { get; set; }
        public string ImageUrl { get; set; }
        public string AltText { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}