using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TrabalhoInterdisciplinar.Helpers;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            try
            {
                if (ControllerHelper.IsTeacherLoggedIn(HttpContext.Session))
                    ViewBag.TeacherLoggedIn = true;
                else if (ControllerHelper.IsStudentLoggedIn(HttpContext.Session))
                    ViewBag.StudentLoggedIn = true;
                return View();
            }
            catch (Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}