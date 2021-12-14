// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.FrameworkSection
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using Dictamenes.Framework.Configuration.Generics;
using System.Configuration;

namespace Dictamenes.Framework.Configuration
{
  public class FrameworkSection : ConfigurationSection
  {
    [ConfigurationProperty("AppID", IsRequired = true)]
    public IntElement AppID => (IntElement) this[nameof (AppID)];

    [ConfigurationProperty("ColorScheme", IsRequired = true)]
    public GenericCollection<ColoSchemeElement> ColorScheme => (GenericCollection<ColoSchemeElement>) this[nameof (ColorScheme)];

    [ConfigurationProperty("LoginUrl", IsRequired = true)]
    public StringElement LoginUrl => (StringElement) this[nameof (LoginUrl)];

    [ConfigurationProperty("HomePages")]
    public HomePagesCollection HomePages => (HomePagesCollection) this[nameof (HomePages)];

    [ConfigurationProperty("WebReferences")]
    public GenericCollection<WebReferenceElement> WebReferences => (GenericCollection<WebReferenceElement>) this[nameof (WebReferences)];

    [ConfigurationProperty("DataSources")]
    public GenericCollection<DataSourceElement> DataSources => (GenericCollection<DataSourceElement>) this[nameof (DataSources)];

    [ConfigurationProperty("Email", DefaultValue = null, IsRequired = false)]
    public EmailElement Email => (EmailElement) this[nameof (Email)];

    [ConfigurationProperty("ErrorReporting")]
    public ErrorReportingElement ErrorReporting => (ErrorReportingElement) this[nameof (ErrorReporting)];

    [ConfigurationProperty("UIGeneration", IsRequired = false)]
    public UIGenerationElement UIGeneration => (UIGenerationElement) this[nameof (UIGeneration)];

    [ConfigurationProperty("AutoLogin", IsRequired = false)]
    public IntElement AutoLogin => (IntElement) this[nameof (AutoLogin)];
  }
}
