using System.Configuration;

namespace Dictamenes.Framework.Configuration
{
    public class RolElement : ConfigurationElement, IElement
    {
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name => (string)this[nameof(Name)];

        [ConfigurationProperty("Id", IsRequired = true)]
        public string Id => (string)this[nameof(Id)];

        public object GetKey() => (object)this.Name;
    }
}