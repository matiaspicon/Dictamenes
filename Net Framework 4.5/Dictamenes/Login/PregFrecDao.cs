using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace WCFLoginUniversal
{
    public class PregFrecDao
    {
        private String connectionString;


        public PregFrecDao(String _connectionString)
        {
            connectionString = _connectionString;
        }

        /* para insertar el id generado para PF */
        public PregFrec InsertoPF(int idusua)
        {
            #region Conectar e in inicializar el SP
            SqlConnection lConnection;
            SqlCommand lStoreProcedure;
            SqlDataReader lReader = null;
            lConnection = new SqlConnection(connectionString);
            lStoreProcedure = new SqlCommand("InsertoURLPF", lConnection);
            lStoreProcedure.CommandType = System.Data.CommandType.StoredProcedure;
            #endregion
            #region Setear los parametros del SP

            // declaro los parametros poniendole el tipo correspondiente.

            SqlParameter lIdUsua = new SqlParameter("@idusua", SqlDbType.Int);
            lIdUsua.Direction = ParameterDirection.Input;
            lIdUsua.Value = idusua;


            // seteo los parametros al SP
            lStoreProcedure.Parameters.Add(lIdUsua);

            #endregion
            #region Abrir la conexion y ejecutar el SP
            // abro conexion y ejecuto el SP
            lConnection.Open();
            //lStoreProcedure.ExecuteNonQuery();
            #endregion
            #region Chequear el resultado del SP y devolver lo que sea necesario
            lReader = lStoreProcedure.ExecuteReader();

            PregFrec lPregFrec = new PregFrec();

            while (lReader.Read())
            {
                lPregFrec.IdPF = (lReader["IdEntrada"].GetType().Equals(typeof(System.DBNull)) ? 0 : (int)lReader["IdEntrada"]);
                lPregFrec.IdUsuario = (lReader["idusuario"].GetType().Equals(typeof(System.DBNull)) ? 0 : (int)lReader["idusuario"]);
            }
            #endregion
            #region cerrar cosas de la DB y devolver el resultado
            lStoreProcedure.Dispose();
            lConnection.Close();
            return lPregFrec;
            #endregion
        }

        /* para insertar el id generado para PF */
        public PregFrec BuscoIDPF(int idPF)
        {
            #region Conectar e in inicializar el SP
            SqlConnection lConnection;
            SqlCommand lStoreProcedure;
            SqlDataReader lReader = null;
            lConnection = new SqlConnection(connectionString);
            lStoreProcedure = new SqlCommand("BuscoIDPF", lConnection);
            lStoreProcedure.CommandType = System.Data.CommandType.StoredProcedure;
            #endregion
            #region Setear los parametros del SP

            // declaro los parametros poniendole el tipo correspondiente.

            SqlParameter lId = new SqlParameter("@id", SqlDbType.Int);
            lId.Direction = ParameterDirection.Input;
            lId.Value = idPF;


            // seteo los parametros al SP
            lStoreProcedure.Parameters.Add(lId);

            #endregion
            #region Abrir la conexion y ejecutar el SP
            // abro conexion y ejecuto el SP
            lConnection.Open();
            lStoreProcedure.ExecuteNonQuery();
            #endregion
            #region Chequear el resultado del SP y devolver lo que sea necesario
            lReader = lStoreProcedure.ExecuteReader();

            PregFrec lPregFrec = new PregFrec();

            while (lReader.Read())
            {
                lPregFrec.IdPF = (lReader["IdEntrada"].GetType().Equals(typeof(System.DBNull)) ? 0 : (int)lReader["IdEntrada"]);
                lPregFrec.IdUsuario = (lReader["idusuario"].GetType().Equals(typeof(System.DBNull)) ? 0 : (int)lReader["idusuario"]);
            }
            #endregion
            #region cerrar cosas de la DB y devolver el resultado
            lStoreProcedure.Dispose();
            lConnection.Close();
            return lPregFrec;
            #endregion
        }

        /* Para borrar un usuario por id */
        public bool borrarUsuarioPorId(int Id)
        {
            #region Conectar e in inicializar el SP
            SqlConnection lConnection;
            SqlCommand lStoreProcedure;
            lConnection = new SqlConnection(connectionString);
            lStoreProcedure = new SqlCommand("EliminoIDPF", lConnection);
            lStoreProcedure.CommandType = System.Data.CommandType.StoredProcedure;
            #endregion
            #region Setear los parametros del SP
            /*
             * Pos	Name	Type	User Type	Options
             * 1	@ID_usuario	numeric(18,0)		Input
             * 2	@error	numeric(18,0)		Output
             */

            // declaro los parametros poniendole el tipo correspondiente.
            SqlParameter lID = new SqlParameter("@id", SqlDbType.Int);
            lID.Direction = ParameterDirection.Input;
            lID.Value = Id;

            // seteo los parametros al SP
            lStoreProcedure.Parameters.Add(lID);
            #endregion
            #region Abrir la conexion y ejecutar el SP
            bool resp = false;

            try
            {
                // abro conexion y ejecuto el SP
                lConnection.Open();
                lStoreProcedure.ExecuteNonQuery();
                resp = true;
            }

            catch
            {
                resp = false;
            }
            #endregion

            #region cerrar cosas de la DB y devolver el resultado
            lStoreProcedure.Dispose();
            lConnection.Close();
            return resp;
            #endregion
        }

    }
}
