using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dictamenes.Database;
using Dictamenes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UglyToad.PdfPig;

namespace Dictamenes.Controllers
{
    public class FileController : Controller
    {
        private readonly DictamenesDbContext context;

        public FileController(DictamenesDbContext context)
        {
            this.context = context;
        }
        

        [HttpGet]
        public async Task<IActionResult> DownloadFile(int id)
        {

            var file = await context.ArchivoPDF.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return NotFound();
            if (!System.IO.File.Exists(file.Path)) return NotFound();
            var memory = new MemoryStream();
            using (var stream = new FileStream(file.Path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, file.TipoArchivo);
        }
        public async Task<IActionResult> DeleteFileFromFileSystem(int id)
        {

            var file = await context.ArchivoPDF.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (file == null) return null;
            if (System.IO.File.Exists(file.Path))
            {
                System.IO.File.Delete(file.Path);
            }
            context.ArchivoPDF.Remove(file);
            context.SaveChanges();
            return Ok(file);
        }

        public static string ExtractTextFromPdf(string path)
        {
            PdfDocument document = PdfDocument.Open(path);
            var contenido = " ";
            for (var i = 0; i < document.NumberOfPages; i++)
            {
                // This starts at 1 rather than 0.
                var page = document.GetPage(i + 1);

                foreach (var word in page.GetWords())
                {
                    contenido += ' ' + word.Text.ToUpper();
                }
            }

            return contenido;
        }
    }
}
