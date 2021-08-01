 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class Supplier:BaseEntity
    {
        public Supplier()
        {
            Products=new List<Product>();
        }
        public string Title { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}