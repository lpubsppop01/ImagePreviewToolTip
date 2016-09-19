using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

namespace lpubsppop01.ImagePreviewToolTip
{
    class StringSpan
    {
        public string Value { get; set; }
        public Microsoft.VisualStudio.Text.Span Span { get; set; }
        public SnapshotSpan LineSnapshotSpan { get; set; }
    }

    class StringSpanSearcher
    {
        readonly IWpfTextView view;
        readonly ITextStructureNavigator navigatorService;

        public StringSpanSearcher(IWpfTextView view, ITextStructureNavigator navigatorService)
        {
            this.view = view;
            this.navigatorService = navigatorService;
        }

        public StringSpan GetStringSpanAtMousePosition()
        {
            List<StringSpan> stringSpans;
            SnapshotSpan? lineSnapshotSpan;
            if (!TryGetStringSpansInLineAtMousePosition(out stringSpans, out lineSnapshotSpan)) return null;

            StringSpan matchedSpan;
            if (!TryGetStringSpanAtMousePosition(stringSpans, out matchedSpan)) return null;
            return matchedSpan;
        }

        bool TryGetStringSpansInLineAtMousePosition(out List<StringSpan> stringSpans, out SnapshotSpan? lineSpan)
        {
            stringSpans = null;
            lineSpan = GetLineSpanAtMousePosition(this.view, this.navigatorService);
            if (lineSpan == null) return false;

            stringSpans = new List<StringSpan>();
            {
                string lineText = lineSpan.Value.GetText();
                int iNext = 0;
                while (iNext != -1)
                {
                    int iStart, iEnd;
                    if (!TryGetQuotedStringRange(lineText, iNext, '"', out iStart, out iEnd) &&
                        !TryGetQuotedStringRange(lineText, iNext, '\'', out iStart, out iEnd)) break;
                    stringSpans.Add(new StringSpan
                    {
                        Value = lineText.Substring(iStart + 1, iEnd - iStart - 1),
                        Span = new Microsoft.VisualStudio.Text.Span(lineSpan.Value.Start.Position + iStart, iEnd - iStart),
                        LineSnapshotSpan = lineSpan.Value
                    });
                    iNext = iEnd + 1;
                }
            }
            return stringSpans.Any();
        }

        static bool TryGetQuotedStringRange(string str, int iStart, char quoteChar, out int iRangeStart, out int iRangeEnd)
        {
            iRangeEnd = -1;
            iRangeStart = GetNotEscapedQuoteIndex(str, iStart, quoteChar);
            if (iRangeStart == -1) return false;
            iRangeEnd = GetNotEscapedQuoteIndex(str, iRangeStart + 1, quoteChar);
            if (iRangeEnd == -1) return false;
            return true;
        }

        static int GetNotEscapedQuoteIndex(string str, int iStart, char quoteChar)
        {
            if (iStart < 0 || iStart > str.Length - 1) return -1;

            int iFound = str.IndexOf(quoteChar, iStart);
            if (iFound == -1) return -1;

            if (iFound > 0 && str[iFound - 1] == '\\')
            {
                return GetNotEscapedQuoteIndex(str, iFound + 1, quoteChar);
            }
            else if (iFound < str.Length - 1 && str[iFound] == str[iFound + 1])
            {
                return GetNotEscapedQuoteIndex(str, iFound + 2, quoteChar);
            }
            return iFound;
        }

        static SnapshotSpan? GetLineSpanAtMousePosition(IWpfTextView view, ITextStructureNavigator navigatorService)
        {
            // ref. https://github.com/NoahRic/GoToDef/blob/master/GoToDefMouseHandler.cs
            var position = Mouse.GetPosition(view.VisualElement);
            var line = view.TextViewLines.GetTextViewLineContainingYCoordinate(position.Y + view.ViewportTop);
            if (line == null) return null;
            return line.Extent;
        }

        bool TryGetStringSpanAtMousePosition(List<StringSpan> stringItems, out StringSpan matchedSpan)
        {
            matchedSpan = null;

            var wordSpan = GetWordSpanAtMousePosition(this.view, this.navigatorService);
            if (wordSpan == null) return false;

            var wordText = wordSpan.Value.GetText();
            matchedSpan = stringItems.FirstOrDefault(t => t.Value.Contains(wordText));
            return matchedSpan != null;
        }

        static SnapshotSpan? GetWordSpanAtMousePosition(IWpfTextView view, ITextStructureNavigator navigatorService)
        {
            // ref. https://github.com/NoahRic/GoToDef/blob/master/GoToDefMouseHandler.cs
            var position = Mouse.GetPosition(view.VisualElement);
            var line = view.TextViewLines.GetTextViewLineContainingYCoordinate(position.Y + view.ViewportTop);
            if (line == null) return null;
            var bufferPosition = line.GetBufferPositionFromXCoordinate(position.X + view.ViewportLeft);
            if (bufferPosition == null) return null;
            return navigatorService.GetExtentOfWord(bufferPosition.Value).Span;
        }
    }
}
