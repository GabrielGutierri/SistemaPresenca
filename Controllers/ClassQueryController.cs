using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Enumerators;
using TrabalhoInterdisciplinar.Helpers;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class ClassQueryController : Controller
    {
        protected bool RequiresAuth { get; set; } = true;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (RequiresAuth && (!ControllerHelper.IsStudentLoggedIn(HttpContext.Session) &&
                !ControllerHelper.IsTeacherLoggedIn(HttpContext.Session)))
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
                PrepareFilterData();

                ClassDAO classDAO = new ClassDAO();
                double active = 0;
                double finished = 0;
                double future = 0;

                if (classDAO.GetListing().Count != 0)
                {
                    foreach (var item in classDAO.GetListing())
                    {
                        if (DateTime.Now < item.ClassDateTime.AddMinutes(15) && DateTime.Now > item.ClassDateTime.AddMinutes(-15))
                        {
                            active++;
                        }
                        else if (DateTime.Now > item.ClassDateTime.AddMinutes(15))
                        {
                            finished++;
                        }
                        else if (DateTime.Now < item.ClassDateTime.AddMinutes(-15))
                        {
                            future++;
                        }
                    }

                    if (active != 0)
                    {
                        ViewBag.Active = Convert.ToInt16((active / classDAO.GetListing().Count) * 100);
                    }
                    else
                    {
                        ViewBag.Active = 0;
                    }
                    ViewBag.Finished = Convert.ToInt16((finished / classDAO.GetListing().Count) * 100);
                    ViewBag.Future = Convert.ToInt16((future / classDAO.GetListing().Count) * 100);
                }

                return View("Index");
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.Message));
            }
        }

        public IActionResult AdvancedClassQuery(DateTime startDate, DateTime endDate, int subjectId, EnumClassStatus classStatus)
        {
            try
            {
                ClassDAO classDAO = new ClassDAO();
                if (startDate.Date == Convert.ToDateTime("01/01/0001"))
                    startDate = SqlDateTime.MinValue.Value;
                if (endDate.Date == Convert.ToDateTime("01/01/0001"))
                    endDate = SqlDateTime.MaxValue.Value;
                if (string.IsNullOrEmpty(subjectId.ToString()))
                    subjectId = 0;

                var list = classDAO.AdvancedQuery(startDate, endDate, subjectId);

                foreach (var item in list)
                {
                    if (DateTime.Now < item.ClassDateTime.AddMinutes(15) && DateTime.Now > item.ClassDateTime.AddMinutes(-15))
                    {
                        item.Status = EnumClassStatus.Active;
                    }
                    else if (DateTime.Now > item.ClassDateTime.AddMinutes(15))
                    {
                        item.Status = EnumClassStatus.Finished;
                    }
                    else if (DateTime.Now < item.ClassDateTime.AddMinutes(-15))
                    {
                        item.Status = EnumClassStatus.Future;
                    }
                }

                list = FilterByStatus(list, classStatus);

                if (list.Count != 0)
                {
                    return PartialView("pvGridClass", list);
                }
                else
                {
                    return PartialView("pvGridNoResults");
                }
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }

        public List<ClassViewModel> FilterByStatus(List<ClassViewModel> list, EnumClassStatus classStatus)
        {
            int classStatusInt = (int)classStatus;
            var filteredList = new List<ClassViewModel>();
            foreach (var item in list)
            {
                if (classStatusInt == 1 && (int)item.Status == 1)
                    filteredList.Add(item);
                else if (classStatusInt == 2 && (int)item.Status == 2)
                    filteredList.Add(item);
                else if (classStatusInt == 3 && (int)item.Status == 3)
                    filteredList.Add(item);
                else if (classStatusInt == 0)
                    filteredList.Add(item);
            }
            return filteredList;
        }

        private void PrepareFilterData()
        {
            try
            {
                SubjectDAO subjectDAO = new SubjectDAO();
                var subjects = subjectDAO.GetListing();
                List<SelectListItem> subjectList = new List<SelectListItem>();
                subjectList.Add(new SelectListItem("Select a subject...", "0"));
                foreach (var subject in subjects)
                {
                    SelectListItem item = new SelectListItem(subject.Description, subject.ID.ToString());
                    subjectList.Add(item);
                }
                ViewBag.Subjects = subjectList;
            }
            catch (Exception err)
            {
                throw new Exception("Error listing filter data... " + err.Message);
            }
        }
    }
}