using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dictamenes.Models
{
    public class FileOnFileSystemModel : FileModel
    {
        public string FilePath { get; set; }
    }
}
