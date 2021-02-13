using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Models
{
    public class BaseEntity : object
    {

        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public System.Guid Id { get; set; }

        [DefaultValue(true)]
        [Display(Name="Active")]
        public bool IsActive { get; set; }

        public System.DateTime CreationDate { get; set; }
      

        public System.DateTime? LastModifiedDate { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public System.DateTime? DeletionDate { get; set; }
 
        [AllowHtml]
        [Display(Name = "Note")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
 
    }
}
