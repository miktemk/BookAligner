using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using Miktemk;

namespace BookAligner.Code
{
    public class DocAlignment
    {
        private IEnumerable<DocSection> sentences = Enumerable.Empty<DocSection>();
        private AlgorithmArrayAlign algoArrayAlign = new AlgorithmArrayAlign();

        public DocAlignmentData AlignData { get; set; }

        public string Text1 { get; set; }
        public string Text2 { get; set; }

        public DocAlignment()
        {
        }

        public WordHighlight GetSentence2(int position1)
        {
            int wordStart, wordLength;
            var selection2 = (long)Text2.Length * position1 / Text1.Length;
            Utils.FindWord(Text2, (int)selection2, out wordStart, out wordLength);
            return new WordHighlight(wordStart, wordLength);
            // TODO use sections  `d21f` to compute the match
        }

        public void RecomputeSentenceAlignment()
        {
            var manualHighlights1 = Utils.GetAlignManual(AlignData, Text1, 0, Text1.Length, take2: false);
            var sectionAll1 = new DocSection(0, Text1.Length);
            var sectionsByManual1 = Utils.SplitSection(sectionAll1, manualHighlights1);

            var manualHighlights2 = Utils.GetAlignManual(AlignData, Text2, 0, Text2.Length, take2: true);
            var sectionAll2 = new DocSection(0, Text2.Length);
            var sectionsByManual2 = Utils.SplitSection(sectionAll2, manualHighlights2);

            sectionsByManual1.EnumerateWith(sectionsByManual2, (sectionM1, sectionM2, indexM) =>
            {
                var regexHighlights1 = Utils.GetAlignRegexes(AlignData, Text1, sectionM1.StartIndex, sectionM1.Length, take2: false)
                                            .OrderBy(x => x.StartIndex)
                                            .ToArray();
                var regexHighlights2 = Utils.GetAlignRegexes(AlignData, Text2, sectionM2.StartIndex, sectionM2.Length, take2: true)
                                            .OrderBy(x => x.StartIndex)
                                            .ToArray();

                var regexPositions1 = regexHighlights1.Select(x => x.StartIndex).ToArray();
                var regexPositions2 = regexHighlights2.Select(x => x.StartIndex).ToArray();

                var regexAlignmentIndices = algoArrayAlign.GetBestAlignment(regexPositions1, regexPositions2, Globals.CharThreshRegexAlign);

                // TODO: use this data to align them store all sections `d21f`
            });

            //var regexHighlights = Utils.GetAlignRegexes(AlignData, text, 0, text.Length, take2);

        }

        #region ----------------- highlights and UI -------------------------------------

        public IEnumerable<WordHighlightWithBrush> GetEmphasizedWords1()
        {
            return GetEmphasizedWords(take2: false);
        }


        public IEnumerable<WordHighlightWithBrush> GetEmphasizedWords2()
        {
            return GetEmphasizedWords(take2: true);
        }


        private IEnumerable<WordHighlightWithBrush> GetEmphasizedWords(bool take2)
        {
            if (AlignData == null)
                return Enumerable.Empty<WordHighlightWithBrush>();
            var result = new List<WordHighlightWithBrush>();

            var text = take2 ? Text2 : Text1;
            var regexHighlights = Utils.GetAlignRegexes(AlignData, text, 0, text.Length, take2);
            result.AddRange(regexHighlights.Select(x => new WordHighlightWithBrush(x)
            {
                Foreground = Brushes.DeepSkyBlue,
            }));

            var manualHighlights = Utils.GetAlignManual(AlignData, text, 0, text.Length, take2);
            result.AddRange(manualHighlights.Select(x => new WordHighlightWithBrush(x)
            {
                Background = Brushes.Coral,
            }));

            return result;
        }

        #endregion

    }
    public class DocSection
    {
        public DocSection(int startIndex, int length)
        {
            StartIndex = startIndex;
            Length = length;
        }

        public int StartIndex { get; set; }
        public int Length { get; set; }

        /// <summary>
        /// Returns posB, B being this object
        /// </summary>
        public int GetRelativePosition(DocSection sectionA, int posA)
        {
            return (int)(StartIndex + (long)Length * posA / sectionA.Length);
        }
    }
}
