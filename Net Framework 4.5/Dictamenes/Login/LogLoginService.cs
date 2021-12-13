using BEUU;
using BEUU.ClasesRpt;
using NegUU;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace WCFLoginUniversal
{
    public class LogLoginService : ILogLogin
    {

        public WCFUsuarioLogeado GetDatosLogin(int idlog, bool PF = false)
        {
            //throw new Exception("MARCO EL ERROR :");

            string idapp = "";
            string iduser = "";

            var wcfll = new WCFUsuarioLogeado();
            try
            {
                SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
                SingletonAplicacionesConexion.ConecString.ConexionString =
                    ConfigurationManager.AppSettings["csAplicaciones"];

                LogLogin logl = NLogLogin.ObtengoLogById(idlog);

                if (logl == null)
                    return null;


                Usuarios usua = logl.Usuario;

                logl.Aplicacion.idAPlicacion = logl.IdAPP;
                logl.Usuario.Id = logl.IdUsua;

                wcfll.ApellidoPersona = usua.ApellidoPersona;
                wcfll.CUIL_CUIT = usua.CUIL_CUIT;
                wcfll.Ent = usua.Ent;
                wcfll.Gerencias = NGerencia.ObtengoGerenciasPorCuil(usua.CUIL_CUIT);
                wcfll.Grupos = NGrupos.ObtenerGrupoPorAPPUsuario(logl.Aplicacion, usua);
                wcfll.Id = usua.Id;
                wcfll.Mail = usua.Mail;
                wcfll.NombrePersona = usua.NombrePersona;
                wcfll.NombreUsuario = usua.NombreUsuario;
                wcfll.Telefono = usua.Telefono;

                if (PF)
                {
                    var pDao = new PregFrecDao(ConfigurationManager.AppSettings["csUniversal"]);
                    PregFrec pf = pDao.InsertoPF(Convert.ToInt32(usua.Id));

                    string menu = MenuHelper.buildMenu(ObtengoMenuAPPUsua(usua.Id, logl.Aplicacion.idAPlicacion),
                        logl.Aplicacion.idAPlicacion, usua.Id, wcfll.Grupos.IdGrupo, pf);

                    wcfll.MenuXML = menu;

                    wcfll.Menu = MenuHelper.armarMenu(menu);
                }
                else
                {
                    string menu = MenuHelper.buildMenu(ObtengoMenuAPPUsua(usua.Id, logl.Aplicacion.idAPlicacion),
                        logl.Aplicacion.idAPlicacion, usua.Id, wcfll.Grupos.IdGrupo, null);

                    wcfll.MenuXML = menu;

                    wcfll.Menu = MenuHelper.armarMenu(menu);
                }
            }

            catch (Exception ex)
            {
                throw new Exception("MARCO EL ERROR :" + ex.ToString() + idapp + " - " + iduser);
            }

            return wcfll;
        }

        public WCFUsuarioLogeado LogeoUsuario(string cuil, string idcomp, string nomusua, string pass, string idapp, bool PF)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            Usuarios usua = NPersonas.ObtengoExistenciaPersona(cuil, idcomp, nomusua, pass);

            if (usua == null) return null;

            List<Aplicaciones> lstapp = usua.Aplicaciones.Where(a => a.idAPlicacion == Convert.ToInt32(idapp)).ToList();

            var app = new Aplicaciones();

            if (lstapp.Count > 0)
                app = lstapp.First();
            else
                return null;

            if (usua != null)
            {
                var wcfll = new WCFUsuarioLogeado
                {
                    ApellidoPersona = usua.ApellidoPersona,
                    CUIL_CUIT = usua.CUIL_CUIT,
                    Ent = usua.Ent,
                    Gerencias = NGerencia.ObtengoGerenciasPorCuil(usua.CUIL_CUIT),
                    Grupos = NGrupos.ObtenerGrupoPorAPPUsuario(app, usua),
                    Id = usua.Id,
                    Mail = usua.Mail,
                    NombrePersona = usua.NombrePersona,
                    NombreUsuario = usua.NombreUsuario,
                    Telefono = usua.Telefono
                };

                if (PF)
                {
                    var pDao = new PregFrecDao(ConfigurationManager.AppSettings["csUniversal"]);
                    PregFrec pf = pDao.InsertoPF(Convert.ToInt32(usua.Id));

                    string menu = MenuHelper.buildMenu(ObtengoMenuAPPUsua(usua.Id, app.idAPlicacion), app.idAPlicacion, usua.Id, wcfll.Grupos.IdGrupo, pf);

                    wcfll.MenuXML = menu;

                    wcfll.Menu = MenuHelper.armarMenu(menu);
                }
                else
                {
                    string menu = MenuHelper.buildMenu(ObtengoMenuAPPUsua(usua.Id, app.idAPlicacion), app.idAPlicacion, usua.Id, wcfll.Grupos.IdGrupo, null);

                    wcfll.MenuXML = menu;

                    wcfll.Menu = MenuHelper.armarMenu(menu);
                }

                return wcfll;
            }
            return null;
        }


        //public void Obtengo

        public string ObtengoMenu(int idusua, int idapp, int idgrupo)
        {
            var pDao = new PregFrecDao(ConfigurationManager.AppSettings["csUniversal"]);
            PregFrec pf = pDao.InsertoPF(Convert.ToInt32(idusua));

            string menu = MenuHelper.buildMenu(ObtengoMenuAPPUsua(idusua, idapp), idapp, idusua, idgrupo, pf);

            return MenuHelper.armarMenu(menu);
        }

        public string ObtengoMenuXML(int idusua, int idapp, int idgrupo)
        {
            var pDao = new PregFrecDao(ConfigurationManager.AppSettings["csUniversal"]);
            PregFrec pf = pDao.InsertoPF(Convert.ToInt32(idusua));

            return MenuHelper.buildMenu(ObtengoMenuAPPUsua(idusua, idapp), idapp, idusua, idgrupo, pf);
        }

        public string Desencriptar(string codigo)
        {
            string[] lsts = codigo.Split('j');

            var abyte = new byte[lsts.Length];

            int i = 0;

            foreach (string str in lsts)
            {
                if (str != "")
                {
                    byte b = Convert.ToByte(str);

                    abyte[i] = b;
                }

                i = i + 1;

            }

            return Encript.DESENC(abyte);
        }

        public static DataTable ObtengoMenuAPPUsua(int aUserID, int aIdSistema)
        {
            #region inicializar conexion

            var lConnection = new SqlConnection(ConfigurationManager.AppSettings["csAplicaciones"]);
            var lStoreProcedure = new SqlCommand("spObtenerMenuPorUsuarioApp", lConnection) { CommandType = CommandType.StoredProcedure };

            #endregion
            #region seteo de parametros
            var lIdUsuario = new SqlParameter("@intUsuarioID", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = aUserID };

            var lAppID = new SqlParameter("@intAppID", SqlDbType.VarChar) { Direction = ParameterDirection.Input, Value = aIdSistema };

            lStoreProcedure.Parameters.Add(lIdUsuario);
            lStoreProcedure.Parameters.Add(lAppID);
            #endregion
            #region ejecutar el store
            lConnection.Open();
            lStoreProcedure.ExecuteNonQuery();
            #endregion
            #region llenar el resultado de data table
            var adapter = new SqlDataAdapter(lStoreProcedure);
            var dt = new DataTable();
            adapter.Fill(dt);
            #endregion
            #region cierro el store, la conexion , y devuelvo el resultado

            lStoreProcedure.Dispose();
            lConnection.Close();

            return dt;
            #endregion


        }


        public bool ActualizoEstadoUsuario(WCFUsuarioLogeado usua)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            Usuarios usuario = NPersonas.ObtengoPersonaCuitCia(usua.CUIL_CUIT, usua.Ent.Codigo);

            return NPersonas.ActualizoUsuario(usuario, usua.Activo != 0);

        }


        public BEUU.PregFrec ObtengoPFByID(int identrada)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            BEUU.PregFrec pf = NPregFrec.ObtengoPFByID(identrada);

            return pf;
        }

        public bool EliminoIDPF(int identrada)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NPregFrec.EliminoIDPF(identrada);
        }


        public Usuarios ObtenerUsuarioSegunId(int id)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            Usuarios usua = NPersonas.ObtengoPersonaPorID(id);

            return usua;
        }

        public Aplicaciones ObtenerAplicacionById(int id)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            Aplicaciones usua = NAplicaciones.ObtenerAplicacionByID(id);

            return usua;
        }

        public string Encriptar(string valor)
        {
            var encoding = new UTF8Encoding();
            byte[] encriptado = encoding.GetBytes(valor);


            return encriptado.Aggregate("", (current, b) => current + b.ToString() + "j");

        }


        public List<RptUsuaLogeados> ObtengoDatosHistoricosLogeo(int appid, int userid, DateTime? fdesde, DateTime? fhasta)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NLogLogin.ObtengoDatosLog(appid, userid, fdesde, fhasta);
        }


        public List<LogLogin> ObtengoListaUsuariosLogeados()
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NLogLogin.ObtengoUsuariosLogeados();
        }

        public List<LogLogin> ObtengoAplicativosConsumidos()
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NLogLogin.ObtengoAplicativosConsumidos();
        }

        public Usuarios ObtengoPersonaPorCuit(string cuit)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NPersonas.ObtengoPersonaPorCuit(cuit);
        }

        public List<Usuarios> ObtengoPersonaByAplicativo(int idapp)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NPersonas.ObtengoUsuariosByAPP(idapp);
        }

        public Usuarios ObtengoPersonaByAplicativoIdUser(int idapp, int idUser)
        {
            try
            {

                SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
                SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

                var lst = new List<Grupos>();
                var lstApp = new List<Aplicaciones>();

                Usuarios pers = NPersonas.ObtengoPersonaPorID(idUser);

                Aplicaciones app = NAplicaciones.ObtenerAplicacionByID(idapp);

                lstApp.Add(app);

                pers.Aplicaciones = lstApp;

                Grupos ogrupo = NGrupos.ObtenerGrupoPorAPPUsuario(new Aplicaciones { idAPlicacion = idapp }, pers);

                lst.Add(ogrupo);

                pers.Grupos = lst;

                Gerencia ger = NGerencia.ObtengoGerenciasPorCuil(pers.CUIL_CUIT);

                pers.Gerencias = ger;

                return pers;
            }

            catch (Exception)
            {
                return null;
            }
        }

        public List<RptRankings> ObtengoRankUserMasLog(int userid, DateTime? fdesde, DateTime? fhasta)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NLogLogin.ObtengoRankUserMasLog(userid, fdesde, fhasta);
        }

        public Usuarios ObtengoPersonaByCuitCiaID(string cuit, string ciaid)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NPersonas.ObtengoPersonaCuitCia(cuit, ciaid);
        }

        public Usuarios ObtengoPersonaPorNomUsua(string nomusua)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NPersonas.ObtengoPersonaPorNomUsua(nomusua);
        }

        public List<RptRankings> ObtengoRankEntMasLog(int entid, DateTime? fdesde, DateTime? fhasta)
        {
            SingletonConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csUniversal"];
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];

            return NLogLogin.ObtengoRankEntMasLog(entid, fdesde, fhasta);
        }

        public string ObtengoCodigoPF(int idusua, int idapp, int idgrupo)
        {
            var pDao = new PregFrecDao(ConfigurationManager.AppSettings["csUniversal"]);
            PregFrec pf = pDao.InsertoPF(Convert.ToInt32(idusua));

            byte[] encriptado = Encript.ENC(idusua + "|" + idapp + "|" + idgrupo + "|" + pf.IdPF);

            return encriptado.Aggregate("", (current, b) => current + b.ToString() + "j");
        }

        public Entidades ObtengoEntidadByID(string IdEntidad)
        {
            return NEntidades.ObtenerEntidadesByID(IdEntidad);
        }

        public List<Entidades> ObtenerEntidadesDeUsuario(double cuil)
        {
            //return NEntidades.ObtenerEntidadesDeUsuario(cuil);

            return null;
        }

        public bool TienePermisoUsuarioAPP(int idusua, int idapp)
        {
            SingletonAplicacionesConexion.ConecString.ConexionString = ConfigurationManager.AppSettings["csAplicaciones"];
            //return NPermisos.TienePermisoUsuarioAPP(idusua, idapp);

            return true;
        }
    }
}
