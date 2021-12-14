using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class CustomerType : BaseEntity
    {
        public string Title { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}