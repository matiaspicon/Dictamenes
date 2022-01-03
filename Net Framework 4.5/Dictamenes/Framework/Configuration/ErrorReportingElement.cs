// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.ErrorReportingElement
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using Dictamenes.Framework.Configuration.Generics;
using System.Configuration;

namespace Dictamenes.Framework.Configuration
{
  public class ErrorReportingElement : ConfigurationElement
  {
    [ConfigurationProperty("Enabled", IsRequired = true)]
    public bool Enabled => (bool) this[nameof (Enabled)];

    [ConfigurationProperty("Email", IsRequired = false)]
    public StringElement Email => (StringElement) this[nameof (Email)];

    [ConfigurationProperty("FileSystem", IsRequired = false)]
    public StringElement FileSystem => (StringElement) this[nameof (FileSystem)];
  }
}
