using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class DiscountCode:BaseEntity
    {
        public DiscountCode()
        {
            Orders=new List<Order>();
        }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }
        [Display]
        public bool IsPercent { get; set; }

        [Required] 
        public decimal Amount { get; set; }

        public bool IsMultiUsing { get; set; }


        public virtual ICollection<Order> Orders { get; set; }
    }
}
