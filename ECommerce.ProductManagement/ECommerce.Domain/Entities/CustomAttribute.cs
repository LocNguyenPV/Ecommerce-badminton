using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class CustomAttribute
    {
        public Guid AttributeId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int SortOrder { get; set; }
        public bool IsRequired {  get; set; }
        public ICollection<VariantAttribute> VariantAttributes { get; set; }= new List<VariantAttribute>();
    }
}
