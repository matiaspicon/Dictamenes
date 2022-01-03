using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictamenes.Models
{

    [Table("SujetosControladosLog")]
    public class SujetoControladoLog
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

        [DisplayName("Tipo de Sujeto Controlado")]
        [ForeignKey(nameof(TipoSujetoControlado))]
        public int IdTipoSujetoControlado { get; set; }

        [DisplayName("Tipo de Sujeto Controlado")]
        public TipoSujetoControlado TipoSujetoControlado { get; set; }

        public bool EstaHabilitado { get; set; }

        public int IdOriginal { set; get; }
        public DateTime FechaModificacion { get; set; }


        public int? IdUsuarioModificacion { get; set; }




    }
}