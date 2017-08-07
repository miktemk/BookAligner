using Miktemk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BookAligner.Code
{
    public static class Utils
    {
        public static void FindParagraph(string text, int position, out int paraStart, out int paraLength)
        {
            paraStart = 0;
            paraLength = text.Length;

            if (position < 0 || position >= text.Length)
            {
                paraLength = 0;
                return;
            }
            
            // go back
            var i = position;
            while (i > 0)
            {
                if (text[i] == '\n') // NOTE: sequence is \r\n so you will never encounter \r on the way back
                {
                    paraStart = i + 1;
                    break;
                }
                i--;
            }

            // go forward
            i = position;
            while (i < text.Length)
            {
                if (text[i] == '\n' || text[i] == '\r')
                    break;
                i++;
            }
            paraLength = i - paraStart;
        }

        public static void FindWord(string text, int position, out int paraStart, out int paraLength)
        {
            paraStart = 0;
            paraLength = text.Length;

            if (position < 0)
            {
                paraLength = 0;
                return;
            }

            if (position >= text.Length)
            {
                // last word
                position = text.Length - 1;
            }

            // make sure we are not on whitespace
            var noWsCharPosition = position;
            while (noWsCharPosition >= 0 && char.IsWhiteSpace(text[noWsCharPosition]))
                noWsCharPosition--;

            // no chars above? search below
            if (noWsCharPosition == -1)
            {
                noWsCharPosition = position;
                while (noWsCharPosition < text.Length && char.IsWhiteSpace(text[noWsCharPosition]))
                    noWsCharPosition++;
            }

            if (noWsCharPosition < 0 || noWsCharPosition > text.Length - 1)
            {
                paraStart = -1;
                paraLength = 0;
                return;
            }

            // go back
            var i = noWsCharPosition;
            while (i > 0)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    paraStart = i + 1;
                    break;
                }
                i--;
            }

            // go forward
            i = noWsCharPosition;
            while (i < text.Length)
            {
                if (char.IsWhiteSpace(text[i]))
                    break;
                i++;
            }
            paraLength = i - paraStart;
        }

        #region ----------------- helpers -------------------------------------

        public static IEnumerable<WordHighlight> GetAlignRegexes(DocAlignmentData alignData, string text, int start, int length, bool take2)
        {
            var result = new List<WordHighlight>();
            text = (start == 0 && length == text.Length)
                ? text
                : text.Substring(start, length);
            foreach (var regexAlign in alignData.Regexes)
            {
                var regex = new Regex(take2 ? regexAlign.Regex2 : regexAlign.Regex1);
                var matches = regex.Matches(text);
                for (int i = 0; i < matches.Count; i++)
                {
                    var mo = matches[i];
                    result.Add(new WordHighlight(mo.Index + start, mo.Length));
                }
            }
            return result;
        }

        public static IEnumerable<WordHighlight> GetAlignManual(DocAlignmentData alignData, string text, int v, int length, bool take2)
        {
            return alignData.Manual.Select(align => new WordHighlightWithBrush(
                take2 ? align.Position2 : align.Position1,
                take2 ? align.Length2 : align.Length1
            ));
        }

        public static IEnumerable<DocSection> SplitSection(DocSection sectionAll, IEnumerable<WordHighlight> manualHighlights)
        {
            var withinHighlights = manualHighlights
                .Where(x => x.StartIndex >= sectionAll.StartIndex && x.StartIndex <= sectionAll.StartIndex + sectionAll.Length)
                .OrderBy(x => x.StartIndex);
            if (!manualHighlights.Any())
                sectionAll.ToIEnumerable();

            var results = new List<DocSection>();
            var indexPrev = sectionAll.StartIndex;
            foreach (var highlight in withinHighlights)
            {
                var indexCur = highlight.StartIndex;
                if (highlight.StartIndex <= indexPrev)
                    continue;
                results.Add(new DocSection(indexPrev, indexCur - indexPrev));
                indexPrev = indexCur;
            }
            if (indexPrev < sectionAll.StartIndex + sectionAll.Length) // last one!
                results.Add(new DocSection(indexPrev, sectionAll.StartIndex + sectionAll.Length - indexPrev));
            return results;
        }

        #endregion
    }
}
