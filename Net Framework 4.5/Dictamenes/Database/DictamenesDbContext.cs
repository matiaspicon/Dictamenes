using System;
using System.Collections.Generic;
using System.Data.Entity;
using Dictamenes.Models;


namespace Dictamenes.Database
{
    public class DictamenesDbContext : DbContext
    {
        public DictamenesDbContext() : base("name=MyDbCS") { }

        public DbSet<ArchivoPDF> ArchivosPDF { get; set; }
        public DbSet<Asunto> Asuntos { get; set; }
        public DbSet<Dictamen> Dictamenes { get; set; }
        public DbSet<SujetoObligado> SujetosObligados { get; set; }
        public DbSet<TipoDictamen> TiposDictamen { get; set; }
        public DbSet<TipoSujetoObligado> TiposSujetoObligado { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        
    }

}