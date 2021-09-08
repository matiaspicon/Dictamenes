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
        public DbSet<Dictamen> Categorias { get; set; }
        public DbSet<SujetoObligado> Clientes { get; set; }
        public DbSet<TipoDictamen> Compras { get; set; }
        public DbSet<TipoSujetoObligado> Empleados { get; set; }
        public DbSet<Usuario> Productos { get; set; }
    }

}