// Decompiled with JetBrains decompiler
// Type: Framework.Login.ILogLogin
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System;
using System.CodeDom.Compiler;
using System.ServiceModel;

namespace Dictamenes.Framework.Login
{
  [ServiceContract(ConfigurationName = "Login.ILogLogin")]
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  public interface ILogLogin
  {
    [OperationContract(Action = "http://tempuri.org/ILogLogin/GetDatosLogin", ReplyAction = "http://tempuri.org/ILogLogin/GetDatosLoginResponse")]
    WCFUsuarioLogeado GetDatosLogin(int idlog, bool PF);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/Desencriptar", ReplyAction = "http://tempuri.org/ILogLogin/DesencriptarResponse")]
    string Desencriptar(string codigo);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ActualizoEstadoUsuario", ReplyAction = "http://tempuri.org/ILogLogin/ActualizoEstadoUsuarioResponse")]
    bool ActualizoEstadoUsuario(WCFUsuarioLogeado usua);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoPFByID", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoPFByIDResponse")]
    PregFrec ObtengoPFByID(int identrada);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/EliminoIDPF", ReplyAction = "http://tempuri.org/ILogLogin/EliminoIDPFResponse")]
    bool EliminoIDPF(int identrada);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtenerUsuarioSegunId", ReplyAction = "http://tempuri.org/ILogLogin/ObtenerUsuarioSegunIdResponse")]
    Usuarios ObtenerUsuarioSegunId(int identrada);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtenerAplicacionById", ReplyAction = "http://tempuri.org/ILogLogin/ObtenerAplicacionByIdResponse")]
    Aplicaciones ObtenerAplicacionById(int id);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoMenu", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoMenuResponse")]
    string ObtengoMenu(int idusua, int idapp, int idgrupo);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoMenuXML", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoMenuXMLResponse")]
    string ObtengoMenuXML(int idusua, int idapp, int idgrupo);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/Encriptar", ReplyAction = "http://tempuri.org/ILogLogin/EncriptarResponse")]
    string Encriptar(string valor);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoDatosHistoricosLogeo", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoDatosHistoricosLogeoResponse")]
    RptUsuaLogeados[] ObtengoDatosHistoricosLogeo(
      int appid,
      int userid,
      DateTime? fdesde,
      DateTime? fhasta);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoListaUsuariosLogeados", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoListaUsuariosLogeadosResponse")]
    LogLogin[] ObtengoListaUsuariosLogeados();

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoAplicativosConsumidos", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoAplicativosConsumidosResponse")]
    LogLogin[] ObtengoAplicativosConsumidos();

    [OperationContract(Action = "http://tempuri.org/ILogLogin/LogeoUsuario", ReplyAction = "http://tempuri.org/ILogLogin/LogeoUsuarioResponse")]
    WCFUsuarioLogeado LogeoUsuario(
      string cuil,
      string idcomp,
      string nomusua,
      string pass,
      string idapp,
      bool PF);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoPersonaPorCuit", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoPersonaPorCuitResponse")]
    Usuarios ObtengoPersonaPorCuit(string cuit);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoPersonaByAplicativo", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoPersonaByAplicativoResponse")]
    Usuarios[] ObtengoPersonaByAplicativo(int idapp);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoRankUserMasLog", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoRankUserMasLogResponse")]
    RptRankings[] ObtengoRankUserMasLog();

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoPersonaByCuitCiaID", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoPersonaByCuitCiaIDResponse")]
    Usuarios ObtengoPersonaByCuitCiaID(string cuit, string ciaid);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoPersonaPorNomUsua", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoPersonaPorNomUsuaResponse")]
    Usuarios ObtengoPersonaPorNomUsua(string nomusua);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoRankEntMasLog", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoRankEntMasLogResponse")]
    RptRankings[] ObtengoRankEntMasLog();

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoPersonaByAplicativoIdUser", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoPersonaByAplicativoIdUserResponse")]
    Usuarios ObtengoPersonaByAplicativoIdUser(int idapp, int idUser);

    [OperationContract(Action = "http://tempuri.org/ILogLogin/ObtengoCodigoPF", ReplyAction = "http://tempuri.org/ILogLogin/ObtengoCodigoPFResponse")]
    string ObtengoCodigoPF(int idusua, int idapp, int idgrupo);
  }
}
