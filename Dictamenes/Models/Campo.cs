using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dictamenes.Models
{
    public abstract class Campo
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Descripcion")]
        [MaxLength(50, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string Descripcion { get; set; }

        [DisplayName("Esta Habilitado")]
        public bool EstaHabilitado { get; set; }

        [DisplayName("Esta Activo")]
        [HiddenInput(DisplayValue = false)]
        public bool EstaActivo { get; set; }

        [DisplayName("Fecha de Modificacion")]
        [HiddenInput(DisplayValue = false)]
        public DateTime FechaModificacion { get; set; }


        [HiddenInput(DisplayValue = false)]
        [ForeignKey("Usuario")]
        public int IdUsuarioModificacion { get; set; }
        public Usuario UsuarioModificacion { get; set; }

    }
}