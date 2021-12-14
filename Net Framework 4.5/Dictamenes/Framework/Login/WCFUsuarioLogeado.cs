// Decompiled with JetBrains decompiler
// Type: Framework.Login.WCFUsuarioLogeado
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
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [DataContract(Name = "WCFUsuarioLogeado", Namespace = "http://schemas.datacontract.org/2004/07/WCFLoginUniversal")]
  [Serializable]
  public class WCFUsuarioLogeado : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private int ActivoField;
    [OptionalField]
    private string ApellidoPersonaField;
    [OptionalField]
    private string CUIL_CUITField;
    [OptionalField]
    private Entidades EntField;
    [OptionalField]
    private Gerencia GerenciasField;
    [OptionalField]
    private Grupos GruposField;
    [OptionalField]
    private int IdField;
    [OptionalField]
    private string MailField;
    [OptionalField]
    private string MenuField;
    [OptionalField]
    private string MenuXMLField;
    [OptionalField]
    private string NombrePersonaField;
    [OptionalField]
    private string NombreUsuarioField;
    [OptionalField]
    private string TelefonoField;

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
    public Grupos Grupos
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
    public string Menu
    {
      get => this.MenuField;
      set
      {
        if (object.ReferenceEquals((object) this.MenuField, (object) value))
          return;
        this.MenuField = value;
        this.RaisePropertyChanged(nameof (Menu));
      }
    }

    [DataMember]
    public string MenuXML
    {
      get => this.MenuXMLField;
      set
      {
        if (object.ReferenceEquals((object) this.MenuXMLField, (object) value))
          return;
        this.MenuXMLField = value;
        this.RaisePropertyChanged(nameof (MenuXML));
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
