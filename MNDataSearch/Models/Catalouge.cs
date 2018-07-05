using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNDataSearch.Models
{
    public class Catlouge
    {
        public int SrNo { get; set; }
        public string UniqueNo { get; set; }
        public string ImagePath { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Synopsis { get; set; }
        public string Language { get; set; }
        public int Year { get; set; }
        public double Duration { get; set; }
        public string bW { get; set; }
        public string Director { get; set; }
        public string Producer { get; set; }
        public string CastCrew { get; set; }
        public string MainClass { get; set; }
    }
}
