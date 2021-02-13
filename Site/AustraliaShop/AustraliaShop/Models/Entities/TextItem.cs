using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class TextItem : BaseEntity
    {
        public string Title { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Summery { get; set; }
     

        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }


        public string LinkUrl { get; set; }
        public string LinkTitle { get; set; }

        public Guid? TextItemTypeId { get; set; }
        public virtual  TextItemType  TextItemType { get; set; }
    }
}