// Decompiled with JetBrains decompiler
// Type: Framework.Login.Aplicaciones
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
  [DataContract(Name = "Aplicaciones", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [DebuggerStepThrough]
  [Serializable]
  public class Aplicaciones : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private Grupos[] ListGruposField;
    [OptionalField]
    private Usuarios[] ListUsuariosField;
    [OptionalField]
    private string NombreAppField;
    [OptionalField]
    private Referente ReferentesField;
    [OptionalField]
    private Responsable ResponsableField;
    [OptionalField]
    private ResponsableSuplente ResponsableSupField;
    [OptionalField]
    private ReferenteSuplente SuplenteField;
    [OptionalField]
    private string UrlField;
    [OptionalField]
    private string UrlLocalField;
    [OptionalField]
    private string UrlPublicacionField;
    [OptionalField]
    private char UsaLoginUField;
    [OptionalField]
    private int idAPlicacionField;

    [Browsable(false)]
    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public Grupos[] ListGrupos
    {
      get => this.ListGruposField;
      set
      {
        if (object.ReferenceEquals((object) this.ListGruposField, (object) value))
          return;
        this.ListGruposField = value;
        this.RaisePropertyChanged(nameof (ListGrupos));
      }
    }

    [DataMember]
    public Usuarios[] ListUsuarios
    {
      get => this.ListUsuariosField;
      set
      {
        if (object.ReferenceEquals((object) this.ListUsuariosField, (object) value))
          return;
        this.ListUsuariosField = value;
        this.RaisePropertyChanged(nameof (ListUsuarios));
      }
    }

    [DataMember]
    public string NombreApp
    {
      get => this.NombreAppField;
      set
      {
        if (object.ReferenceEquals((object) this.NombreAppField, (object) value))
          return;
        this.NombreAppField = value;
        this.RaisePropertyChanged(nameof (NombreApp));
      }
    }

    [DataMember]
    public Referente Referentes
    {
      get => this.ReferentesField;
      set
      {
        if (object.ReferenceEquals((object) this.ReferentesField, (object) value))
          return;
        this.ReferentesField = value;
        this.RaisePropertyChanged(nameof (Referentes));
      }
    }

    [DataMember]
    public Responsable Responsable
    {
      get => this.ResponsableField;
      set
      {
        if (object.ReferenceEquals((object) this.ResponsableField, (object) value))
          return;
        this.ResponsableField = value;
        this.RaisePropertyChanged(nameof (Responsable));
      }
    }

    [DataMember]
    public ResponsableSuplente ResponsableSup
    {
      get => this.ResponsableSupField;
      set
      {
        if (object.ReferenceEquals((object) this.ResponsableSupField, (object) value))
          return;
        this.ResponsableSupField = value;
        this.RaisePropertyChanged(nameof (ResponsableSup));
      }
    }

    [DataMember]
    public ReferenteSuplente Suplente
    {
      get => this.SuplenteField;
      set
      {
        if (object.ReferenceEquals((object) this.SuplenteField, (object) value))
          return;
        this.SuplenteField = value;
        this.RaisePropertyChanged(nameof (Suplente));
      }
    }

    [DataMember]
    public string Url
    {
      get => this.UrlField;
      set
      {
        if (object.ReferenceEquals((object) this.UrlField, (object) value))
          return;
        this.UrlField = value;
        this.RaisePropertyChanged(nameof (Url));
      }
    }

    [DataMember]
    public string UrlLocal
    {
      get => this.UrlLocalField;
      set
      {
        if (object.ReferenceEquals((object) this.UrlLocalField, (object) value))
          return;
        this.UrlLocalField = value;
        this.RaisePropertyChanged(nameof (UrlLocal));
      }
    }

    [DataMember]
    public string UrlPublicacion
    {
      get => this.UrlPublicacionField;
      set
      {
        if (object.ReferenceEquals((object) this.UrlPublicacionField, (object) value))
          return;
        this.UrlPublicacionField = value;
        this.RaisePropertyChanged(nameof (UrlPublicacion));
      }
    }

    [DataMember]
    public char UsaLoginU
    {
      get => this.UsaLoginUField;
      set
      {
        if (this.UsaLoginUField.Equals(value))
          return;
        this.UsaLoginUField = value;
        this.RaisePropertyChanged(nameof (UsaLoginU));
      }
    }

    [DataMember]
    public int idAPlicacion
    {
      get => this.idAPlicacionField;
      set
      {
        if (this.idAPlicacionField.Equals(value))
          return;
        this.idAPlicacionField = value;
        this.RaisePropertyChanged(nameof (idAPlicacion));
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
