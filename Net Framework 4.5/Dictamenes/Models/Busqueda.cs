
using System;

namespace Dictamenes.Models
{
    public class Busqueda
    {
        public bool Collapse { get; set; }
        public string NroGDE { get; set; }

        public string NroExp { get; set; }

        public DateTime? FechaCargaInicio { get; set; }

        public DateTime? FechaCargaFinal { get; set; }

        //public bool EsPublico { get; set; }

        public string Contenido { get; set; }

        public string Detalle { get; set; }

        public int? IdAsunto { get; set; }

        public int? IdTipoSujetoControlado { get; set; }

        public int? IdTipoDictamen { get; set; }

        public bool EsDenunciante { get; set; }

        public int? IdSujetoControlado { get; set; }

        public long? CuilCuit { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

    }
}
