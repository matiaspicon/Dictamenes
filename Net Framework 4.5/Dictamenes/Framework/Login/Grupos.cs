// Decompiled with JetBrains decompiler
// Type: Framework.Login.Grupos
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
  [DebuggerStepThrough]
  [DataContract(Name = "Grupos", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [Serializable]
  public class Grupos : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private Aplicaciones AplicacionField;
    [OptionalField]
    private string GrupoDescripcionField;
    [OptionalField]
    private int IdGrupoField;

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
    public string GrupoDescripcion
    {
      get => this.GrupoDescripcionField;
      set
      {
        if (object.ReferenceEquals((object) this.GrupoDescripcionField, (object) value))
          return;
        this.GrupoDescripcionField = value;
        this.RaisePropertyChanged(nameof (GrupoDescripcion));
      }
    }

    [DataMember]
    public int IdGrupo
    {
      get => this.IdGrupoField;
      set
      {
        if (this.IdGrupoField.Equals(value))
          return;
        this.IdGrupoField = value;
        this.RaisePropertyChanged(nameof (IdGrupo));
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
