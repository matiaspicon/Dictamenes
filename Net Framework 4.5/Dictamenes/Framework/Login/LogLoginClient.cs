// Decompiled with JetBrains decompiler
// Type: Framework.Login.LogLoginClient
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Dictamenes.Framework.Login
{
  [DebuggerStepThrough]
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  public class LogLoginClient : ClientBase<ILogLogin>, ILogLogin
  {
    public LogLoginClient()
    {
    }

    public LogLoginClient(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    public LogLoginClient(string endpointConfigurationName, string remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public LogLoginClient(string endpointConfigurationName, EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public LogLoginClient(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
    }

    public WCFUsuarioLogeado GetDatosLogin(int idlog, bool PF) => this.Channel.GetDatosLogin(idlog, PF);

    public string Desencriptar(string codigo) => this.Channel.Desencriptar(codigo);

    public bool ActualizoEstadoUsuario(WCFUsuarioLogeado usua) => this.Channel.ActualizoEstadoUsuario(usua);

    public PregFrec ObtengoPFByID(int identrada) => this.Channel.ObtengoPFByID(identrada);

    public bool EliminoIDPF(int identrada) => this.Channel.EliminoIDPF(identrada);

    public Usuarios ObtenerUsuarioSegunId(int identrada) => this.Channel.ObtenerUsuarioSegunId(identrada);

    public Aplicaciones ObtenerAplicacionById(int id) => this.Channel.ObtenerAplicacionById(id);

    public string ObtengoMenu(int idusua, int idapp, int idgrupo) => this.Channel.ObtengoMenu(idusua, idapp, idgrupo);

    public string ObtengoMenuXML(int idusua, int idapp, int idgrupo) => this.Channel.ObtengoMenuXML(idusua, idapp, idgrupo);

    public string Encriptar(string valor) => this.Channel.Encriptar(valor);

    public RptUsuaLogeados[] ObtengoDatosHistoricosLogeo(
      int appid,
      int userid,
      DateTime? fdesde,
      DateTime? fhasta)
    {
      return this.Channel.ObtengoDatosHistoricosLogeo(appid, userid, fdesde, fhasta);
    }

    public LogLogin[] ObtengoListaUsuariosLogeados() => this.Channel.ObtengoListaUsuariosLogeados();

    public LogLogin[] ObtengoAplicativosConsumidos() => this.Channel.ObtengoAplicativosConsumidos();

    public WCFUsuarioLogeado LogeoUsuario(
      string cuil,
      string idcomp,
      string nomusua,
      string pass,
      string idapp,
      bool PF)
    {
      return this.Channel.LogeoUsuario(cuil, idcomp, nomusua, pass, idapp, PF);
    }

    public Usuarios ObtengoPersonaPorCuit(string cuit) => this.Channel.ObtengoPersonaPorCuit(cuit);

    public Usuarios[] ObtengoPersonaByAplicativo(int idapp) => this.Channel.ObtengoPersonaByAplicativo(idapp);

    public RptRankings[] ObtengoRankUserMasLog() => this.Channel.ObtengoRankUserMasLog();

    public Usuarios ObtengoPersonaByCuitCiaID(string cuit, string ciaid) => this.Channel.ObtengoPersonaByCuitCiaID(cuit, ciaid);

    public Usuarios ObtengoPersonaPorNomUsua(string nomusua) => this.Channel.ObtengoPersonaPorNomUsua(nomusua);

    public RptRankings[] ObtengoRankEntMasLog() => this.Channel.ObtengoRankEntMasLog();

    public Usuarios ObtengoPersonaByAplicativoIdUser(int idapp, int idUser) => this.Channel.ObtengoPersonaByAplicativoIdUser(idapp, idUser);

    public string ObtengoCodigoPF(int idusua, int idapp, int idgrupo) => this.Channel.ObtengoCodigoPF(idusua, idapp, idgrupo);
  }
}
