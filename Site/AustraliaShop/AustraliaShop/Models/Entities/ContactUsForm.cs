using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class ContactUsForm:BaseEntity
    {
        [Required]
        [MaxLength(300)]
        public string Name { get; set; }

        [Required]
        [MaxLength(300)]
        public string Email { get; set; }

        [Required]
        [MaxLength(300)]
        [Column(TypeName = "ntext")]
        public string Message { get; set; }

        [Required]
        [MaxLength(50)]
        public string Ip { get; set; }
    }
}