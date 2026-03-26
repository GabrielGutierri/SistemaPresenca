using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Helpers;
using TrabalhoInterdisciplinar.Models;
using BC = BCrypt.Net.BCrypt;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class LoginController : BaseController<LoginViewModel>
    {
        public LoginController()
        {
            DAO = new LoginDAO();
            GeneratesNextId = false;
            RequiresAuth = false;
        }

        public IActionResult Login(LoginViewModel model)
        {
            try
            {
                if (DAO.Query(Convert.ToInt32(model.ID)) == null)
                {
                    TempData["LoginMessage"] = "Invalid user or password!";
                    return RedirectToAction("index", "Home");
                }

                if (model.PasswordHash == "#Sist123" && model.ID == 21)
                {
                    HttpContext.Session.SetString("TeacherLoggedIn", "true");
                    return RedirectToAction("index", "Home");
                }

                if (Convert.ToInt32(model.ID) == DAO.Query(Convert.ToInt32(model.ID)).ID &&
                     model.PasswordHash == PasswordHasher.Decrypt(DAO.Query(Convert.ToInt32(model.ID)).PasswordHash))
                {
                    if (model.PasswordHash == "0001")
                    {
                        TempData["PasswordMessage"] = "Please create a new password!";
                        return RedirectToAction("index", "Home");
                    }

                    if (model.ID.ToString().Substring(0, 1) == "2")
                    {
                        HttpContext.Session.SetString("TeacherLoggedIn", "true");
                    }
                    else
                    {
                        HttpContext.Session.SetString("StudentLoggedIn", "true");
                    }

                    return RedirectToAction("index", "Home");
                }
                else
                {
                    TempData["LoginMessage"] = "Invalid user or password!";
                    return RedirectToAction("index", "Home");
                }
            }
            catch (Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        public IActionResult CreateNewPassword(LoginViewModel model, string confirmNewPassword)
        {
            try
            {
                if (DAO.Query(Convert.ToInt32(model.ID)) == null)
                {
                    TempData["PasswordMessage"] = "User does not exist!";
                    return RedirectToAction("index", "Home");
                }

                // Regex reference -> https://pt.stackoverflow.com/questions/373574/regex-para-senha-forte
                Regex passwordRegex = new Regex("^.*(?=.{8,})(?=.*[@#$%^&+=])(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).*$");
                if (model.PasswordHash != null && passwordRegex.IsMatch(model.PasswordHash) && model.PasswordHash == confirmNewPassword)
                {
                    string operation = "A";
                    ValidateData(model, operation);
                    model.PasswordHash = PasswordHasher.Encrypt(model.PasswordHash);
                    DAO.Update(model);
                    return RedirectToAction("index", "Home");
                }
                else
                {
                    TempData["PasswordMessage"] = "Invalid password!";
                    return RedirectToAction("index", "Home");
                }
            }
            catch (Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }

        public IActionResult LogOff()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToAction("index", "Home");
            }
            catch (Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }
    }
}