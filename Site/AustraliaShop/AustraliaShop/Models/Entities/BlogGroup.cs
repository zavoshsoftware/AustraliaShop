using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class BlogGroup : BaseEntity
    {
        public BlogGroup()
        {
            Blogs = new List<Blog>();
        }
        [Required]
        [Display(Name="Blog Group Title")]
        public string Title { get; set; }

        [Required]
        public string Summery { get; set; }

        [Required]
        public string UrlParam { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }

    }
}