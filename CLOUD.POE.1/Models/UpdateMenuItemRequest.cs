using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLOUD.POE._1.Models
{

    /// Contains only the values staff are allowed to update for an existing menu item
   
    /// Both properties are nullable because staff may update only one of them.
    

    public class UpdateMenuItemRequest
    {
       
        /// New price, null means keep the existing price.
       
        public double? Price { get; set; }

        
        /// New availability status.
        /// Null means leave availability unchanged.
     
        public bool? IsAvailable { get; set; }
    }
}