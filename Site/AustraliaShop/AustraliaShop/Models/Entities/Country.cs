using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class Country
    {
        public Country()
        {
            Users=new List<User>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name= "Country")]
        public string Title { get; set; }

        [StringLength(2)]
        public string TwoCharCountryCode { get; set; }
        [StringLength(3)]
        public string ThreeCharCountryCode { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}