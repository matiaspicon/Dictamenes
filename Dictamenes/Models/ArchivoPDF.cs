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

        public string path { get; set; }

        public string contenido { get; set; }



    }
}