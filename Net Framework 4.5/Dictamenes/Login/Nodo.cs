using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace UsuarioUniversal.Entities
{
    [XmlRoot("Nodo")]
    public class Nodo
    {
        [XmlElement("url")]
        public String url { get; set; }
        [XmlElement("icono")]
        public String icono { get; set; }
        [XmlElement("descripcion")]
        public String descripcion { get; set; }
        [XmlElement("id")]
        public String id { get; set; }

          
        public Nodo(String id,String link,String icono,String descripcion)
            
        {
            this.id = id;
            this.url = link;
            this.icono = icono;
            this.descripcion = descripcion;

        }
        public Nodo()
        {
        }
        
        [XmlArray("hijos")]
        public List<Nodo> _hijos = new List<Nodo>();
        public  void Agregar(Nodo c)
        {
          _hijos.Add(c);
        }

        public  void Remover(Nodo c)
        {
            _hijos.Remove(c);
        }
        
       
    }
}