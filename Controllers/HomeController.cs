using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Helpers;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            HelperDAO.StringCX = _config.GetConnectionString("SQLServerDbConnectionString");
        }

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