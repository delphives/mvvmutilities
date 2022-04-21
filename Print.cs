using prj_Entity;
using prj_Entity.Models;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace WpfApplication.Utilities
{
    public class Print
    {
        /*public static void CreateDocumentFromTemplateNoFormat(Facture f, string template)
        {
            //chargement du document
            Document document = new Document();
            document.LoadFromFile(template);
            //remplacement des champs par les valeurs
            document.Replace("client_prenom", f.Client.Prenom, true, true);
            document.Replace("client_nom", f.Client.Nom, true, true);
            document.Replace("client_adresse", f.Client.Rue, true, true);
            document.Replace("client_npa", f.Client.Localite.Npa.ToString(), true, true);
            document.Replace("client_localite", f.Client.Localite.Nom, true, true);
            document.Replace("facture_date", f.DateFacture.ToString("dd.MM.yyyy"), true, true);
            document.Replace("facture_num", f.Numero, true, true);
            //la première section (élément 0) est celle ou l'on trouve un titre "Titre1"
            Spire.Doc.Section s = document.Sections[0];
            Table table = s.AddTable(true);
            String[] Header = { "N°", "Article", "Quantité", "Prix unitaire", "Total" };
            table.ResetCells(f.LigneFactures.Count + 1, Header.Length);
            //en-tête de table
            TableRow FRow = table.Rows[0];
            FRow.IsHeader = true;
            for (int i = 0; i < Header.Length; i++)
            {
                FRow.Cells[i].AddParagraph().AppendText(Header[i]);
            }

            decimal grandTotal = 0;
            List<LigneFacture> listLignesFactures = new List<LigneFacture>(f.LigneFactures);
            //lignes de facture
            for (int r = 0; r < listLignesFactures.Count; r++)
            {
                LigneFacture lf = listLignesFactures[r];
                TableRow DataRow = table.Rows[r + 1];
                //colonnes
                for (int c = 0; c < Header.Length; c++)
                {
                    Paragraph p2 = DataRow.Cells[c].AddParagraph();
                    decimal total = lf.Montant * lf.Quantite;
                    switch (c)
                    {
                        case 0:
                            p2.AppendText(lf.Article.Id.ToString());
                            break;
                        case 1:
                            p2.AppendText(lf.Article.Nom);
                            break;
                        case 2:
                            p2.AppendText(lf.Quantite.ToString());
                            break;
                        case 3:
                            p2.AppendText(lf.Montant.ToString());
                            break;
                        case 4:
                            p2.AppendText(total.ToString());
                            grandTotal += total;
                            break;
                        default:
                            Console.WriteLine("Erreur dans le numéro de colonne");
                            break;
                    }
                }

            }
            //TOTAL
            Paragraph pa = s.AddParagraph();
            pa.AppendText("\n");
            TextRange t = pa.AppendText($"TOTAL : {grandTotal.ToString()}");
            pa.Format.HorizontalAlignment = HorizontalAlignment.Right;
            //pdf
            document.SaveToFile("xxx.pdf", FileFormat.PDF);
            System.Diagnostics.Process.Start("xxx.pdf");
        }*/

        public static void CreateDocumentFromTemplateWithFormat(Facture f, string template)
        {
            Document document = new Document();
            document.LoadFromFile(template);
            document.Replace("client_prenom", f.Client.Prenom, true, true);
            document.Replace("client_nom", f.Client.Nom, true, true);
            document.Replace("client_adresse", f.Client.Rue, true, true);
            document.Replace("client_npa", f.Client.Localite.Npa.ToString(), true, true);
            document.Replace("client_localite", f.Client.Localite.Nom, true, true);
            document.Replace("facture_date", f.DateFacture.ToString("dd.MM.yyyy"), true, true);
            document.Replace("facture_num", f.Numero, true, true);

            //la première section est celle ou l'on trouve un titre "Titre1"
            Section s = document.Sections[0];
            Table table = s.AddTable(true);
            String[] Header = { "N°", "Article", "Quantité", "Prix unitaire", "Total" };
            table.ResetCells(f.LigneFactures.Count + 1, Header.Length);

            //Header Row
            TableRow FRow = table.Rows[0];
            FRow.IsHeader = true;
            //Row Height
            //FRow.Height = 18;
            FRow.Cells[0].Width = 20;
            FRow.Cells[1].Width = 150;
            FRow.Cells[2].Width = 60;
            FRow.Cells[3].Width = 80;
            FRow.Cells[4].Width = 40;
            //Header Format
            FRow.RowFormat.BackColor = Color.LightBlue;
            for (int i = 0; i < Header.Length; i++)
            {
                //Cell Alignment
                Paragraph p = FRow.Cells[i].AddParagraph();
                FRow.Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                //Data Format
                TextRange TR = p.AppendText(Header[i]);
                TR.CharacterFormat.FontName = "Calibri";
                TR.CharacterFormat.FontSize = 14;
                TR.CharacterFormat.TextColor = Color.Teal;
                TR.CharacterFormat.Bold = true;
            }

            decimal grandTotal = 0;
            List<LigneFacture> listLignesFactures = new List<LigneFacture>(f.LigneFactures);
            //Data Row
            for (int r = 0; r < listLignesFactures.Count; r++)
            {
                LigneFacture lf = listLignesFactures[r];
                TableRow DataRow = table.Rows[r + 1];

                //Row Height
                DataRow.Height = 15;

                //C Represents Column. 5 -> nombre de colonnes
                for (int c = 0; c < 5; c++)
                {
                    //Cell Alignment
                    DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                    //Fill Data in Rows
                    Paragraph p2 = DataRow.Cells[c].AddParagraph();
                    TextRange TR2 = null;
                    decimal total = lf.Montant * lf.Quantite;
                    switch (c)
                    {
                        case 0:
                            TR2 = p2.AppendText(lf.Article.Id.ToString());
                            break;
                        case 1:
                            TR2 = p2.AppendText(lf.Article.Nom);
                            break;
                        case 2:
                            TR2 = p2.AppendText(lf.Quantite.ToString());
                            break;
                        case 3:
                            TR2 = p2.AppendText(lf.Montant.ToString());
                            break;
                        case 4:
                            TR2 = p2.AppendText(total.ToString());
                            grandTotal += total;
                            break;
                        default:
                            Console.WriteLine("Erreur dans le numéro de colonne");
                            break;
                    }

                    //Format Cells
                    p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                    TR2.CharacterFormat.FontName = "Calibri";
                    TR2.CharacterFormat.FontSize = 12;
                    TR2.CharacterFormat.TextColor = Color.Brown;
                }

            }
            //TOTAL
            Paragraph pa = s.AddParagraph();
            pa.AppendText("\n");
            TextRange t = pa.AppendText($"TOTAL : {f.TotalFacture.ToString()}");
            pa.Format.HorizontalAlignment = HorizontalAlignment.Right;
            t.CharacterFormat.FontName = "Calibri";
            t.CharacterFormat.FontSize = 16;
            t.CharacterFormat.TextColor = Color.SteelBlue;

            document.SaveToFile("xxx.pdf", FileFormat.PDF);
            //ok pour .net framework
            //System.Diagnostics.Process.Start("xxx.pdf");

            //ok pour .net core
            var pr = new Process();
            pr.StartInfo = new ProcessStartInfo(@"xxx.pdf")
            {
                UseShellExecute = true
            };
            pr.Start();

            //document.SaveToFile("SpireDocTestModified.docx", FileFormat.Docx2013);
        }
    }
}
