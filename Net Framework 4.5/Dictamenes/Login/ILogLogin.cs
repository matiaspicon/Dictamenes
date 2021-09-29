using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;
using BEUU;
//using BEUU.ClasesRpt;

namespace WCFLoginUniversal
{
    [ServiceContract]
    public interface ILogLogin
    {

        [OperationContract]
        WCFUsuarioLogeado GetDatosLogin(int idlog, bool PF);

        [OperationContract]
        string Desencriptar(string codigo);

        [OperationContract]
        bool ActualizoEstadoUsuario(WCFUsuarioLogeado usua);

        [OperationContract]
        BEUU.PregFrec ObtengoPFByID(int identrada);

        [OperationContract]
        bool EliminoIDPF(int identrada);

        [OperationContract]
        Usuarios ObtenerUsuarioSegunId(int identrada);

        [OperationContract]
        Aplicaciones ObtenerAplicacionById(int id);

        [OperationContract]
        string ObtengoMenu(int idusua, int idapp, int idgrupo);

        [OperationContract]
        string ObtengoMenuXML(int idusua, int idapp, int idgrupo);

        [OperationContract]
        string Encriptar(string valor);

        //[OperationContract]
        //List<RptUsuaLogeados> ObtengoDatosHistoricosLogeo(int appid, int userid, DateTime? fdesde, DateTime? fhasta);

        [OperationContract]
        List<LogLogin> ObtengoListaUsuariosLogeados();

        [OperationContract]
        List<LogLogin> ObtengoAplicativosConsumidos();

        [OperationContract]
        WCFUsuarioLogeado LogeoUsuario(string cuil, string idcomp, string nomusua, string pass, string idapp, bool PF);

        [OperationContract]
        Usuarios ObtengoPersonaPorCuit(string cuit);

        [OperationContract]
        List<Usuarios> ObtengoPersonaByAplicativo(int idapp);

        //[OperationContract]
        //List<RptRankings> ObtengoRankUserMasLog(int userid, DateTime? fdesde, DateTime? fhasta);

        [OperationContract]
        Usuarios ObtengoPersonaByCuitCiaID(string cuit, string ciaid);

        [OperationContract]
        Usuarios ObtengoPersonaPorNomUsua(string nomusua);

        //[OperationContract]
        //List<RptRankings> ObtengoRankEntMasLog(int entid, DateTime? fdesde, DateTime? fhasta);

        [OperationContract]
        Usuarios ObtengoPersonaByAplicativoIdUser(int idapp, int idUser);

        [OperationContract]
        string ObtengoCodigoPF(int idusua, int idapp, int idgrupo);

        [OperationContract]
        Entidades ObtengoEntidadByID(string idEntidad);

        [OperationContract]
        List<BEUU.Entidades> ObtenerEntidadesDeUsuario(double cuil);

        [OperationContract]
        bool TienePermisoUsuarioAPP(int idusua, int idapp);
    }

    [DataContract]
    public class WCFUsuarioLogeado
    {
        public WCFUsuarioLogeado()
        {
            Ent = new Entidades();
            Grupos = new Grupos();
            ApellidoPersona = "";
        }

        [DataMember]
        public string NombreUsuario { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string NombrePersona { get; set; }

        [DataMember]
        public string ApellidoPersona { get; set; }

        [DataMember]
        public string CUIL_CUIT { get; set; }

        [DataMember]
        public string Mail { get; set; }

        [DataMember]
        public string Telefono { get; set; }

        [DataMember]
        public string Menu { get; set; }

        [DataMember]
        public string MenuXML { get; set; }

        [DataMember]
        public Grupos Grupos { get; set; }

        [DataMember]
        public Entidades Ent { get; set; }

        [DataMember]
        public Gerencia Gerencias { get; set; }

        [DataMember]
        public int Activo { get; set; }
    }
}
