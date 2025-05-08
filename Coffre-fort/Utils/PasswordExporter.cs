using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Coffre_fort.View_model;
using Microsoft.Win32;
using OfficeOpenXml;

namespace Coffre_fort.Utils
{
    public static class PasswordExporter
    {
        public static void Export(ObservableCollection<PasswordEntry> passwords, string format)
        {
            if (passwords == null || passwords.Count == 0)
            {
                System.Windows.MessageBox.Show("Aucune donnée à exporter.");
                return;
            }

            var dialog = new SaveFileDialog();
            string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            switch (format.ToUpper())
            {
                case "CSV":
                    dialog.Filter = "Fichier CSV (*.csv)|*.csv";
                    dialog.FileName = $"export_{date}.csv";
                    if (dialog.ShowDialog() == true)
                    {
                        ExportCsv(passwords, dialog.FileName);
                        System.Windows.MessageBox.Show("Export CSV réussi :\n" + dialog.FileName);
                    }
                    break;

                case "XML":
                    dialog.Filter = "Fichier XML (*.xml)|*.xml";
                    dialog.FileName = $"export_{date}.xml";
                    if (dialog.ShowDialog() == true)
                    {
                        ExportXml(passwords, dialog.FileName);
                        System.Windows.MessageBox.Show("Export XML réussi :\n" + dialog.FileName);
                    }
                    break;

                case "EXCEL":
                    dialog.Filter = "Fichier Excel (*.xlsx)|*.xlsx";
                    dialog.FileName = $"export_{date}.xlsx";
                    if (dialog.ShowDialog() == true)
                    {
                        ExportExcel(passwords, dialog.FileName);
                        System.Windows.MessageBox.Show("Export Excel réussi :\n" + dialog.FileName);
                    }
                    break;

                default:
                    System.Windows.MessageBox.Show("Format non supporté.");
                    break;
            }
        }

        private static void ExportCsv(ObservableCollection<PasswordEntry> list, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("NomCompte;MotDePasse;DateAjout;Tags");

            foreach (var p in list)
            {
                var mdp = SecurityHelper.Decrypt(p.MotDePasse);
                var dateFormatee = p.DateAjout.ToString("yyyy-MM-dd HH:mm:ss");
                sb.AppendLine($"{p.NomCompte};{mdp};{dateFormatee};{p.Tags}");
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        private static void ExportXml(ObservableCollection<PasswordEntry> list, string path)
        {
            var serializer = new XmlSerializer(typeof(ObservableCollection<PasswordEntry>));
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, list);
        }

        private static void ExportExcel(ObservableCollection<PasswordEntry> list, string path)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("MotsDePasse");

            sheet.Cells[1, 1].Value = "NomCompte";
            sheet.Cells[1, 2].Value = "MotDePasse";
            sheet.Cells[1, 3].Value = "DateAjout";
            sheet.Cells[1, 4].Value = "Tags";

            int row = 2;
            foreach (var p in list)
            {
                sheet.Cells[row, 1].Value = p.NomCompte;
                sheet.Cells[row, 2].Value = SecurityHelper.Decrypt(p.MotDePasse);
                sheet.Cells[row, 3].Value = p.DateAjout;
                sheet.Cells[row, 3].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                sheet.Cells[row, 4].Value = p.Tags;
                row++;
            }

            package.SaveAs(new FileInfo(path));
        }
    }
}
