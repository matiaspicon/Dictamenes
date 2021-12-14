// Decompiled with JetBrains decompiler
// Type: Framework.Security.SessionToken
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using Dictamenes.Framework.Login;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Dictamenes.Framework.Security
{
  public class SessionToken
  {
    public string SessionID { get; set; }

    public string OperacionID { get; set; }

    public string CiaID { get; set; }

    public int UsuarioID { get; set; }

    public string UsuarioNombre { get; set; }

    public string UsuarioApellido { get; set; }

    public string Cuil { get; set; }

    public int GrupoID { get; set; }

    public string GrupoNombre { get; set; }

    public Gerencia Gerencia { get; set; }

    public string Menu { get; set; }

    public string MenuXML { get; set; }

    public System.Collections.Generic.Dictionary<string, object> Dictionary { get; set; }

    public override string ToString()
    {
      StringWriter stringWriter = new StringWriter();
      foreach (PropertyInfo property in this.GetType().GetProperties())
      {
        if (property.Name != "Dictionary")
          stringWriter.WriteLine("{0}={1}", (object) property.Name, property.GetValue((object) this, (object[]) null));
      }
      stringWriter.WriteLine("Dictionary:");
      foreach (KeyValuePair<string, object> keyValuePair in this.Dictionary)
        stringWriter.WriteLine("{0}={1}", (object) keyValuePair.Key, keyValuePair.Value);
      return stringWriter.ToString();
    }
  }
}
