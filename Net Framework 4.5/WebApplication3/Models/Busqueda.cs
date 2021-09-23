  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dictamenes.Models
{
    public class Busqueda
    {
        public string NroGDE { get; set; }
        
        public string NroExp { get; set; }

        public DateTime? FechaCargaInicio { get; set; }

        public DateTime? FechaCargaFinal { get; set; }

        public string Contenido { get; set; }
        
        public string Detalle { get; set; }

        public int? IdAsunto { get; set; }

        public int? IdTipoSujetoObligado { get; set; }

        public int? IdTipoDictamen { get; set; }

        public bool EsDenunciante { get; set; }

        public int? IdSujetoObligado { get; set; }

        public int? CuilCuit { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

    }
}
