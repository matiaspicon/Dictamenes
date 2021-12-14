// Decompiled with JetBrains decompiler
// Type: Framework.Login.Usuarios
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Dictamenes.Framework.Login
{
  [DataContract(Name = "Usuarios", Namespace = "http://schemas.datacontract.org/2004/07/BEUU")]
  [DebuggerStepThrough]
  [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
  [Serializable]
  public class Usuarios : Personas
  {
    [OptionalField]
    private string NombreUsuarioField;
    [OptionalField]
    private string PasswordField;
    [OptionalField]
    private bool TieneDesafioField;

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
    public string Password
    {
      get => this.PasswordField;
      set
      {
        if (object.ReferenceEquals((object) this.PasswordField, (object) value))
          return;
        this.PasswordField = value;
        this.RaisePropertyChanged(nameof (Password));
      }
    }

    [DataMember]
    public bool TieneDesafio
    {
      get => this.TieneDesafioField;
      set
      {
        if (this.TieneDesafioField.Equals(value))
          return;
        this.TieneDesafioField = value;
        this.RaisePropertyChanged(nameof (TieneDesafio));
      }
    }
  }
}
