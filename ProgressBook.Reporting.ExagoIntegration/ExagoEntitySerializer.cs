namespace ProgressBook.Reporting.ExagoIntegration
{
    using System;
    using System.Text;
    using System.Xml;
    using System.Linq;
    using ProgressBook.Reporting.Data.Entities;

    public interface IExagoEntitySerializer
    {
        string ToXml(ReportViewInfo reportViewInfo, string dataSourceId);
    }

    public class ExagoEntitySerializer : IExagoEntitySerializer
    {
        public string ToXml(ReportViewInfo reportViewInfo, string dataSourceId)
        {
            var sb = new StringBuilder();
            var writer = XmlWriter.Create(sb, new XmlWriterSettings {OmitXmlDeclaration = true, Indent = true});

            writer.WriteStartDocument();
            writer.WriteStartElement("entity");

            writer.WriteStartElement("entity_name");
            writer.WriteString(reportViewInfo.EntityName);
            writer.WriteEndElement();

            writer.WriteStartElement("db_name");
            writer.WriteString(reportViewInfo.ViewName);
            writer.WriteEndElement();

            writer.WriteStartElement("category");
            writer.WriteString(reportViewInfo.Category);
            writer.WriteEndElement();

            writer.WriteStartElement("datasource_id");
            writer.WriteString(dataSourceId);
            writer.WriteEndElement();

            writer.WriteStartElement("object_type");
            writer.WriteString("view");
            writer.WriteEndElement();

            writer.WriteStartElement("schema");
            writer.WriteString(reportViewInfo.SchemaName);
            writer.WriteEndElement();

            if (reportViewInfo.MetaColumns != null && reportViewInfo.MetaColumns.Any())
            {
                // for this view, we only explicitly set its schema access type to "Metadata" if the view has a Metadata extended property. 
                // Otherwise use global default.
                writer.WriteStartElement("schema_access_type");
                writer.WriteString("Metadata");
                writer.WriteEndElement();

                foreach (var kvp in reportViewInfo.MetaColumns)
                {
                    writer.WriteStartElement("column_metadata");

                    writer.WriteStartElement("col_name");
                    writer.WriteString(kvp.Key);
                    writer.WriteEndElement();
                    writer.WriteStartElement("col_type");
                    writer.WriteString(kvp.Value);
                    writer.WriteEndElement();

                    writer.WriteEndElement(); // </column_metadata>
                }
            }

            if (!string.IsNullOrEmpty(reportViewInfo.KeyColumns))
            {
                var keyColList = reportViewInfo.KeyColumns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                foreach (var keyCol in keyColList)
                {
                    writer.WriteStartElement("key");

                    writer.WriteStartElement("col_name");
                    writer.WriteString(keyCol.Trim());
                    writer.WriteEndElement();

                    writer.WriteEndElement(); // </key>
                }
            }

            if (!string.IsNullOrEmpty(reportViewInfo.TenantColumns))
            {
                var tenantColList = reportViewInfo.TenantColumns.Split(new[] {','},
                                                                       StringSplitOptions.RemoveEmptyEntries);

                foreach (var tenantCol in tenantColList)
                {
                    writer.WriteStartElement("tenant");

                    writer.WriteStartElement("col_name");
                    writer.WriteString(tenantCol.Trim());
                    writer.WriteEndElement();

                    writer.WriteStartElement("parameter_id");
                    writer.WriteString(tenantCol.Trim());
                    writer.WriteEndElement();

                    writer.WriteEndElement(); // </tenant>
                }
            }

            writer.WriteEndElement(); // </entity>
            writer.WriteEndDocument();
            writer.Flush();

            return sb.ToString();
        }
    }
}