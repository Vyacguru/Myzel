﻿using Avalonia.Input;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = System.Range;
using TextDocument = AvaloniaEdit.Document.TextDocument;

namespace Myzel.GUI.Essentials.Extensions
{
    public static class TextEditorExtensions
    {
        public static string? GetWordAtPointerPosition(this TextEditor editor, PointerEventArgs e)
        {
            TextViewPosition? mousePosition = editor.GetPositionFromPoint(e.GetPosition(editor));

            if (mousePosition == null)
                return string.Empty;

            int offset = editor.Document.GetOffset(mousePosition.Value.Location);

            return editor.GetWordAtOffset(offset);
        }

        public static string? GetWordAtOffset(this TextEditor editor, int offset)
        {
            if (editor.Document == null) return null;
            if (offset >= editor.Document.TextLength)
                offset--;

            int offsetStart = TextUtilities.GetNextCaretPosition(editor.Document, offset, LogicalDirection.Backward,
                CaretPositioningMode.WordBorder);
            int offsetEnd = TextUtilities.GetNextCaretPosition(editor.Document, offset, LogicalDirection.Forward,
                CaretPositioningMode.WordBorder);

            if (offsetEnd == -1 || offsetStart == -1)
                return string.Empty;

            string? currentChar = editor.Document.GetText(offset, 1);

            return string.IsNullOrWhiteSpace(currentChar) ? string.Empty : editor.Document.GetText(offsetStart, offsetEnd - offsetStart);

        }

        public static Range? GetWordRangeAtPointerPosition(this TextEditor editor, PointerEventArgs e)
        {
            TextViewPosition? mousePosition = editor.GetPositionFromPoint(e.GetPosition(editor));

            if (mousePosition == null)
                return null;

            int offset = editor.Document.GetOffset(mousePosition.Value.Location);

            return editor.GetWordRangeAtOffset(offset);
        }

        public static Range? GetWordRangeAtOffset(this TextEditor editor, int offset)
        {
            if (editor.Document == null) return null;
            if (offset >= editor.Document.TextLength)
                offset--;

            int offsetStart = TextUtilities.GetNextCaretPosition(editor.Document, offset, LogicalDirection.Backward,
                CaretPositioningMode.WordBorder);
            int offsetEnd = TextUtilities.GetNextCaretPosition(editor.Document, offset, LogicalDirection.Forward,
                CaretPositioningMode.WordBorder);

            if (offsetStart < 0 || offsetEnd < 0) return null;
            return new Range(offsetStart, offsetEnd);
        }

        public static int GetOffsetFromPointerPosition(this TextEditor editor, PointerEventArgs e)
        {
            TextViewPosition? pos = editor.GetPositionFromPoint(e.GetPosition(editor)); //gets position of mouse
            return pos.HasValue ? editor.Document.GetOffset(pos.Value.Location) : -1;
        }

        public static int GetOffsetFromPosition(this TextDocument doc, Position pos)
        {
            int line = pos.Line + 1;
            int column = pos.Character + 1;
            if (line > doc.LineCount || line < 1) return doc.TextLength;
            DocumentLine? docline = doc.GetLineByNumber(line);
            if (docline.Offset + column > doc.TextLength) return doc.TextLength;
            return docline.Offset + column;
        }

        public static Position GetPositionFromOffset(this TextDocument doc, int offset)
        {
            if (offset >= doc.TextLength) offset = doc.TextLength - 1;
            TextLocation loc = doc.GetLocation(offset);
            return new Position(loc.Line - 1, loc.Column - 1);
        }
    }
}