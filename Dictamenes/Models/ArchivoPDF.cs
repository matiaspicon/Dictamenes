using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dictamenes.Models
{
    public class ArchivoPDF
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoArchivo { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public DateTime FechaCarga { get; set; }

        public string Contenido { get; set; }

    }
}