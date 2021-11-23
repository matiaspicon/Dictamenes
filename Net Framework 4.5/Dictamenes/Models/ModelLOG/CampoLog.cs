
using System;
using System.ComponentModel.DataAnnotations;

namespace Dictamenes.Models
{
    public abstract class CampoLog
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string Descripcion { get; set; }

        public bool EstaHabilitado { get; set; }

        public int IdOriginal { get; set; }

        public DateTime FechaModificacion { get; set; }


        public int? IdUsuarioModificacion { get; set; }



    }
}