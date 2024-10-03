using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BuyingCatalogueFunctionTests.OrganisationImport;

public static class TestExtensions
{
    public static string ToXml<T>(this T val)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));

        var settings = new XmlWriterSettings()
        {
            Encoding = new UnicodeEncoding(false, true),
            Indent = false,
            OmitXmlDeclaration = false
        };

        using var stringWriter = new UTF8StringWriter();
        using var xmlTextWriter = XmlWriter.Create(stringWriter, settings);

        xmlSerializer.Serialize(stringWriter, val);

        return stringWriter.ToString();
    }

    public static Stream ToStream(this string val) => new MemoryStream(Encoding.UTF8.GetBytes(val));

    private sealed class UTF8StringWriter : StringWriter
    {
        public UTF8StringWriter() : base(new(), CultureInfo.InvariantCulture)
        {
        }

        /// <inheritdoc />
        public override Encoding Encoding => new UTF8Encoding();
    }

}
