 
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Dictamenes.Models
{
    public abstract class Campo
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "{0} admite un máximo de {1} caracteres")]
        public string Descripcion { get; set; }

        public bool EstaHabilitado { get; set; }        

    }
}