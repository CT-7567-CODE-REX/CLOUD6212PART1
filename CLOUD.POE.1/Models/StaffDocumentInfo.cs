using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLOUD.POE._1.Models
{
    public class StaffDocumentInfo
    {
        // Name of the file stored in the staff-docs share.
        public string FileName { get; set; } = string.Empty;

        // File size in bytes.
        public long Size { get; set; }

        // Time the stored file was last modified.
        public DateTimeOffset? LastModified { get; set; }
    }
}