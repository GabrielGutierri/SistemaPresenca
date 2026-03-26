
using System.Collections.Generic;

namespace TrabalhoInterdisciplinar.Models
{
    public class AttendanceResponseViewModel
    {
        public string type { get; set; }
        public string value { get; set; }
        public IDictionary<string, MetaData> metadata { get; set; }
    }
    public class MetaData
    {

    }

    public class TimeStant
    {
        public string type { get; set; }
        public string value { get; set; }
    }
}
