using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace Helpers
{
    public class CodeGenerator
    {
        private static object lockobj = new object();

        private DatabaseContext db = new DatabaseContext();
        public int ReturnOrderCode()
        {

            Order order = db.Orders.Where(c => c.IsDeleted == false).OrderByDescending(current => current.Code).FirstOrDefault();

            if (order != null)
            {
                lock (lockobj)
                {
                    return order.Code + 1;
                }
            }
            return 100000;
        }


        public int ReturnProductCode()
        {
            Product product = db.Products.Where(c => c.IsDeleted == false).OrderByDescending(current => current.Code).FirstOrDefault();

            if (product != null)
            {
                lock (lockobj)
                {
                    return Convert.ToInt32(product.Code) + 1;
                }
            }
            return 100;
        }

    }
}