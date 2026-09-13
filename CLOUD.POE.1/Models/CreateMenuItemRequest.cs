using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLOUD.POE._1.Models
{

    /// Represents the JSON body sent when staff create a new CoffeeNChill menu item.
    
    /// Used by:
    /// POST /api/menu
    
    public class CreateMenuItemRequest
    {
        
        /// Menu category, for example Hot Drinks.
   
        public string Category { get; set; } = string.Empty;

     
        /// Unique CoffeeNChill SKU, for example COF-001.
   
        public string Id { get; set; } = string.Empty;

   
        /// Item name shown to customers.
       
        public string Name { get; set; } = string.Empty;

      
        /// Short menu description.
        
        public string Description { get; set; } = string.Empty;

    
        /// Selling price of the item.
    
        public double Price { get; set; }

       
        /// Shows whether the item is currently available.
        
        public bool IsAvailable { get; set; }
    }
}
