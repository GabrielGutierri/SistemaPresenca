using System;

namespace TrabalhoInterdisciplinar.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Error { get; set; }

        public ErrorViewModel(string message)
        {
            Error = message;
        }

        public ErrorViewModel()
        {

        }
    }
}
