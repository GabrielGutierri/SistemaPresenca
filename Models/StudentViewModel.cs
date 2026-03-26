using Microsoft.AspNetCore.Http;
using System;

namespace TrabalhoInterdisciplinar.Models
{
    public class StudentViewModel:BaseViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Cpf { get; set; }
        public int BiometricId { get; set; }
        public IFormFile Image { get; set; }
        public byte[] ImageBytes { get; set; }
        public string ImageBase64
        {
            get
            {
                if (ImageBytes != null)
                    return Convert.ToBase64String(ImageBytes);
                else
                    return string.Empty;
            }
        }
    }
}
