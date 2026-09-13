using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLOUD.POE._1.Models
{

    /// Represents the menu item returned to Postman or another client using the CoffeeNChill API.

    /// It hides Azure-specific terms such as PartitionKey and RowKey from the client.
    

    public class MenuItemResponse
    {
        /// Menu category.
        
        public string Category { get; set; } = string.Empty;

        /// CoffeeNChill product SKU.
       
        public string Id { get; set; } = string.Empty;

        /// Product name.
        
        public string Name { get; set; } = string.Empty;

      
        /// Product description.
       
        public string Description { get; set; } = string.Empty;

        
        /// Current selling price.
       
        public double Price { get; set; }

     
        /// Shows whether customers can currently order the item.
        
        public bool IsAvailable { get; set; }
    }
}