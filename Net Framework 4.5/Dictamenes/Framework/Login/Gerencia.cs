// Decompiled with JetBrains decompiler
// Type: Framework.Login.Gerencia
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Dictamenes.Framework.Login
{
  [DataContract(Name = "Gerencia", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Serializable]
  public class Gerencia : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private int GerenciaIDField;
    [OptionalField]
    private string NombreGerenciaField;

    [Browsable(false)]
    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public int GerenciaID
    {
      get => this.GerenciaIDField;
      set
      {
        if (this.GerenciaIDField.Equals(value))
          return;
        this.GerenciaIDField = value;
        this.RaisePropertyChanged(nameof (GerenciaID));
      }
    }

    [DataMember]
    public string NombreGerencia
    {
      get => this.NombreGerenciaField;
      set
      {
        if (object.ReferenceEquals((object) this.NombreGerenciaField, (object) value))
          return;
        this.NombreGerenciaField = value;
        this.RaisePropertyChanged(nameof (NombreGerencia));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
