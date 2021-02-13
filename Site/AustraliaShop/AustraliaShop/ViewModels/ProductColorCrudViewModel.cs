using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ViewModels
{
    public class ProductColorCrudViewModel
    {
        public bool IsActive { get; set; }
        public Guid ColorId { get; set; }
        public Guid? ProductId { get; set; }
    }
}