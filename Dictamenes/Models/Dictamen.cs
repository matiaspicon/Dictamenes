using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dictamenes.Models
{
    public class Dictamen
    {
        [Key]
        public int id { get; set; }
        
        [Required]
        [MaxLength(30, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string NroGDE { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string NroExpediente { get; set; }

        [Required]
        public DateTime FechaCarga { get; set; }

        public string Detalle { get; set; }

        public bool EsPublico { get; set; }

        [ForeignKey(nameof(ArchivoPDF))]
        public int IdArchivoLigado { get; set; }
        public ArchivoPDF ArchivoLigado { get; set; }

        [ForeignKey(nameof(SujetoObligado))]
        public int IdSujetoObligado { get; set; }
        public SujetoObligado SujetoObligado { get; set; }

        [ForeignKey(nameof(Asunto))]
        public int IdAsunto { get; set; }
        public Asunto Asunto { get; set; }

        [ForeignKey(nameof(TipoDictamen))]
        public int IdTipoDictamen { get; set; }
        public TipoDictamen TipoDictamen { get; set; }

        [ForeignKey(nameof(Usuario))]
        public int IdUsuarioGenerador { get; set; }
        public Usuario UsuarioGenerador { get; set; }


        // valores necesarios para el borrado logico

        public bool EstaActivo { get; set; }

        public DateTime FechaModificacion { get; set; }

        [ForeignKey(nameof(Usuario))]
        public int IdUsuarioModificacion { get; set; }
        public Usuario UsuarioModificacion { get; set; }






    }

}
