using System.IO;
using System.Text;
using SyncWorks.Models;

namespace SyncWorks.Methods
{
    public class FileExporter
    {
        public void ExportToCsv(ExtractedScheduleData data, string filePath)
        {
            var sb = new StringBuilder();

            foreach (var headerRow in data.Headers)
            {
                sb.AppendLine(string.Join(";", headerRow));
            }

            foreach (var row in data.Rows)
            {
                sb.AppendLine(string.Join(";", row));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
    }
}
