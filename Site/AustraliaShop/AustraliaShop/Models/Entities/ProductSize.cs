using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class ProductSize : BaseEntity
    {
        public ProductSize()
        {
            OrderDetails = new List<OrderDetail>();
        }
        public Guid SizeId { get; set; }
        public virtual Size Size { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Stock { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}