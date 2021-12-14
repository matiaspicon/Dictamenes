// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.Generics.StringElement
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System.Configuration;
using System.Xml;

namespace Dictamenes.Framework.Configuration.Generics
{
  public class StringElement : ConfigurationElement
  {
    protected override void DeserializeElement(XmlReader reader, bool s) => this.Value = reader.ReadElementContentAs(typeof (string), (IXmlNamespaceResolver) null) as string;

    public string Value { get; private set; }

    public override string ToString() => this.Value;
  }
}
