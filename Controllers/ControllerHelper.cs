using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class ControllerHelper
    {
        public static Boolean IsStudentLoggedIn(ISession session)
        {
            string loggedIn = session.GetString("StudentLoggedIn");
            if (loggedIn == null)
                return false;
            else
                return true;
        }

        public static Boolean IsTeacherLoggedIn(ISession session)
        {
            string loggedIn = session.GetString("TeacherLoggedIn");
            if (loggedIn == null || loggedIn == "StudentLoggedIn")
                return false;
            else
                return true;
        }
    }
}