using System;

namespace TrabalhoInterdisciplinar.Models
{
    public class AttendanceViewModel:BaseViewModel
    {
        public int StudentCode { get; set; }
        public int ClassCode { get; set; }
        public string AttendanceStatus { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public string StudentName { get; set; }
        public string ClassDescription { get; set; }
        public string SubjectDescription { get; set; }
    }
}
