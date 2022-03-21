using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InfoRetrieval.Models
{
    public class Dictionary
    {
        [Key]
        public int termId { get; set; }
        public string term { get; set; }
        public int[] docID { get; set; }
        public double[] tfPerDoc { get; set; }
        public double df { get; set; }

    }
}
