using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Models;
using ViewModels;

//using ViewModels;

namespace Helpers
{
    public class BaseViewModelHelper
    {
        private DatabaseContext db = new DatabaseContext();

        public List<MenuItems> GetMenuProductGroup()
        {
            List<MenuItems> menuItems = new List<MenuItems>();

           var productGroups = db.ProductGroups
                .Where(c => c.ParentId == null && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).Select(i=>new
                {
                    i.Title,
                    i.UrlParam,
                    i.Id
                });

            foreach (var productGroup in productGroups)
            {
                var childGroups = db.ProductGroups.Where(c => c.ParentId == productGroup.Id).Select(c => new
                {
                    c.Title,
                    c.UrlParam
                });

                List<ProductGroupItemViewModel> chilProductGroupItemViewModels=new List<ProductGroupItemViewModel>();

                foreach (var childGroup in childGroups)
                {
                    chilProductGroupItemViewModels.Add(new ProductGroupItemViewModel()
                    {
                        Title = childGroup.Title,
                        UrlParam = childGroup.UrlParam
                    });
                }

               menuItems.Add(new MenuItems()
               {
                   ParentProductGroup = new ProductGroupItemViewModel()
                   {
                       Title = productGroup.Title,
                       UrlParam = productGroup.UrlParam
                   },
                   ChildProductGroups = chilProductGroupItemViewModels
               });
            }
            return menuItems;
        }


        public string GetTextItemByName(string name, string field)
        {
            TextItem textItem = db.TextItems.FirstOrDefault(c => c.Name == name);
            if (textItem != null)
            {
                if (field == "summery")
                    return textItem.Summery;
                return textItem.LinkUrl;
            }
            return string.Empty;
        }

        public bool GetAuthenticationStatus()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
        }

        public string GetAuthenticateUserName()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)HttpContext.Current.User.Identity;
                string name= identity.FindFirst(System.Security.Claims.ClaimTypes.Surname).Value;
                return name;
            }
            return "ورود";
        }
        public string GetRoleTitle()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)HttpContext.Current.User.Identity;
                string role= identity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value;
                return role;
            }
            return "";
        }

   
    }
}