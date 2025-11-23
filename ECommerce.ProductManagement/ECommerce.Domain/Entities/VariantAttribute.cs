using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities
{
    public class VariantAttribute
    {
        public Guid VariantAttributeId { get; set; }
        public Guid VariantId { get; set; }
        public Guid AttributeId { get; set; }
        public string AttributeValue { get; set; } = string.Empty;
        public ProductVariant Variant { get; set; }
        public CustomAttribute CustomAttribute { get; set; }

    }
}
