using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Dictamenes.Models
{
    public abstract class BorradoLogico
    {
        [Key]
        public int Id { get; set; }

        public bool EstaActivo { get; set; }

        public DateTime FechaModificacion { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuarioModificacion { get; set; }
        public Usuario UsuarioModificacion { get; set; }

    }
}
