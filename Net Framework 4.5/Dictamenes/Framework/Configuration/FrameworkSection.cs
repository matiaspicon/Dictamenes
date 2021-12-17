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
        public IntElement AppID => (IntElement)this[nameof(AppID)];

        [ConfigurationProperty("LoginUrl", IsRequired = true)]
        public StringElement LoginUrl => (StringElement)this[nameof(LoginUrl)];

        [ConfigurationProperty("Roles")]
        public GenericCollection<RolElement> Roles => (GenericCollection<RolElement>)this[nameof(Roles)];

        [ConfigurationProperty("WebReferences")]
        public GenericCollection<WebReferenceElement> WebReferences => (GenericCollection<WebReferenceElement>)this[nameof(WebReferences)];

        [ConfigurationProperty("DataSources")]
        public GenericCollection<DataSourceElement> DataSources => (GenericCollection<DataSourceElement>)this[nameof(DataSources)];
    }
}
