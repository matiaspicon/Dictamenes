// Decompiled with JetBrains decompiler
// Type: Framework.Configuration.Generics.GenericCollection`1
// Assembly: Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: BBD532E9-8A6A-474E-B9F7-ECCE8755DE6F
// Assembly location: C:\Users\mpicon\source\repos\Dictamenes\Net Framework 4.5\Dictamenes\bin\Framework.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Dictamenes.Framework.Configuration.Generics
{
  public class GenericCollection<T> : ConfigurationElementCollection, IEnumerable<T>, IEnumerable
    where T : ConfigurationElement, new()
  {
    private readonly List<T> _elements = new List<T>();

    protected override ConfigurationElement CreateNewElement()
    {
      T newElement = new T();
      this._elements.Add(newElement);
      return (ConfigurationElement) newElement;
    }

    protected override object GetElementKey(ConfigurationElement element) => (object) this._elements.Find((Predicate<T>) (e => e.Equals((object) element)));

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this._elements.GetEnumerator();

    public T this[object index] => this._elements.FirstOrDefault<T>((Func<T, bool>) (c => ((IElement) (object) c).GetKey().Equals(index)));

    public object Base(string index) => this[index];
  }
}
