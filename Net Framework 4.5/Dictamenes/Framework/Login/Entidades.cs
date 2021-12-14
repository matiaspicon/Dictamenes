// Decompiled with JetBrains decompiler
// Type: Framework.Login.Entidades
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
  [DataContract(Name = "Entidades", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Serializable]
  public class Entidades : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private string CodigoField;
    [OptionalField]
    private string DenominacionField;
    [OptionalField]
    private string DenominacionCortaField;
    [OptionalField]
    private TipoEntidad TipoEntidadField;

    [Browsable(false)]
    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public string Codigo
    {
      get => this.CodigoField;
      set
      {
        if (object.ReferenceEquals((object) this.CodigoField, (object) value))
          return;
        this.CodigoField = value;
        this.RaisePropertyChanged(nameof (Codigo));
      }
    }

    [DataMember]
    public string Denominacion
    {
      get => this.DenominacionField;
      set
      {
        if (object.ReferenceEquals((object) this.DenominacionField, (object) value))
          return;
        this.DenominacionField = value;
        this.RaisePropertyChanged(nameof (Denominacion));
      }
    }

    [DataMember]
    public string DenominacionCorta
    {
      get => this.DenominacionCortaField;
      set
      {
        if (object.ReferenceEquals((object) this.DenominacionCortaField, (object) value))
          return;
        this.DenominacionCortaField = value;
        this.RaisePropertyChanged(nameof (DenominacionCorta));
      }
    }

    [DataMember]
    public TipoEntidad TipoEntidad
    {
      get => this.TipoEntidadField;
      set
      {
        if (object.ReferenceEquals((object) this.TipoEntidadField, (object) value))
          return;
        this.TipoEntidadField = value;
        this.RaisePropertyChanged(nameof (TipoEntidad));
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
