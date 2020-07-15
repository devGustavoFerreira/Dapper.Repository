using System;

namespace DapperRepository.Tests
{
    public class Product : BaseEntity
    {
        public Product()
        {
            CreatedDate = DateTime.Now;
        }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}