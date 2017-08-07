using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BookAligner.Code
{
    public class WordHighlight
    {
        public WordHighlight(int startChar, int length)
        {
            StartIndex = startChar;
            Length = length;
        }

        public int StartIndex { get; set; }
        public int Length { get; set; }

        public WordHighlight MakeCopy()
        {
            return new WordHighlight(StartIndex, Length);
        }
    }

    public class WordHighlightWithBrush : WordHighlight
    {
        public WordHighlightWithBrush(int startChar, int length)
            : base(startChar, length)
        { }

        public WordHighlightWithBrush(WordHighlight x)
            : base(x.StartIndex, x.Length)
        { }

        public Brush Foreground { get; set; }
        public Brush Background { get; set; }
    }
}
