using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PharmacySystemInfrastructure
{
    internal static class XmlExtension
    {
        public static string PrettyXml(TextWriter writer)
        {
            var stringBuilder = new StringBuilder();
            var element = XElement.Parse(writer.ToString());
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                NewLineOnAttributes = true
            };
            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }
            return stringBuilder.ToString();
        }
    }

    internal static class FileExtension
    {
        public static void CompressToZip(List<string> filesToBeCompressed, string targetPath)
        {
            var zs = new ZipOutputStream(File.Create(targetPath));
            zs.SetLevel(9);
            foreach (var file in filesToBeCompressed)
            {
                var s = File.OpenRead(file);
                var buffer = new byte[s.Length];
                s.Read(buffer, 0, buffer.Length);
                var entry = new ZipEntry(Path.GetFileName(file))
                {
                    DateTime = DateTime.Now,
                    Size = s.Length
                };
                s.Close();
                zs.PutNextEntry(entry);
                zs.Write(buffer, 0, buffer.Length);
            }
            zs.Finish();
            zs.Close();
        }
    }
}
