using System;
using TrabalhoInterdisciplinar.Enumerators;

namespace TrabalhoInterdisciplinar.Models
{
    public class ClassViewModel: BaseViewModel
    {
        public string Content { get; set; }
        public DateTime ClassDateTime { get; set; }
        public int SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public EnumClassStatus Status { get; set; }
    }

}
