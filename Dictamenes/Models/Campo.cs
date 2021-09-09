using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictamenes.Models
{
    public abstract class Campo
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string Descripcion { get; set; }

        public bool EstaHabilitado { get; set; }

        public bool EstaActivo { get; set; }

        public DateTime FechaModificacion { get; set; }

        [ForeignKey(nameof(Usuario))]
        public int IdUsuarioModificacion { get; set; }
        public Usuario UsuarioModificacion { get; set; }

    }
}