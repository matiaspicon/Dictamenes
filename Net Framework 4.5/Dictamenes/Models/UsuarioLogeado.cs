﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dictamenes.Models
{
    public class UsuarioLogueado
    {

        public string NombreUsuario { get; set; }

        public int Id { get; set; }

        public string NombrePersona { get; set; }

        public string ApellidoPersona { get; set; }

        public string CUIL_CUIT { get; set; }

        public string Mail { get; set; }

        public string Telefono { get; set; }

        public string GrupoDescripcion { get; set; }

        public int IdGrupo { get; set; }

    }
}