using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dictamenes.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Dictamenes.Database
{
    public class DictamenesDbContext : DbContext
    {
        public DictamenesDbContext(DbContextOptions<DictamenesDbContext> options) : base(options)
        {
        }

        public DbSet<ArchivoPDF> ArchivoPDF { get; set; }
        public DbSet<Asunto> Asunto { get; set; }
        public DbSet<Dictamen> Dictamenes { get; set; }
        public DbSet<SujetoObligado> SujetoObligado { get; set; }
        public DbSet<TipoDictamen> TipoDictamen { get; set; }
        public DbSet<TipoSujetoObligado> TipoSujetoObligado { get; set; }
        public DbSet<Usuario> Usuario { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Busqueda>().HasNoKey();
        }


        public DbSet<Dictamenes.Models.Busqueda> Busqueda { get; set; }
    }

}