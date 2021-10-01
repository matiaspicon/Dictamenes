using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dictamenes.Database;
using Dictamenes.Models;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Web.Hosting;

namespace Dictamenes.Controllers
{
    public class FileController : Controller
    {
        private DictamenesDbContext db = new DictamenesDbContext();

        public FileController() { }

        [HttpGet]
        public async Task<ActionResult> DownloadFile(int id)
        {


            var file = await db.ArchivosPDF.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return HttpNotFound();
            string pathAbsolute = Server.MapPath(file.Path);
            if (!System.IO.File.Exists(pathAbsolute)) return HttpNotFound();
            var memory = new MemoryStream();
            using (var stream = new FileStream(Server.MapPath(file.Path), FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, file.TipoArchivo);
        }


        //public async Task<ActionResult> DeleteFileFromFileSystem(int id)
        //{

        //    var file = await db.ArchivoPDF.Where(x => x.Id == id).FirstOrDefaultAsync();
        //    if (file == null) return null;
        //    if (System.IO.File.Exists(file.Path))
        //    {
        //        System.IO.File.Delete(file.Path);
        //    }
        //    db.ArchivoPDF.Remove(file);
        //    db.SaveChanges();
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
            string pathAbsolute = HostingEnvironment.MapPath(path);
            PdfReader reader2 = new PdfReader(pathAbsolute);
            string strText = string.Empty;

            for (int page = 1; page <= reader2.NumberOfPages; page++)
            {
                ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
                PdfReader reader = new PdfReader(pathAbsolute);
                String s = PdfTextExtractor.GetTextFromPage(reader, page, its);

                s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));
                strText += s;
                reader.Close();
            }
            return strText;
        }
    }
}
