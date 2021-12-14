// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.DataSourceElement
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System.Configuration;

namespace Dictamenes.Framework.Configuration
{
  public class DataSourceElement : ConfigurationElement, IElement
  {
    [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
    public string Name => (string) this[nameof (Name)];

    [ConfigurationProperty("Type", IsRequired = true)]
    public string Type => (string) this[nameof (Type)];

    [ConfigurationProperty("ConnectionString", IsRequired = true)]
    public string ConnectionString => (string) this[nameof (ConnectionString)];

    [ConfigurationProperty("IsDefault", IsRequired = false)]
    public bool IsDefault => (bool) this[nameof (IsDefault)];

    public object GetKey() => (object) this.Name;
  }
}
