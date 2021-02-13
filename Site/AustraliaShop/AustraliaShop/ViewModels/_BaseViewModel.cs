using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Helpers;
using Models;

namespace ViewModels
{

    public class _BaseViewModel
    {
        private BaseViewModelHelper baseviewmodel = new BaseViewModelHelper();

        public List<MenuItems> MenuItems { get { return baseviewmodel.GetMenuProductGroup(); } }

        //public string Address { get { return baseviewmodel.GetTextItemByName("address", "summery"); } }
        //public string Phone { get { return baseviewmodel.GetTextItemByName("header-phone", "summery"); } }
        //public string Email { get { return baseviewmodel.GetTextItemByName("email", "summery"); } }
        //public string Instagram { get { return baseviewmodel.GetTextItemByName("Instagram", "linkUrl"); } }
        //public string Telegram { get { return baseviewmodel.GetTextItemByName("telegram", "linkUrl"); } }
        //public string Aparat { get { return baseviewmodel.GetTextItemByName("aparat", "linkUrl"); } }
        //public string Youtube { get { return baseviewmodel.GetTextItemByName("youtube", "linkUrl"); } }
        public bool IsAuthenticate { get { return baseviewmodel.GetAuthenticationStatus(); } }
        public string UserFullName { get { return baseviewmodel.GetAuthenticateUserName(); } }
        public string UserRole { get { return baseviewmodel.GetRoleTitle(); } }
        //public string WhatsappPhone { get { return baseviewmodel.GetTextItemByName("whatsapp-phone", "summery"); } }
        //public string WhatsappText { get { return baseviewmodel.GetTextItemByName("whatsapp-text", "summery"); } }
    }

    public class MenuItems
    {
        public ProductGroupItemViewModel ParentProductGroup { get; set; }
        public List<ProductGroupItemViewModel> ChildProductGroups { get; set; }

    }

    public class ProductGroupItemViewModel
    {
        public string Title { get; set; }
        public string UrlParam { get; set; }
    }
}