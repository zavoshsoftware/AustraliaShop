
namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Globalization;
    using System.Linq;

    public class User : BaseEntity
    {
        public User()
        {
            Orders = new List<Order>();
        }
        [Required]
        [StringLength(350)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string CellNum { get; set; }


        [StringLength(256)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
        public string Email { get; set; }
      
        [StringLength(150)]
        public string Password { get; set; }


        [Display(Name = "Gender")]
        public Guid? GenderId { get; set; }

        public Guid RoleId { get; set; }

        public decimal? RemainCredit { get; set; }

        public DateTime? BirthDate { get; set; }
     
        public virtual Gender Gender { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public int? CountryId { get; set; }
        public Country Country { get; set; }
    }
}

