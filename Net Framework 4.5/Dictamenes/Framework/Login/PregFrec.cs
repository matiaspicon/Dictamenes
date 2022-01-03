// Decompiled with JetBrains decompiler
// Type: Framework.Login.PregFrec
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
  [DebuggerStepThrough]
  [DataContract(Name = "PregFrec", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Serializable]
  public class PregFrec : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private int IdEntradaField;
    [OptionalField]
    private Usuarios UsuarioField;

    [Browsable(false)]
    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public int IdEntrada
    {
      get => this.IdEntradaField;
      set
      {
        if (this.IdEntradaField.Equals(value))
          return;
        this.IdEntradaField = value;
        this.RaisePropertyChanged(nameof (IdEntrada));
      }
    }

    [DataMember]
    public Usuarios Usuario
    {
      get => this.UsuarioField;
      set
      {
        if (object.ReferenceEquals((object) this.UsuarioField, (object) value))
          return;
        this.UsuarioField = value;
        this.RaisePropertyChanged(nameof (Usuario));
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
