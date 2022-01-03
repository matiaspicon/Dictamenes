// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.EmailElement
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System.Configuration;

namespace Dictamenes.Framework.Configuration
{
  public class EmailElement : ConfigurationElement
  {
    [ConfigurationProperty("SMTP", IsRequired = true)]
    public string SMTP => (string) this[nameof (SMTP)];

    [ConfigurationProperty("Port", DefaultValue = 25, IsRequired = false)]
    public int Port => (int) this[nameof (Port)];

    [ConfigurationProperty("User", IsRequired = false)]
    public string User => (string) this[nameof (User)];

    [ConfigurationProperty("Password", IsRequired = false)]
    public string Password => (string) this[nameof (Password)];
  }
}
