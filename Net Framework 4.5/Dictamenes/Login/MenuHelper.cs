using System;
using System.Linq;
using UsuarioUniversal.Entities;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;
using NegUU;
using System.Xml.Xsl;


namespace WCFLoginUniversal
{
    public class MenuHelper
    {
        public static String buildMenu(DataTable dt, int idapp, int idusua, int idgrupo, PregFrec opf)
        {
            var root = new Nodo("1", "/", null, "Menu");
            if (dt.Rows.Count == 0)
                return null;
            foreach (DataRow row in dt.Rows)
            {
                //if (row["ItemLink"] != null)
                //{
                //Esta Condicion indica que son padres
                if (!row["ItemOrden"].Equals(row["ItemPadre"])) continue;

                string imagen = "";
                string link = "";

                if (row["ItemLink"]!=DBNull.Value)
                    link = (string)row["ItemLink"];

                if (row["ItemImagen"] !=DBNull.Value)
                    imagen = (string)row["ItemImagen"];

                            
                var current = new Nodo(row["ItemOrden"].ToString(), link, imagen, (string)row["ItemDescripcion"]);
                //Agregamos el item al menu principal
                root.Agregar(current);
                AddMenuItem(current, dt, idapp, idusua, idgrupo, opf);
                //}
            }
            var serializer = new XmlSerializer(typeof(Nodo));
            var memStream = new MemoryStream();
            serializer.Serialize(memStream, root);
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1"); // UTF8Encoding.UTF8)


            var xmlTextWriter = new XmlTextWriter(memStream, encoding);

            return ISOByteArrayToString(((MemoryStream)xmlTextWriter.BaseStream).ToArray());
        }

        private static String ISOByteArrayToString(Byte[] characters)
        {
            Encoding encoding = Encoding.GetEncoding("ISO-8859-1"); // UTF8Encoding.UTF8)
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            var encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static void AddMenuItem(Nodo nodo, DataTable dt, int idapp, int idusua, int idgrupo, PregFrec opf)
        {
            string itemlink = "";

            foreach (DataRow row in from DataRow row in dt.Rows where row["ItemPadre"].ToString().Equals(nodo.id) && !row["ItemOrden"].Equals(row["ItemPadre"]) where row["ItemLink"] != null select row)
            {
                if (row["ItemLink"].ToString().Substring(0, 1) == "#" && opf!=null)
                {
                    byte[] encriptado = Encript.ENC(idusua + "|" + idapp + "|" + idgrupo + "|" + opf.IdPF);

                    string strencript = encriptado.Aggregate("", (current, b) => current + b.ToString() + "j");

                    string[] lst = row["ItemLink"].ToString().Split('#');

                    switch (lst[2])
                    {
                        case "U":
                            if (lst[3] == "")
                            {
                                itemlink = System.Configuration.ConfigurationManager.AppSettings["rutaMP"] + strencript;
                            }
                            else
                            {
                                itemlink = System.Configuration.ConfigurationManager.AppSettings["rutaF"] + strencript;
                            }
                            break;
                        case "M":
                            if (lst[3] == "")
                            {
                                itemlink = System.Configuration.ConfigurationManager.AppSettings["rutaFaq"] + strencript;
                            }
                            else
                            {
                                itemlink = System.Configuration.ConfigurationManager.AppSettings["rutaF"] + strencript + "&Faqs=1";
                            }
                            break;
                    }
                }
                else
                {
                    itemlink = row["ItemLink"].ToString();
                }

                string imagen = "";

                if (row["ItemImagen"] != DBNull.Value)
                    imagen = (string)row["ItemImagen"];

                var child = new Nodo((string)row["ItemOrden"], itemlink, imagen, (string)row["ItemDescripcion"]);
                nodo.Agregar(child);
                AddMenuItem(child, dt, idapp, idusua, idgrupo, opf);
            }
        }

        /* arma el menu con los datos que se pasan. */
        public static String armarMenu(String aXmlString, String aXslString)
        {
            #region Obtener el XML del String del Menu
            // Encode the XML string in a UTF-8 byte array
            byte[] lEncodedString = Encoding.UTF8.GetBytes(aXmlString);

            // Put the byte array into a stream and rewind it to the beginning
            var lMemStream = new MemoryStream(lEncodedString);
            lMemStream.Flush();
            lMemStream.Position = 0;

            // Build the XmlDocument from the MemorySteam of UTF-8 encoded bytes
            var lXmlDoc = new XmlDocument();
            lXmlDoc.Load(lMemStream);
            #endregion
            #region Obtener el XSL del String de la URL que pasen.
            // Encode the XML string in a UTF-8 byte array
            byte[] lEncodedStringXSL = Encoding.UTF8.GetBytes(aXslString);

            // Put the byte array into a stream and rewind it to the beginning
            var lMemStreamXSL = new MemoryStream(lEncodedStringXSL);
            lMemStreamXSL.Flush();
            lMemStreamXSL.Position = 0;

            // Build the XmlDocument from the MemorySteam of UTF-8 encoded bytes
            var lXSLDoc = new XmlDocument();
            lXSLDoc.Load(lMemStreamXSL);
            #endregion
            #region Hago la transformacion
            var lXslt = new XslTransform();
            lXslt.Load(lXSLDoc);
            var lWritter = new StringWriter();
            lXslt.Transform(lXmlDoc.CreateNavigator(), null, lWritter);
            #endregion
            return lWritter.ToString();
        }

        /* arma el menu con los datos que se pasan. 
         * si no viene el xsl usa el que tiene aca.
         */
        public static String armarMenu(String aXmlString)
        {
            #region Obtener el XML del String del Menu
            Encoding lEncoding = Encoding.GetEncoding("ISO-8859-1");
            byte[] lEncodedString = lEncoding.GetBytes(depurarXMLMenu(aXmlString));
            // Put the byte array into a stream and rewind it to the beginning
            var lMemStream = new MemoryStream(lEncodedString);
            lMemStream.Flush();
            lMemStream.Position = 0;

            // Build the XmlDocument from the MemorySteam of UTF-8 encoded bytes
            var lXmlDoc = new XmlDocument();
            lXmlDoc.Load(lMemStream);
            #endregion
            #region Obtener el XSL del String de la URL que pasen.
            const string lUrlFromMenu = "http://seguro3.ssn.gov.ar/UsuarioUniversalWS/xslt/menu.xsl";
            var lXSLReader = new XmlTextReader(lUrlFromMenu);
            var lXSLDoc = new XmlDocument();
            lXSLDoc.Load(lXSLReader);
            #endregion
            #region Hago la transformacion
            var lXslt = new XslTransform();
            lXslt.Load(lXSLDoc);
            var lWritter = new StringWriter();
            lXslt.Transform(lXmlDoc.CreateNavigator(), null, lWritter);
            #endregion
            return lWritter.ToString();

        }

        private static String depurarXMLMenu(String aXmlMenu)
        {
            return aXmlMenu.Replace("\n", "").Replace("\\\"", "");
        }
    }
}