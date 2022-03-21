using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InfoRetrieval.Models
{
    public class Documents
    {
        [Key]
        public int docID { get; set; }
        
        public string rawDocument { get; set; }
        public string processedDocument { get; set; }
        [Column(TypeName = "text[]")]
        public string[] terms { get; set; }

    }
}
