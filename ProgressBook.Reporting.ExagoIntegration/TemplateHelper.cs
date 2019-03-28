namespace ProgressBook.Reporting.ExagoIntegration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DocumentFormat.OpenXml.Packaging;
    using DocumentFormat.OpenXml.Wordprocessing;

    public static class TemplateHelper
    {
        public static IEnumerable<string> ReadBookmarksFromWordDocument(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            IList<string> list = new List<string>();

            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);

                using (WordprocessingDocument document = WordprocessingDocument.Open(stream, false))
                {
                    foreach (var bookmarkStart in document.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                    {
                        if (bookmarkStart.Name != "_GoBack")
                        {
                            list.Add(bookmarkStart.Name);
                        }
                    }
                }
            }

            return list;
        }
    }
}