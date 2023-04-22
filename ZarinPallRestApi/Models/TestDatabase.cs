using System.Collections.Generic;
using ZarinPallRestApi.Models;
using ZarinpalRestApi.Models;

namespace ZarinpalRestApi.Helpers
{
    public static class ProductDatabase
    {
        public static readonly List<Product> Data;

        static ProductDatabase()
        {

            Data = new List<Product>
            {
                new Product {Id = 1, Title = "Asp.net 6", Image = "/image/asp.jpg", Price = 20000},
                new Product {Id = 2, Title = "Asp.net 6", Image = "/image/asp.jpg", Price = 10000}
            };
         
        }

        public static Product GetById(int id)
        {
            return Data.FirstOrDefault(x => x.Id == id);
        }
    }
}
