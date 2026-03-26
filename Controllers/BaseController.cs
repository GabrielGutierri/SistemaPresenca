using Microsoft.AspNetCore.Mvc;
using System;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using TrabalhoInterdisciplinar.Helpers;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class BaseController<T> : Controller where T : BaseViewModel
    {
        protected BaseDAO<T> DAO { get; set; }
        protected bool GeneratesNextId { get; set; }
        protected string IndexViewName { get; set; } = "index";
        protected string FormViewName { get; set; } = "Form";
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

        public virtual IActionResult Index()
        {
            try
            {
                var list = DAO.GetListing();
                return View(IndexViewName, list);
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }

        public virtual IActionResult Create()
        {
            try
            {
                ViewBag.Operation = "I";
                T model = Activator.CreateInstance(typeof(T)) as T;
                PopulateViewData("I", model);
                return View(FormViewName, model);
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }

        protected virtual void PopulateViewData(string operation, T model)
        {
            if (GeneratesNextId && operation == "I")
                model.ID = DAO.NextId();
        }

        public virtual IActionResult Save(T model, string operation)
        {
            try
            {
                ValidateData(model, operation);
                if (ModelState.IsValid == false)
                {
                    ViewBag.Operation = operation;
                    PopulateViewData(operation, model);
                    return View(FormViewName, model);
                }
                else
                {
                    if (operation == "I")
                    {
                        DAO.Insert(model);
                        TempData["AlertMessage"] = "Data saved successfully...!";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        DAO.Update(model);
                        return RedirectToAction("Index", "ListingQuery");
                    }
                }
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }

        protected virtual void ValidateData(T model, string operation)
        {
            ModelState.Clear();
            if (operation == "I" && DAO.Query(model.ID) != null)
                ModelState.AddModelError("Id", "ID is already in use!");
            if (operation == "A" && DAO.Query(model.ID) == null)
                ModelState.AddModelError("Id", "This record does not exist!");
            if (model.ID <= 0)
                ModelState.AddModelError("Id", "Invalid ID!");
        }

        public IActionResult Edit(int id)
        {
            try
            {
                ViewBag.Operation = "A";
                var model = DAO.Query(id);
                if (model == null)
                    return RedirectToAction("Index", "ListingQuery");
                else
                {
                    PopulateViewData("A", model);
                    return View(FormViewName, model);
                }
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }

        public virtual IActionResult Delete(int id)
        {
            try
            {
                DAO.Delete(id);
                return RedirectToAction("Index", "ListingQuery");
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }
    }
}