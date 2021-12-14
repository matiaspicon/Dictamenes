// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.ColoSchemeElement
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System.Configuration;

namespace Dictamenes.Framework.Configuration
{
  public class ColoSchemeElement : ConfigurationElement, IElement
  {
    [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
    public string Name => (string) this[nameof (Name)];

    [ConfigurationProperty("Value", IsRequired = true)]
    public string Value => (string) this[nameof (Value)];

    public object GetKey() => (object) this.Name;
  }
}
