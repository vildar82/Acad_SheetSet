namespace Acad_SheetSet.Utils
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using Autodesk.AutoCAD.ApplicationServices;
    using static Autodesk.AutoCAD.ApplicationServices.Core.Application;

    public static class AcadHelper
    {
        public static Document Doc => DocumentManager.MdiActiveDocument;

        public static Document GetOpenedDocument(string file)
        {
            return DocumentManager.Cast<Document>().FirstOrDefault(d =>
                Path.GetFullPath(d.Name).Equals(Path.GetFullPath(file), StringComparison.OrdinalIgnoreCase));
        }

        public static void ShowMessage(
            string message,
            string caption = "Acad_SheetSet",
            MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
        }

        public static void ShowMessage(this Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        public static string GetUserPluginFile(string name)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "Acad_SheetSet", name);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool EqualsIgnoreCase(this string string1, string string2)
        {
            return string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase) ||
                   IsBothStringsIsNullOrEmpty(string1, string2);
        }

        public static bool IsBothStringsIsNullOrEmpty(this string s1, string s2)
        {
            return string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2);
        }
    }
}