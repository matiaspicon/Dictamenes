using Dictamenes.Database;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

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


        public static string ExtractTextFromPdf(string path)
        {
            string pathAbsolute = HostingEnvironment.MapPath(path);
            string strText = String.Empty;
            try
            {
                PdfReader reader2 = new PdfReader(pathAbsolute);
                strText = string.Empty;

                for (int page = 1; page <= reader2.NumberOfPages; page++)
                {
                    ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
                    PdfReader reader = new PdfReader(pathAbsolute);
                    String s = PdfTextExtractor.GetTextFromPage(reader, page, its);

                    s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));
                    strText += s;
                    reader.Close();
                }
            }
            catch
            {

            }

            return strText;
        }
    }
}
