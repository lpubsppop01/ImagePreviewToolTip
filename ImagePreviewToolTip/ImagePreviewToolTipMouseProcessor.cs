using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace lpubsppop01.ImagePreviewToolTipVSIX
{
    [Export(typeof(IMouseProcessorProvider))]
    [ContentType("Code")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("ImagePreviewToolTip")]
    internal sealed class ImagePreviewToolTipMouseProcessorProvider : IMouseProcessorProvider
    {
        [Import]
        public ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        public IToolTipProviderFactory ToolTipProviderFactory { get; set; }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            var navigator = NavigatorService.GetTextStructureNavigator(wpfTextView.TextBuffer);
            ITextDocument textDocument;
            TextDocumentFactoryService.TryGetTextDocument(wpfTextView.TextBuffer, out textDocument);
            return new ImagePreviewToolTipMouseProcessor(wpfTextView, navigator, ToolTipProviderFactory, textDocument);
        }
    }

    class ImagePreviewToolTipMouseProcessor : MouseProcessorBase
    {
        readonly StringSpanSearcher spanSearcher;
        readonly IToolTipProvider toolTipProvider;
        readonly ITextDocument textDocument;

        Span? shownSpan;

        public ImagePreviewToolTipMouseProcessor(IWpfTextView view, ITextStructureNavigator navigator,
            IToolTipProviderFactory toolTipProviderFactory, ITextDocument textDocument)
        {
            this.spanSearcher = new StringSpanSearcher(view, navigator);
            this.toolTipProvider = toolTipProviderFactory.GetToolTipProvider(view);
            this.textDocument = textDocument;

            this.shownSpan = null;
        }

        public override void PreprocessMouseMove(MouseEventArgs e)
        {
            if (!ImagePreviewToolTipSettings.Current.IsEnabled)
            {
                ClearToolTipIfShown();
                return;
            }

            var foundSpan = spanSearcher.GetStringSpanAtMousePosition();
            if (foundSpan == null)
            {
                ClearToolTipIfShown();
                return;
            }

            ShowToolTipIfNotShown(foundSpan);
        }

        void ClearToolTipIfShown()
        {
            if (shownSpan == null) return;
            this.shownSpan = null;
            this.toolTipProvider.ClearToolTip();
        }

        void ShowToolTipIfNotShown(StringSpan targetSpan)
        {
            if (this.shownSpan == targetSpan.Span) return;
            this.shownSpan = targetSpan.Span;

            string basePath = (textDocument != null) ? Path.GetDirectoryName(textDocument.FilePath) : "";
            var content = CreateToolTipContent(targetSpan.Value, basePath);
            if (content == null) return;

            this.toolTipProvider.ClearToolTip();
            this.toolTipProvider.ShowToolTip(
                targetSpan.LineSnapshotSpan.Snapshot.CreateTrackingSpan(targetSpan.Span, SpanTrackingMode.EdgeExclusive),
                content, PopupStyles.PositionClosest);
        }

        static UIElement CreateToolTipContent(string imageUrl, string basePath)
        {
            var imageSource = ImageLoader.Load(imageUrl, basePath);
            if (imageSource == null) return null;

            var workingArea = System.Windows.Forms.Screen.GetWorkingArea(System.Windows.Forms.Cursor.Position);
            double maxWidth = Math.Min(workingArea.Width * 0.9, imageSource.Width);
            double maxHeight = Math.Min(workingArea.Height * 0.45, imageSource.Height);

            var image = new Image { MaxWidth = maxWidth, MaxHeight = maxHeight };
            image.Source = imageSource;
            var imagePanel = new Grid();
            imagePanel.Children.Add(image);
            var content = new Border
            {
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Child = imagePanel
            };
            return content;
        }
    }
}
