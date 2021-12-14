// Decompiled with JetBrains decompiler
// Type: Framework.Login.TipoEntidad
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
  [DataContract(Name = "TipoEntidad", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Serializable]
  public class TipoEntidad : IExtensibleDataObject, INotifyPropertyChanged
  {
    [NonSerialized]
    private ExtensionDataObject extensionDataField;
    [OptionalField]
    private string DescripcionField;
    [OptionalField]
    private int RegistroField;
    [OptionalField]
    private string ValorField;

    [Browsable(false)]
    public ExtensionDataObject ExtensionData
    {
      get => this.extensionDataField;
      set => this.extensionDataField = value;
    }

    [DataMember]
    public string Descripcion
    {
      get => this.DescripcionField;
      set
      {
        if (object.ReferenceEquals((object) this.DescripcionField, (object) value))
          return;
        this.DescripcionField = value;
        this.RaisePropertyChanged(nameof (Descripcion));
      }
    }

    [DataMember]
    public int Registro
    {
      get => this.RegistroField;
      set
      {
        if (this.RegistroField.Equals(value))
          return;
        this.RegistroField = value;
        this.RaisePropertyChanged(nameof (Registro));
      }
    }

    [DataMember]
    public string Valor
    {
      get => this.ValorField;
      set
      {
        if (object.ReferenceEquals((object) this.ValorField, (object) value))
          return;
        this.ValorField = value;
        this.RaisePropertyChanged(nameof (Valor));
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
