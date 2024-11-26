using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Diagnostics;
using System.Windows;
using CelltechFinal.Models;

namespace CelltechFinal.Helpers
{
    public static class ComprobanteHelper
    {
        public static void GenerarComprobantePDF(Venta venta)
        {
            string fileName = $"Comprobante_{venta.VentaID}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Comprobantes",
                fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, fs);

                document.Open();

                // Agregar logo
                try
                {
                    string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.png");
                    if (File.Exists(logoPath))
                    {
                        Image logo = Image.GetInstance(logoPath);
                        logo.ScaleToFit(100f, 100f);
                        logo.Alignment = Element.ALIGN_CENTER;
                        document.Add(logo);
                    }
                }
                catch { /* Continuar sin logo */ }

                // Título
                Paragraph titulo = new Paragraph("COMPROBANTE DE VENTA",
                    new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                titulo.SpacingAfter = 20f;
                document.Add(titulo);

                // Información de la venta
                document.Add(new Paragraph($"Nº de Venta: {venta.VentaID}",
                    new Font(Font.FontFamily.HELVETICA, 12)));
                document.Add(new Paragraph($"Fecha: {venta.Fecha:dd/MM/yyyy HH:mm:ss}",
                    new Font(Font.FontFamily.HELVETICA, 12)));
                document.Add(new Paragraph("\n"));

                // Tabla de productos
                PdfPTable tabla = new PdfPTable(5);
                tabla.WidthPercentage = 100;
                tabla.SetWidths(new float[] { 2f, 4f, 1f, 1.5f, 1.5f });

                // Encabezados
                string[] headers = { "Código", "Producto", "Cant.", "Precio Unit.", "Subtotal" };
                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header,
                        new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                    cell.BackgroundColor = new BaseColor(240, 240, 240);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 8;
                    tabla.AddCell(cell);
                }

                // Detalles
                foreach (var detalle in venta.Detalles)
                {
                    tabla.AddCell(new PdfPCell(new Phrase(detalle.Codigo)));
                    tabla.AddCell(new PdfPCell(new Phrase(detalle.Nombre)));
                    tabla.AddCell(new PdfPCell(new Phrase(detalle.Cantidad.ToString()))
                    { HorizontalAlignment = Element.ALIGN_CENTER });
                    tabla.AddCell(new PdfPCell(new Phrase(detalle.PrecioUnitario.ToString("C2")))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });
                    tabla.AddCell(new PdfPCell(new Phrase(detalle.Subtotal.ToString("C2")))
                    { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(tabla);

                // Total
                Paragraph total = new Paragraph($"\nTOTAL: {venta.Total:C2}",
                    new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                total.Alignment = Element.ALIGN_RIGHT;
                document.Add(total);

                // Pie de página
                Paragraph footer = new Paragraph("\n¡Gracias por su compra!",
                    new Font(Font.FontFamily.HELVETICA, 12));
                footer.Alignment = Element.ALIGN_CENTER;
                document.Add(footer);

                document.Close();
            }

            // Abrir el PDF
            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch
            {
                MessageBox.Show($"El comprobante se ha guardado en:\n{path}",
                    "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}