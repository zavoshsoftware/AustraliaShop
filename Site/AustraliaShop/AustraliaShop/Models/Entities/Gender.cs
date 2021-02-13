namespace Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public  class Gender : BaseEntity
    {
        public Gender()
        {
            Users = new List<User>();
        }


        [Required]
        [StringLength(10)]
        [Display(Name = "Title")]
        public string Title { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
