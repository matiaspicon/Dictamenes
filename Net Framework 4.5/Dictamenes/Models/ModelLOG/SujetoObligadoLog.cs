using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dictamenes.Models
{
    
    [Table("SujetosObligadosLog")]
    public class SujetoObligadoLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long CuilCuit { get; set; }

        [MaxLength(50, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string Nombre { get; set; }

        [MaxLength(50, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string Apellido { get; set; }

        [DisplayName("Razon Social")]
        [MaxLength(80, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string RazonSocial { get; set; }

        [DisplayName("Tipo de Sujeto Obligado")]
        [ForeignKey(nameof(TipoSujetoObligado))]
        public int IdTipoSujetoObligado { get; set; }

        [DisplayName("Tipo de Sujeto Obligado")]
        public TipoSujetoObligado TipoSujetoObligado { get; set; }

        // valores necesarios para el borrado logico
        public int Guid { get; set; }

        public DateTime FechaModificacion { get; set; }

        [ForeignKey("UsuarioModificacion")]
        public int IdUsuarioModificacion { get; set; }

        public Usuario UsuarioModificacion { get; set; }


    }
}