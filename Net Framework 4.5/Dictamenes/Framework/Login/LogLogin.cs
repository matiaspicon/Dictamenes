// Decompiled with JetBrains decompiler
// Type: Framework.Login.LogLogin
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
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "LogLogin", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [DebuggerStepThrough]
  [Serializable]
  public class LogLogin : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private Aplicaciones AplicacionField;
    [OptionalField]
    private DateTime FechaField;
    [OptionalField]
    private int IdAPPField;
    [OptionalField]
    private int IdUsuaField;
    [OptionalField]
    private string NomAppField;
    [OptionalField]
    private string NomUsuaField;
    [OptionalField]
    private Usuarios UsuarioField;
    [OptionalField]
    private int idLogField;

    [Browsable(false)]
    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public Aplicaciones Aplicacion
    {
      get => this.AplicacionField;
      set
      {
        if (object.ReferenceEquals((object) this.AplicacionField, (object) value))
          return;
        this.AplicacionField = value;
        this.RaisePropertyChanged(nameof (Aplicacion));
      }
    }

    [DataMember]
    public DateTime Fecha
    {
      get => this.FechaField;
      set
      {
        if (this.FechaField.Equals(value))
          return;
        this.FechaField = value;
        this.RaisePropertyChanged(nameof (Fecha));
      }
    }

    [DataMember]
    public int IdAPP
    {
      get => this.IdAPPField;
      set
      {
        if (this.IdAPPField.Equals(value))
          return;
        this.IdAPPField = value;
        this.RaisePropertyChanged(nameof (IdAPP));
      }
    }

    [DataMember]
    public int IdUsua
    {
      get => this.IdUsuaField;
      set
      {
        if (this.IdUsuaField.Equals(value))
          return;
        this.IdUsuaField = value;
        this.RaisePropertyChanged(nameof (IdUsua));
      }
    }

    [DataMember]
    public string NomApp
    {
      get => this.NomAppField;
      set
      {
        if (object.ReferenceEquals((object) this.NomAppField, (object) value))
          return;
        this.NomAppField = value;
        this.RaisePropertyChanged(nameof (NomApp));
      }
    }

    [DataMember]
    public string NomUsua
    {
      get => this.NomUsuaField;
      set
      {
        if (object.ReferenceEquals((object) this.NomUsuaField, (object) value))
          return;
        this.NomUsuaField = value;
        this.RaisePropertyChanged(nameof (NomUsua));
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

    [DataMember]
    public int idLog
    {
      get => this.idLogField;
      set
      {
        if (this.idLogField.Equals(value))
          return;
        this.idLogField = value;
        this.RaisePropertyChanged(nameof (idLog));
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
