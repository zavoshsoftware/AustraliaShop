using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Models
{
   public class DatabaseContext:DbContext
    {
        static DatabaseContext()
        {
             System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>());
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogGroup> BlogGroups { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<TextItem> TextItems { get; set; }
        public DbSet<ContactUsForm> ContactUsForms { get; set; }
        public DbSet<TextItemType> TextItemTypes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }

        public System.Data.Entity.DbSet<Models.Size> Sizes { get; set; }

        public System.Data.Entity.DbSet<Models.Color> Colors { get; set; }
    }
}
