using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dictamenes.Database;
using System.Web.Mvc;
using System.Data.Entity;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

namespace Dictamenes.Controllers
{
    public class FileController : Controller
    {
        private DictamenesDbContext _context = new DictamenesDbContext();

        public FileController() { }

        
        

        [HttpGet]
        public async Task<ActionResult> DownloadFile(int id)
        {

            var file = await _context.ArchivosPDF.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return HttpNotFound();
            if (!System.IO.File.Exists(file.Path)) return HttpNotFound();
            var memory = new MemoryStream();
            using (var stream = new FileStream(file.Path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, file.TipoArchivo);
        }

        //public async Task<ActionResult> DeleteFileFromFileSystem(int id)
        //{

        //    var file = await _context.ArchivoPDF.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    if (file == null) return null;
        //    if (System.IO.File.Exists(file.Path))
        //    {
        //        System.IO.File.Delete(file.Path);
        //    }
        //    _context.ArchivoPDF.Remove(file);
        //    _context.SaveChanges();
        //    return RedirectToAc
        //}

        //public static string ExtractTextFromPdf(string path)
        //{
        //    PdfDocument document = PdfDocument.Open(path);
        //    var contenido = " ";
        //    for (var i = 0; i < document.NumberOfPages; i++)
        //    {
        //        // This starts at 1 rather than 0.
        //        var page = document.GetPage(i + 1);

        //        foreach (var word in page.GetWords())
        //        {
        //            contenido += ' ' + word.Text.ToUpper();
        //        }
        //    }

        //    return contenido;
        //}


        public static string ExtractTextFromPdf(string path)
        {
            PdfReader reader2 = new PdfReader((string)path);
            string strText = string.Empty;

            for (int page = 1; page <= reader2.NumberOfPages; page++)
            {
                ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                PdfReader reader = new PdfReader((string)path);
                String s = PdfTextExtractor.GetTextFromPage(reader, page, its);

                s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));
                strText = strText + s;
                reader.Close();
            }
            return strText;
        }
    }
}
