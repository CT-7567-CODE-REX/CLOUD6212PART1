using System;

using Azure;
using Azure.Data.Tables;

namespace CLOUD.POE._1.Models
{


    /// Represents one CoffeeNChill menu item stored  in Azure Table Storage.
    
    /// PartitionKey stores the menu category.
    
    /// RowKey stores the unique item SKU.

    public class MenuItemEntity : ITableEntity
        {
            
            /// Menu category such as Hot Drinks or Pastries.
            
            /// Azure Table Storage uses this as the partition.
         
            public string PartitionKey { get; set; } = string.Empty;

          
            /// Unique product identifier such as COF-001.
          
            public string RowKey { get; set; } = string.Empty;

           
            /// Name shown on the CoffeeNChill menu.
           
            public string Name { get; set; } = string.Empty;

           
            /// Short description of the food or drink.
            
            public string Description { get; set; } = string.Empty;

           
            /// Current selling price of the item.
            
            public double Price { get; set; }

            
            /// Indicates whether customers can currently order the item.
           
            public bool IsAvailable { get; set; }

            
            /// Automatically maintained by Azure Table Storage.
            
            public DateTimeOffset? Timestamp { get; set; }

            
            /// Used by Azure to track the version of the entity.
            
            public ETag ETag { get; set; }
        }
    }
