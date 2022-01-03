// Decompiled with JetBrains decompiler
// Type: Framework.Exceptions.DatabaseException
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System;
using System.Runtime.Serialization;

namespace Dictamenes.Framework.Exceptions
{
  public class DatabaseException : ApplicationException
  {
    protected DatabaseException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public DatabaseException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public DatabaseException(string message)
      : base(message)
    {
    }

    public DatabaseException()
    {
    }
  }
}
