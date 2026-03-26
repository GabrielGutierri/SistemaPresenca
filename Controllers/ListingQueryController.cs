using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System;
using TrabalhoInterdisciplinar.Models;
using TrabalhoInterdisciplinar.DAO;
using Microsoft.AspNetCore.Mvc.Filters;
using TrabalhoInterdisciplinar.Enumerators;
using TrabalhoInterdisciplinar.Helpers;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class ListingQueryController : Controller
    {
        protected bool RequiresAuth { get; set; } = true;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (RequiresAuth && !ControllerHelper.IsTeacherLoggedIn(HttpContext.Session))
                context.Result = RedirectToAction("Index", "Home");
            else
            {
                if (ControllerHelper.IsTeacherLoggedIn(HttpContext.Session))
                    ViewBag.TeacherLoggedIn = true;
                else if (ControllerHelper.IsStudentLoggedIn(HttpContext.Session))
                    ViewBag.StudentLoggedIn = true;
                base.OnActionExecuting(context);
            }
        }

        public IActionResult Index()
        {
            try
            {
                StudentDAO studentDAO = new StudentDAO();
                SubjectDAO subjectDAO = new SubjectDAO();
                TeacherDAO teacherDAO = new TeacherDAO();
                ClassDAO classDAO = new ClassDAO();

                ViewBag.StudentCount = studentDAO.GetListing().Count;
                ViewBag.TeacherCount = teacherDAO.GetListing().Count;
                ViewBag.SubjectCount = subjectDAO.GetListing().Count;
                ViewBag.ClassCount = classDAO.GetListing().Count;

                return View("Index");
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.Message));
            }
        }

        public IActionResult GetAdvancedQueryData(EnumTableType tableType, int id)
        {
            try
            {
                switch (tableType)
                {
                    case EnumTableType.Student:
                        return AdvancedStudentQuery(id);
                    case EnumTableType.Teacher:
                        return AdvancedTeacherQuery(id);
                    case EnumTableType.Subject:
                        return AdvancedSubjectQuery(id);
                    case EnumTableType.Lesson:
                        return AdvancedClassQuery(id);
                    default:
                        throw new Exception("This option does not exist");
                }
            }
            catch (Exception error)
            {
                return Json(new { error = true, msg = error.Message });
            }
        }

        private IActionResult AdvancedSubjectQuery(int id)
        {
            try
            {
                SubjectDAO subjectDAO = new SubjectDAO();
                if (string.IsNullOrEmpty(id.ToString()))
                    id = 0;
                var list = subjectDAO.AdvancedQuery(id);

                if (list.Count != 0)
                    return PartialView("pvGridSubject", list);
                else
                    return PartialView("pvGridNoResults");
            }
            catch (Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        private IActionResult AdvancedTeacherQuery(int id)
        {
            try
            {
                TeacherDAO teacherDAO = new TeacherDAO();
                if (string.IsNullOrEmpty(id.ToString()))
                    id = 0;
                var list = teacherDAO.AdvancedQuery(id);

                if (list.Count != 0)
                    return PartialView("pvGridTeacher", list);
                else
                    return PartialView("pvGridNoResults");
            }
            catch (Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        public IActionResult AdvancedStudentQuery(int id)
        {
            try
            {
                StudentDAO studentDAO = new StudentDAO();
                if (string.IsNullOrEmpty(id.ToString()))
                    id = 0;
                var list = studentDAO.AdvancedQuery(id);

                if (list.Count != 0)
                    return PartialView("pvGridStudent", list);
                else
                    return PartialView("pvGridNoResults");
            }
            catch (Exception error)
            {
                return Json(new { error = true, msg = error.Message });
            }
        }

        public IActionResult AdvancedClassQuery(int id)
        {
            try
            {
                ClassDAO classDAO = new ClassDAO();
                if (string.IsNullOrEmpty(id.ToString()))
                    id = 0;
                var list = classDAO.AdvancedQuery(id);

                if (list.Count != 0)
                    return PartialView("pvGridClass", list);
                else
                    return PartialView("pvGridNoResults");
            }
            catch (Exception error)
            {
                return Json(new { error = true, msg = error.Message });
            }
        }
    }
}