// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.Generics.BoolElement
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System.Configuration;
using System.Xml;

namespace Dictamenes.Framework.Configuration.Generics
{
  public class BoolElement : ConfigurationElement
  {
    protected override void DeserializeElement(XmlReader reader, bool s)
    {
      try
      {
        this.Value = reader.ReadElementContentAs(typeof (bool), (IXmlNamespaceResolver) null) as bool?;
      }
      catch
      {
        this.Value = new bool?();
      }
    }

    public bool? Value { get; private set; }
  }
}
