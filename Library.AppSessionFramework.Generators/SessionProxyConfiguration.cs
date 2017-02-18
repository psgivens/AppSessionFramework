using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PhillipScottGivens.Library.AppSessionFramework.Generators
{
    public class SessionProxyConfiguration
    {
        public string Namespace { get; set; }

        [XmlArrayItem("Assembly")]
        public AssemblyPointer[] Assemblies { get; set; }

        static public void SerializeToXML(SessionProxyConfiguration configInfo)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SessionProxyConfiguration));
            TextWriter textWriter = new StreamWriter(@"assembly.xml");
            serializer.Serialize(textWriter, configInfo);
            textWriter.Close();
        }

        static public SessionProxyConfiguration DeserializeFromXML()
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SessionProxyConfiguration));
            TextReader textReader = new StreamReader(@"assembly.xml");

            var configuration = (SessionProxyConfiguration)deserializer.Deserialize(textReader);
            textReader.Close();

            return configuration;
        }
    }

    public class AssemblyPointer
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public string GetFullPath()
        {
            return System.IO.Path.Combine(Path, Name + ".dll");
        }
    }
}
