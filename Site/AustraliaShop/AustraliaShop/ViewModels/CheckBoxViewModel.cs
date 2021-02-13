using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class CheckBoxViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
    }
}