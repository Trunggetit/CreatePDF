using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CombinePDF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public TypeOfTransaction _task = TypeOfTransaction.BEGIN;
        public string _ext = "";
        public readonly string PDF = "pdf";
        public readonly string JPG = "jpg";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            _task = TypeOfTransaction.PDF;

            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult dag = dialog.ShowDialog();
            //// Set filter for file extension and default file extension 

            if(dag == System.Windows.Forms.DialogResult.OK)
            {
                txtPDF.Text = dialog.SelectedPath;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                txtSave.Text = saveFileDialog.FileName + ".pdf";

            }
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            if(cboTypes.SelectedIndex > -1)
            {
                int type = cboTypes.SelectedIndex;
                if (!string.IsNullOrEmpty(txtSave.Text))
                {
                    switch (type)
                    {
                        case 0:
                            _task = TypeOfTransaction.JPG;
                            CompileResults(txtPDF.Text, txtSave.Text, JPG);
                            break;
                        case 1:
                            _task = TypeOfTransaction.PDF;
                            CompileResults(txtPDF.Text, txtSave.Text, PDF);
                            break;
                    }

                }
            }

        }

        public void CompileResults(string folderLocation, string destination, string type)
        {
            string[] pdfFiles = Directory.GetFiles(folderLocation, "*." + type)
                                     .Select(System.IO.Path.GetFullPath)
                                     .ToArray();
            switch(_task)
            {
                case TypeOfTransaction.JPG:

                    bool isPrinted = PrintJPG(pdfFiles.AsEnumerable().ToList<string>(), destination);

                    if(isPrinted)
                    {
                        MessageBox.Show("Completed " + "Please check this folder " + destination);
                    }

                    break;
                case TypeOfTransaction.PDF:

                    bool isPrintedPDF = MergePDFs(pdfFiles.AsEnumerable().ToList<string>(), destination);

                    if (isPrintedPDF)
                    {
                        MessageBox.Show("Completed " + "Please check this folder " + destination);
                    }


                    break;
            }
        }

        public bool MergePDFs(List<string> InFiles, string OutFile)
        {
            bool merged = true;
            try
            {
                List<PdfReader> readerList = new List<PdfReader>();
                foreach (string filePath in InFiles)
                {
                    PdfReader pdfReader = new PdfReader(filePath);
                    readerList.Add(pdfReader);
                }

                //Define a new output document and its size, type
                Document document = new Document(PageSize.A4, 0, 0, 0, 0);
                //Create blank output pdf file and get the stream to write on it.
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(OutFile, FileMode.Create));
                document.Open();

                foreach (PdfReader reader in readerList)
                {
                    PdfReader.unethicalreading = true;
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.Add(iTextSharp.text.Image.GetInstance(page));
                    }
                }
                document.Close();
                foreach (PdfReader reader in readerList)
                {
                    reader.Close();
                }

            }
            catch (Exception ex)
            {
                merged = false;
            }


            return merged;
        }

        public bool PrintJPG(List<string> InFiles, string OutFile)
        {
            Document document = new Document();
            using (var stream = new FileStream(OutFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {

                PdfWriter.GetInstance(document, stream);
                document.Open();

                foreach(var st in InFiles)
                {
                    using (var imageStream = new FileStream(st, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var image = iTextSharp.text.Image.GetInstance(imageStream);
                        document.Add(image);
                    }
                }

                document.Close();
            }

            return true;
        }

    }

    public enum TypeOfTransaction { PDF, JPG, SAVE, GO, BEGIN };
}
