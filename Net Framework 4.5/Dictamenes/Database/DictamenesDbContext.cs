using Dictamenes.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Dictamenes.Database
{ 
    public class DictamenesDbContext : System.Data.Entity.DbContext
    {
        public DictamenesDbContext() : base("name=SQLServer") { }

        public DbSet<ArchivoPDF> ArchivosPDF { get; set; }
        public DbSet<Asunto> Asuntos { get; set; }
        public DbSet<Dictamen> Dictamenes { get; set; }
        public DbSet<SujetoControlado> SujetosControlados { get; set; }
        public DbSet<TipoDictamen> TiposDictamen { get; set; }
        public DbSet<TipoSujetoControlado> TiposSujetoControlado { get; set; }
        public DbSet<AsuntoLog> AsuntosLog { get; set; }
        public DbSet<DictamenLog> DictamenesLog { get; set; }
        public DbSet<SujetoControladoLog> SujetosControladosLog { get; set; }
        public DbSet<TipoDictamenLog> TiposDictamenLog { get; set; }
        public DbSet<TipoSujetoControladoLog> TiposSujetoControladoLog { get; set; }


        public virtual List<Dictamen> Sp_FiltrarDictamenes(string nroGDE, string nroExpediente, Nullable<System.DateTime> fechaInicio, Nullable<System.DateTime> fechaFinal, string detalle, string contenido, Nullable<int> idAsunto, Nullable<int> idTipoDictamen, Nullable<int> idSujetoControlado, Nullable<int> idTipoSujetoControlado, Nullable<long> cuilCuit, string nombre, string apellido)
        {
            var nroGDEParameter = nroGDE != null ?
                new SqlParameter("@NroGDE", nroGDE) :
                new SqlParameter("@NroGDE", DBNull.Value);

            var nroExpedienteParameter = nroExpediente != null ?
                new SqlParameter("@NroExpediente", nroExpediente) :
                new SqlParameter("@NroExpediente", DBNull.Value);

            var fechaInicioParameter = fechaInicio.HasValue ?
                new SqlParameter("@FechaInicio", fechaInicio) :
                new SqlParameter("@FechaInicio", DBNull.Value);

            var fechaFinalParameter = fechaFinal.HasValue ?
                new SqlParameter("@FechaFinal", fechaFinal) :
                new SqlParameter("@FechaFinal", DBNull.Value);

            var detalleParameter = detalle != null ?
                new SqlParameter("@Detalle", detalle) :
                new SqlParameter("@Detalle", DBNull.Value);

            var contenidoParameter = contenido != null ?
                new SqlParameter("@Contenido", contenido) :
                new SqlParameter("@Contenido", DBNull.Value);

            var idAsuntoParameter = idAsunto.HasValue ?
                new SqlParameter("@IdAsunto", idAsunto) :
                new SqlParameter("@IdAsunto", DBNull.Value);

            var idTipoDictamenParameter = idTipoDictamen.HasValue ?
                new SqlParameter("@IdTipoDictamen", idTipoDictamen) :
                new SqlParameter("@IdTipoDictamen", DBNull.Value);

            var idSujetoControladoParameter = idSujetoControlado.HasValue ?
                new SqlParameter("@IdSujetoControlado", idSujetoControlado) :
                new SqlParameter("@IdSujetoControlado", DBNull.Value);

            var idTipoSujetoControladoParameter = idTipoSujetoControlado.HasValue ?
                new SqlParameter("@IdTipoSujetoControlado", idTipoSujetoControlado) :
                new SqlParameter("@IdTipoSujetoControlado", DBNull.Value);

            var cuilCuitParameter = cuilCuit.HasValue ?
                new SqlParameter("@CuilCuit", cuilCuit) :
                new SqlParameter("@CuilCuit", DBNull.Value);

            var nombreParameter = nombre != null ?
                new SqlParameter("@Nombre", nombre) :
                new SqlParameter("@Nombre", DBNull.Value);

            var apellidoParameter = apellido != null ?
                new SqlParameter("@Apellido", apellido) :
                new SqlParameter("@Apellido", DBNull.Value);

            var algo = this.Dictamenes.SqlQuery("sp_FiltrarDictamenes @NroGDE , @NroExpediente , @FechaInicio , @FechaFinal , @Detalle , @Contenido , @IdAsunto , @IdTipoDictamen , @IdSujetoControlado, @idTipoSujetoControlado , @CuilCuit , @Nombre , @Apellido",
            nroGDEParameter, nroExpedienteParameter, fechaInicioParameter, fechaFinalParameter, detalleParameter, contenidoParameter,
            idAsuntoParameter, idTipoDictamenParameter, idSujetoControladoParameter, idTipoSujetoControladoParameter, cuilCuitParameter, nombreParameter, apellidoParameter);

            var devolver = algo.AsQueryable<Dictamen>().Include(d => d.Asunto);
            return devolver.ToList();


        }

        public class Algo
        {
            Int32 CompaniaID { get; set; }

            String CiaID { get; set; }

            String Denominacion { get; set; }

            String CUIT { get; set; }

            String DenominacionCorta { get; set; }
        }


        public virtual List<Algo> Sp_GetAseguradoras()
        {
            return this.Database.SqlQuery<Algo>("Traer_Aseguradoras").ToList();
        }

        public virtual List<Algo> Sp_GetReaseguradoras()
        {
            return this.Database.SqlQuery<Algo>("Traer_Reaseguradoras").ToList();
        }


    }

}