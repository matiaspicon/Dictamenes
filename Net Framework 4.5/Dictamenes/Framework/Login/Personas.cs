// Decompiled with JetBrains decompiler
// Type: Framework.Login.Personas
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
  [DataContract(Name = "Personas", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [KnownType(typeof (ResponsableSuplente))]
  [KnownType(typeof (ReferenteSuplente))]
  [KnownType(typeof (Usuarios))]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [KnownType(typeof (Referente))]
  [KnownType(typeof (Responsable))]
  [Serializable]
  public class Personas : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private int ActivoField;
    [OptionalField]
    private string ApellidoPersonaField;
    [OptionalField]
    private Framework.Login.Aplicaciones[] AplicacionesField;
    [OptionalField]
    private string CUIL_CUITField;
    [OptionalField]
    private int CantAplicacionesField;
    [OptionalField]
    private Entidades EntField;
    [OptionalField]
    private DateTime FechaCambioField;
    [OptionalField]
    private Gerencia GerenciasField;
    [OptionalField]
    private Framework.Login.Grupos[] GruposField;
    [OptionalField]
    private int IdField;
    [OptionalField]
    private string MailField;
    [OptionalField]
    private string NombrePersonaField;
    [OptionalField]
    private string TelefonoField;
    [OptionalField]
    private int UsuaCambioField;

    [Browsable(false)]
    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public int Activo
    {
      get => this.ActivoField;
      set
      {
        if (this.ActivoField.Equals(value))
          return;
        this.ActivoField = value;
        this.RaisePropertyChanged(nameof (Activo));
      }
    }

    [DataMember]
    public string ApellidoPersona
    {
      get => this.ApellidoPersonaField;
      set
      {
        if (object.ReferenceEquals((object) this.ApellidoPersonaField, (object) value))
          return;
        this.ApellidoPersonaField = value;
        this.RaisePropertyChanged(nameof (ApellidoPersona));
      }
    }

    [DataMember]
    public Framework.Login.Aplicaciones[] Aplicaciones
    {
      get => this.AplicacionesField;
      set
      {
        if (object.ReferenceEquals((object) this.AplicacionesField, (object) value))
          return;
        this.AplicacionesField = value;
        this.RaisePropertyChanged(nameof (Aplicaciones));
      }
    }

    [DataMember]
    public string CUIL_CUIT
    {
      get => this.CUIL_CUITField;
      set
      {
        if (object.ReferenceEquals((object) this.CUIL_CUITField, (object) value))
          return;
        this.CUIL_CUITField = value;
        this.RaisePropertyChanged(nameof (CUIL_CUIT));
      }
    }

    [DataMember]
    public int CantAplicaciones
    {
      get => this.CantAplicacionesField;
      set
      {
        if (this.CantAplicacionesField.Equals(value))
          return;
        this.CantAplicacionesField = value;
        this.RaisePropertyChanged(nameof (CantAplicaciones));
      }
    }

    [DataMember]
    public Entidades Ent
    {
      get => this.EntField;
      set
      {
        if (object.ReferenceEquals((object) this.EntField, (object) value))
          return;
        this.EntField = value;
        this.RaisePropertyChanged(nameof (Ent));
      }
    }

    [DataMember]
    public DateTime FechaCambio
    {
      get => this.FechaCambioField;
      set
      {
        if (this.FechaCambioField.Equals(value))
          return;
        this.FechaCambioField = value;
        this.RaisePropertyChanged(nameof (FechaCambio));
      }
    }

    [DataMember]
    public Gerencia Gerencias
    {
      get => this.GerenciasField;
      set
      {
        if (object.ReferenceEquals((object) this.GerenciasField, (object) value))
          return;
        this.GerenciasField = value;
        this.RaisePropertyChanged(nameof (Gerencias));
      }
    }

    [DataMember]
    public Framework.Login.Grupos[] Grupos
    {
      get => this.GruposField;
      set
      {
        if (object.ReferenceEquals((object) this.GruposField, (object) value))
          return;
        this.GruposField = value;
        this.RaisePropertyChanged(nameof (Grupos));
      }
    }

    [DataMember]
    public int Id
    {
      get => this.IdField;
      set
      {
        if (this.IdField.Equals(value))
          return;
        this.IdField = value;
        this.RaisePropertyChanged(nameof (Id));
      }
    }

    [DataMember]
    public string Mail
    {
      get => this.MailField;
      set
      {
        if (object.ReferenceEquals((object) this.MailField, (object) value))
          return;
        this.MailField = value;
        this.RaisePropertyChanged(nameof (Mail));
      }
    }

    [DataMember]
    public string NombrePersona
    {
      get => this.NombrePersonaField;
      set
      {
        if (object.ReferenceEquals((object) this.NombrePersonaField, (object) value))
          return;
        this.NombrePersonaField = value;
        this.RaisePropertyChanged(nameof (NombrePersona));
      }
    }

    [DataMember]
    public string Telefono
    {
      get => this.TelefonoField;
      set
      {
        if (object.ReferenceEquals((object) this.TelefonoField, (object) value))
          return;
        this.TelefonoField = value;
        this.RaisePropertyChanged(nameof (Telefono));
      }
    }

    [DataMember]
    public int UsuaCambio
    {
      get => this.UsuaCambioField;
      set
      {
        if (this.UsuaCambioField.Equals(value))
          return;
        this.UsuaCambioField = value;
        this.RaisePropertyChanged(nameof (UsuaCambio));
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
