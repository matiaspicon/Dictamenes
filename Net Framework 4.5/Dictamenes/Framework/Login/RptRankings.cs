// Decompiled with JetBrains decompiler
// Type: Framework.Login.RptRankings
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Dictamenes.Framework.Login
{
  [DataContract(Name = "RptRankings", Namespace = "http://schemas.datacontract.org/2004/07/BEUU.ClasesRpt")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DebuggerStepThrough]
  [Serializable]
  public class RptRankings : RptUsuaLogeados
  {
    [OptionalField]
    private int CuentaField;
    [OptionalField]
    private int SumaField;

    [DataMember]
    public int Cuenta
    {
      get => this.CuentaField;
      set
      {
        if (this.CuentaField.Equals(value))
          return;
        this.CuentaField = value;
        this.RaisePropertyChanged(nameof (Cuenta));
      }
    }

    [DataMember]
    public int Suma
    {
      get => this.SumaField;
      set
      {
        if (this.SumaField.Equals(value))
          return;
        this.SumaField = value;
        this.RaisePropertyChanged(nameof (Suma));
      }
    }
  }
}
