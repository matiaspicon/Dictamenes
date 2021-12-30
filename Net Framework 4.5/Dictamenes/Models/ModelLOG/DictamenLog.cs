using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictamenes.Models
{
    [Table("DictamenesLog")]
    public class DictamenLog
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Número de GDE")]
        [RegularExpression("[iI][fF]-[0-9]{4}-[0-9]+-[aA][pP][nN]-[A-Za-z]+#[A-Za-z]+",
         ErrorMessage = "El Número de GDE ingresado no es valido.")]
        [MaxLength(30, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string NroGDE { get; set; }

        [Required]
        [DisplayName("Número de Expediente")]
        [RegularExpression("[eE][xX]-[0-9]{4}-[0-9]+-[aA][pP][nN]-[A-Za-z]+#[A-Za-z]+",
         ErrorMessage = "El Número de Expediente ingresado no es valido.")]
        [MaxLength(30, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string NroExpediente { get; set; }

        [DataType(DataType.DateTime), Required]
        [DisplayName("Fecha de Carga")]
        public DateTime FechaCarga { get; set; }

        public string Detalle { get; set; }

        [DisplayName("Es Público")]
        public bool EsPublico { get; set; }

        [ForeignKey(nameof(ArchivoPDF))]
        public int? IdArchivoPDF { get; set; }
        public ArchivoPDF ArchivoPDF { get; set; }

        [ForeignKey(nameof(SujetoControlado))]
        public int? IdSujetoControlado { get; set; }
        public SujetoControlado SujetoControlado { get; set; }

        [ForeignKey(nameof(Asunto))]
        public int IdAsunto { get; set; }
        public Asunto Asunto { get; set; }

        [ForeignKey(nameof(TipoDictamen))]
        public int? IdTipoDictamen { get; set; }
        public TipoDictamen TipoDictamen { get; set; }
        public int IdOriginal { set; get; }

        public bool Borrado { get; set; }

        public bool HaySujetoControlado { get; set; }
        public DateTime FechaModificacion { get; set; }


        public int? IdUsuarioModificacion { get; set; }



    }

}
