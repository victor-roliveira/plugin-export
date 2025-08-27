namespace SyncWorks.Models
{
    public class ExtractedScheduleData
    {
        public string ScheduleName { get; set; }
        public List<List<string>> Headers { get; set; }
        public List<List<string>> Rows { get; set; }

        public ExtractedScheduleData()
        {
            Headers = new List<List<string>>();
            Rows = new List<List<string>>();
        }
    }
}
