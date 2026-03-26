using TrabalhoInterdisciplinar.DAO;

namespace TrabalhoInterdisciplinar.Helpers
{
    public class ValidationHelpers
    {
        public static bool IsCPFRegistered(string cpf, string operation = "I")
        {
            StudentDAO studentDAO = new StudentDAO();
            TeacherDAO teacherDAO = new TeacherDAO();
            var studentCpf = studentDAO.GetListing().Find(a => a.Cpf == cpf);

            if (studentCpf == null)
            {
                var teacherCpf = teacherDAO.GetListing().Find(p => p.CPF == cpf);
                if (teacherCpf == null)
                    return true;
                return false;
            }
            return false;
        }

        public static bool IsPhoneRegistered(string phone)
        {
            StudentDAO studentDAO = new StudentDAO();
            TeacherDAO teacherDAO = new TeacherDAO();
            var studentPhone = studentDAO.GetListing().Find(a => a.Phone == phone);
            if (studentPhone == null)
            {
                var teacherPhone = teacherDAO.GetListing().Find(p => p.Phone == phone);
                if (teacherPhone == null)
                    return true;
                return false;
            }
            return false;
        }

        public static bool ValidateCPF(string cpf)
        {
            int[] multiplier1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digit;
            int sum;
            int remainder;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            switch (cpf)
            {
                case "11111111111":
                    return false;
                case "00000000000":
                    return false;
                case "2222222222":
                    return false;
                case "33333333333":
                    return false;
                case "44444444444":
                    return false;
                case "55555555555":
                    return false;
                case "66666666666":
                    return false;
                case "77777777777":
                    return false;
                case "88888888888":
                    return false;
                case "99999999999":
                    return false;
            }

            tempCpf = cpf.Substring(0, 9);
            sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];
            remainder = sum % 11;
            if (remainder < 2)
                remainder = 0;
            else
                remainder = 11 - remainder;
            digit = remainder.ToString();
            tempCpf = tempCpf + digit;
            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];
            remainder = sum % 11;
            if (remainder < 2)
                remainder = 0;
            else
                remainder = 11 - remainder;
            digit = digit + remainder.ToString();
            return cpf.EndsWith(digit);
        }
    }
}