
using Dictamenes.Framework.Configuration;
using System.Configuration;
using System.Web;

namespace Dictamenes.Framework
{
    public static class App
  {
    private static FrameworkSection _config;

    public static FrameworkSection Config
    {
      get
      {
        if (App._config == null)
        {
          ConfigurationSection section = ConfigurationManager.OpenMappedMachineConfiguration(new ConfigurationFileMap(HttpContext.Current.Request.PhysicalApplicationPath + "Web.config")).GetSection("framework");
          App._config = section != null && !(section.GetType() != typeof (FrameworkSection)) ? (FrameworkSection) section : throw new Framework.Exceptions.ConfigurationException("Framework section missing or wrong formatted");
        }
        return App._config;
      }
    }
  }
}
