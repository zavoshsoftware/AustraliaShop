namespace Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Role: BaseEntity
    {
        public Role()
        {
            Users = new List<User>();
        }


        [Required]
        [StringLength(50)]
        [Display(Name = "Role")]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
