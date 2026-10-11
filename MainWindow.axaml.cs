using Avalonia.Controls;
using Avalonia.Interactivity;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Avalonia.Platform.Storage;
using System;

namespace PdfToolBoxMac
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        // 1. JUNTAR MÚLTIPLOS PDFs
        private async void BtnJuntar_Click(object sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Selecione os PDFs para juntar",
                AllowMultiple = true,
                FileTypeFilter = new[] { new FilePickerFileType("PDFs") { Patterns = new[] { "*.pdf" } } }
            });

            if (files != null && files.Count > 1)
            {
                var saveFile = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Salvar PDF Unificado",
                    DefaultExtension = "pdf",
                    SuggestedFileName = "PDF_Unificado.pdf"
                });

                if (saveFile != null)
                {
                    try
                    {
                        using (PdfDocument outputDocument = new PdfDocument())
                        {
                            foreach (var file in files)
                            {
                                using (var stream = await file.OpenReadAsync())
                                {
                                    using (PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import))
                                    {
                                        for (int i = 0; i < inputDocument.PageCount; i++)
                                        {
                                            outputDocument.AddPage(inputDocument.Pages[i]);
                                        }
                                    }
                                }
                            }
                            outputDocument.Save(saveFile.Path.LocalPath);
                        }
                        TxtStatus.Text = "PDFs unidos com sucesso!";
                    }
                    catch
                    {
                        TxtStatus.Text = "Erro ao unir PDFs.";
                    }
                }
            }
        }

        // 2. DIVIDIR PDF EM PÁGINAS INDIVIDUAIS
        private async void BtnDividir_Click(object sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Selecione o PDF para dividir",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("PDFs") { Patterns = new[] { "*.pdf" } } }
            });

            if (files != null && files.Count > 0)
            {
                string inputPath = files[0].Path.LocalPath;
                string outputFolder = Path.GetDirectoryName(inputPath)!;

                try
                {
                    using (PdfDocument inputDocument = PdfReader.Open(inputPath, PdfDocumentOpenMode.Import))
                    {
                        for (int i = 0; i < inputDocument.PageCount; i++)
                        {
                            using (PdfDocument newDoc = new PdfDocument())
                            {
                                newDoc.AddPage(inputDocument.Pages[i]);
                                string pageFileName = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(inputPath)}_Pagina_{i + 1}.pdf");
                                newDoc.Save(pageFileName);
                            }
                        }
                    }
                    TxtStatus.Text = "PDF dividido com sucesso na pasta do original!";
                }
                catch
                {
                    TxtStatus.Text = "Erro ao dividir PDF.";
                }
            }
        }

        // 3. REMOVER A ÚLTIMA PÁGINA DO PDF
        private async void BtnRemover_Click(object sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Selecione o PDF para remover página",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("PDFs") { Patterns = new[] { "*.pdf" } } }
            });

            if (files != null && files.Count > 0)
            {
                string inputPath = files[0].Path.LocalPath;

                var saveFile = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Salvar PDF Editado",
                    DefaultExtension = "pdf",
                    SuggestedFileName = "PDF_Editado.pdf"
                });

                if (saveFile != null)
                {
                    try
                    {
                        using (PdfDocument inputDocument = PdfReader.Open(inputPath, PdfDocumentOpenMode.Import))
                        {
                            using (PdfDocument outputDocument = new PdfDocument())
                            {
                                int totalPages = inputDocument.PageCount;
                                for (int i = 0; i < totalPages - 1; i++)
                                {
                                    outputDocument.AddPage(inputDocument.Pages[i]);
                                }
                                outputDocument.Save(saveFile.Path.LocalPath);
                            }
                        }
                        TxtStatus.Text = "Última página removida com sucesso!";
                    }
                    catch
                    {
                        TxtStatus.Text = "Erro ao processar página.";
                    }
                }
            }
        }

        // 4. BOTÃO DE INFO
        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            TxtStatus.Text = "Para converter Word, utilize o LibreOffice no Mac.";
        }
    }
}