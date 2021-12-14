// Decompiled with JetBrains decompiler
// Type: Framework.Login.RptUsuaLogeados
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
  [KnownType(typeof (RptRankings))]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "RptUsuaLogeados", Namespace = "http://schemas.datacontract.org/2004/07/BEUU.ClasesRpt")]
  [DebuggerStepThrough]
  [Serializable]
  public class RptUsuaLogeados : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private string ApellidoUsuarioField;
    [OptionalField]
    private int CiaIDField;
    [OptionalField]
    private DateTime FechaField;
    [OptionalField]
    private string NomAplicacionField;
    [OptionalField]
    private string NombreCIAField;
    [OptionalField]
    private string NombreUsuarioField;
    [OptionalField]
    private string UsuarioField;
    [OptionalField]
    private int idLogField;

    [Browsable(false)]
    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public string ApellidoUsuario
    {
      get => this.ApellidoUsuarioField;
      set
      {
        if (object.ReferenceEquals((object) this.ApellidoUsuarioField, (object) value))
          return;
        this.ApellidoUsuarioField = value;
        this.RaisePropertyChanged(nameof (ApellidoUsuario));
      }
    }

    [DataMember]
    public int CiaID
    {
      get => this.CiaIDField;
      set
      {
        if (this.CiaIDField.Equals(value))
          return;
        this.CiaIDField = value;
        this.RaisePropertyChanged(nameof (CiaID));
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
    public string NomAplicacion
    {
      get => this.NomAplicacionField;
      set
      {
        if (object.ReferenceEquals((object) this.NomAplicacionField, (object) value))
          return;
        this.NomAplicacionField = value;
        this.RaisePropertyChanged(nameof (NomAplicacion));
      }
    }

    [DataMember]
    public string NombreCIA
    {
      get => this.NombreCIAField;
      set
      {
        if (object.ReferenceEquals((object) this.NombreCIAField, (object) value))
          return;
        this.NombreCIAField = value;
        this.RaisePropertyChanged(nameof (NombreCIA));
      }
    }

    [DataMember]
    public string NombreUsuario
    {
      get => this.NombreUsuarioField;
      set
      {
        if (object.ReferenceEquals((object) this.NombreUsuarioField, (object) value))
          return;
        this.NombreUsuarioField = value;
        this.RaisePropertyChanged(nameof (NombreUsuario));
      }
    }

    [DataMember]
    public string Usuario
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
