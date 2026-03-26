using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Helpers;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class TeacherController : BaseController<TeacherViewModel>
    {
        public TeacherController()
        {
            DAO = new TeacherDAO();
            GeneratesNextId = true;
        }

        public override IActionResult Save(TeacherViewModel model, string operation)
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
                        LoginViewModel loginModel = new LoginViewModel()
                        {
                            ID = model.ID,
                            PasswordHash = PasswordHasher.Encrypt("0001")
                        };
                        LoginDAO loginDAO = new LoginDAO();
                        loginDAO.Insert(loginModel);
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

        protected override void ValidateData(TeacherViewModel teacher, string operation)
        {
            base.ValidateData(teacher, operation);
            if (string.IsNullOrEmpty(teacher.Name))
                ModelState.AddModelError("Name", "Required field.");
            if (string.IsNullOrEmpty(teacher.Email))
                ModelState.AddModelError("Email", "Required field.");
            else
            {
                Regex emailRegex = new Regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$");
                if (!emailRegex.IsMatch(teacher.Email))
                    ModelState.AddModelError("Email", "Invalid email.");
            }
            if (string.IsNullOrEmpty(teacher.Phone))
                ModelState.AddModelError("Phone", "Required field.");
            else
            {
                Regex phoneRegex = new Regex("^\\([1-9]{2}\\) (?:[2-8]|9[1-9])[0-9]{3}\\-[0-9]{4}$");
                if (!phoneRegex.IsMatch(teacher.Phone))
                    ModelState.AddModelError("Phone", "Invalid phone number.");
            }
            if (string.IsNullOrEmpty(teacher.CPF))
                ModelState.AddModelError("CPF", "Required field.");
            else
            {
                Regex cpfRegex = new Regex("^\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}$");
                if (!cpfRegex.IsMatch(teacher.CPF) || !ValidationHelpers.ValidateCPF(teacher.CPF))
                    ModelState.AddModelError("CPF", "Invalid CPF.");
                if (operation == "I" && !ValidationHelpers.IsCPFRegistered(teacher.CPF))
                {
                    ModelState.AddModelError("CPF", "CPF already registered.");
                }
            }
        }

        public override IActionResult Delete(int id)
        {
            try
            {
                SubjectDAO subjectDAO = new SubjectDAO();
                foreach (var item in subjectDAO.GetListing())
                    if (item.TeacherCode == id)
                    {
                        TempData["AlertMessage"] = "Cannot delete. Teacher has open subjects.";
                        return RedirectToAction("Index", "ListingQuery");
                    }
                DAO.Delete(id);
                LoginDAO loginDAO = new LoginDAO();
                loginDAO.Delete(id);
                return RedirectToAction("Index", "ListingQuery");
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }
    }
}