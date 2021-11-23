
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dictamenes.Models
{
    public abstract class Campo
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        [DisplayName("Decripción")]
        public string Descripcion { get; set; }

        [DisplayName("Esta habilitado")]
        public bool EstaHabilitado { get; set; }

        public DateTime FechaModificacion { get; set; }


        public int? IdUsuarioModificacion { get; set; }



    }
}