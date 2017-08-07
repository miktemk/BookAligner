using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookAligner.Code
{
    public class DocAlignmentData
    {
        public List<DocAlignmentRegex> Regexes { get; } = new List<DocAlignmentRegex>();
        public List<DocAlignmentManual> Manual { get; } = new List<DocAlignmentManual>();
    }

    public class DocAlignmentRegex
    {
        public string Regex1 { get; set; }
        public string Regex2 { get; set; }
    }

    public class DocAlignmentManual
    {
        public int Position1 { get; set; }
        public int Position2 { get; set; }
        public int Length1 { get; set; }
        public int Length2 { get; set; }
    }
}
