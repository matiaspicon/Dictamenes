// Decompiled with JetBrains decompiler
// Type: Framework.Exceptions.ConfigurationException
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System;

namespace Dictamenes.Framework.Exceptions
{
  public class ConfigurationException : ApplicationException
  {
    public ConfigurationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ConfigurationException(string message)
      : base(message)
    {
    }

    public ConfigurationException()
    {
    }
  }
}
