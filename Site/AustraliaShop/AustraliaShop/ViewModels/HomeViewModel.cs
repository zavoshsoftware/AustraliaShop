using System.Collections.Generic;
using Models;

namespace ViewModels
{
    public class HomeViewModel : _BaseViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<ProductItemViewModel> NewProducts { get; set; }
        public List<ProductItemViewModel> BestSaleProducts { get; set; }
        public List<ProductItemViewModel> SpecialOfferProducts { get; set; }
        public List<ProductItemViewModel> TopRatedProducts { get; set; }
        public List<DealOfDayProductItemViewModel> DealOfDayProducts { get; set; }

        public List<ProductGroup> ProductGroups { get; set; }
    }
}