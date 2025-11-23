using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class ProductImage
    {
        public Guid ImageId { get; set; }
        public Guid ProductId { get; set; }
        public string Url {  get; set; }
        public string AltText { get; set; }
        public int SortOrder { get; set; }
        public bool IsPrimary { get; set; }
        public long CreatedAt { get; set; } = DateTime.Now.Ticks;
        public Product Product { get; set; } = null;
    }
}
