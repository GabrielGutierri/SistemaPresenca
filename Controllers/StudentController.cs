using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TrabalhoInterdisciplinar.HelixConnection;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Helpers;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class StudentController : BaseController<StudentViewModel>
    {
        public StudentController()
        {
            DAO = new StudentDAO();
            GeneratesNextId = true;
        }

        public byte[] ConvertImageToByte(IFormFile file)
        {
            if (file != null)
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            else
                return null;
        }

        public override IActionResult Save(StudentViewModel model, string operation)
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
                        StudentDAO newStudentDAO = new StudentDAO();
                        model.BiometricId = newStudentDAO.NextBiometricId();
                        DAO.Insert(model);
                        LoginViewModel loginModel = new LoginViewModel()
                        {
                            ID = model.ID,
                            PasswordHash = PasswordHasher.Encrypt("0001")
                        };
                        LoginDAO loginDAO = new LoginDAO();
                        loginDAO.Insert(loginModel);
                        TempData["AlertMessage"] = "Data saved successfully...! ";
                        MQTTConnection mqttConnection = new MQTTConnection();
                        mqttConnection.ProvisionMQTT(model);
                        mqttConnection.RegisterMQTT(model);
                        mqttConnection.PublishCreateMQTT(model);
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

        protected override void ValidateData(StudentViewModel student, string operation)
        {
            base.ValidateData(student, operation);
            ValidateDataPartTwo(student, operation);
            if (student.Image == null && operation == "I")
                ModelState.AddModelError("Image", "Please choose an image.");
            if (student.Image != null && student.Image.Length / 1024 / 1024 >= 2)
                ModelState.AddModelError("Image", "Image limited to 2 MB.");
            if (ModelState.IsValid)
            {
                if (operation == "A" && student.Image == null)
                {
                    StudentViewModel existingStudent = DAO.Query(student.ID);
                    student.ImageBytes = existingStudent.ImageBytes;
                }
                else
                {
                    student.ImageBytes = ConvertImageToByte(student.Image);
                }
            }
        }

        private void ValidateDataPartTwo(StudentViewModel student, string operation)
        {
            if (string.IsNullOrEmpty(student.Name))
                ModelState.AddModelError("Name", "Required field.");
            if (string.IsNullOrEmpty(student.Email))
                ModelState.AddModelError("Email", "Required field.");
            else
            {
                Regex emailRegex = new Regex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$");
                if (!emailRegex.IsMatch(student.Email))
                    ModelState.AddModelError("Email", "Invalid email.");
            }
            if (string.IsNullOrEmpty(student.Phone))
                ModelState.AddModelError("Phone", "Required field.");
            else
            {
                Regex phoneRegex = new Regex("^\\([1-9]{2}\\) (?:[2-8]|9[1-9])[0-9]{3}\\-[0-9]{4}$");
                if (!phoneRegex.IsMatch(student.Phone))
                    ModelState.AddModelError("Phone", "Invalid phone number.");
            }
            if (string.IsNullOrEmpty(student.Cpf))
                ModelState.AddModelError("Cpf", "Required field.");
            else
            {
                Regex cpfRegex = new Regex("^\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}$");
                if (!cpfRegex.IsMatch(student.Cpf) || !ValidationHelpers.ValidateCPF(student.Cpf))
                    ModelState.AddModelError("Cpf", "Invalid CPF.");
                if (operation == "I" && !ValidationHelpers.IsCPFRegistered(student.Cpf))
                {
                    ModelState.AddModelError("Cpf", "CPF already registered.");
                }
            }
        }

        public override IActionResult Delete(int id)
        {
            try
            {
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