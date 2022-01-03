// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.UIGenerationElement
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System.Configuration;

namespace Dictamenes.Framework.Configuration
{
  public class UIGenerationElement : ConfigurationElement
  {
    [ConfigurationProperty("Enabled", IsRequired = true)]
    public bool Enabled => (bool) this[nameof (Enabled)];
  }
}
