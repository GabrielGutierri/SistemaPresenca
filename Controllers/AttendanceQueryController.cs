using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TrabalhoInterdisciplinar.HelixConnection;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Enumerators;
using TrabalhoInterdisciplinar.HelixConnection;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class AttendanceQueryController : Controller
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

        public IActionResult ReadAttendance()
        {
            try
            {
                MQTTConnection mqttConnection = new MQTTConnection();
                mqttConnection.PublishReadMQTT();
                return RedirectToAction("Index", "AttendanceQuery");
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }

        // Check if there are new attendance records
        // Get the data from the Orion history and insert it into the SQL attendance table
        // (get the date and time of that data and store it)
        public IActionResult Index()
        {
            try
            {
                StudentDAO studentDAO = new StudentDAO();
                SubjectDAO subjectDAO = new SubjectDAO();
                AttendanceDAO attendanceDAO = new AttendanceDAO();
                attendanceDAO.FetchMongoDBData();
                int studentCount = studentDAO.GetListing().Count;
                ViewBag.AttendanceData = new List<object>();
                foreach (var item in subjectDAO.GetListing())
                {
                    List<int> attendanceCounts = attendanceDAO.GetAttendanceAndSubjectCounts(item.ID);
                    if (attendanceCounts[0] != 0 && attendanceCounts[1] != 0)
                    {
                        double recordedPresences = Convert.ToDouble(attendanceCounts[0]);
                        double totalPresences = Convert.ToDouble(attendanceCounts[1]);
                        int percentage = Convert.ToInt32((recordedPresences / (studentCount * totalPresences)) * 100);
                        ViewBag.AttendanceData.Add(new { percentage, item.Description });
                    }
                }

                PrepareFilterData();
                return View("Index");
            }
            catch (Exception error)
            {
                return View("Error", new ErrorViewModel(error.ToString()));
            }
        }

        private void PrepareFilterData()
        {
            try
            {
                ClassDAO classDAO = new ClassDAO();
                var classes = classDAO.GetListing();

                StudentDAO studentDAO = new StudentDAO();
                var students = studentDAO.GetListing();

                List<SelectListItem> classList = new List<SelectListItem>();
                classList.Add(new SelectListItem("Select a class...", "0"));
                foreach (var classItem in classes)
                {
                    SubjectDAO subjectDAO = new SubjectDAO();
                    SelectListItem item = new SelectListItem($"{classItem.Content} - {subjectDAO.Query(classItem.SubjectCode).Description}", classItem.ID.ToString());
                    classList.Add(item);
                }

                List<SelectListItem> studentList = new List<SelectListItem>();
                studentList.Add(new SelectListItem("Select a student...", "0"));
                foreach (var student in students)
                {
                    SelectListItem item = new SelectListItem(student.Name, student.ID.ToString());
                    studentList.Add(item);
                }
                ViewBag.Students = studentList;
                ViewBag.Classes = classList;
            }
            catch (Exception err)
            {
                throw new Exception("Error listing filter data... " + err.Message);
            }
        }

        public IActionResult AdvancedAttendanceQuery(int studentId, int classId)
        {
            try
            {
                AttendanceDAO attendanceDAO = new AttendanceDAO();

                if (string.IsNullOrEmpty(studentId.ToString()))
                    studentId = 0;
                if (string.IsNullOrEmpty(classId.ToString()))
                    classId = 0;

                var list = attendanceDAO.AdvancedAttendanceQuery(studentId, classId);
                return PartialView("pvGridAttendance", list);
            }
            catch (Exception err)
            {
                return View("Error", new ErrorViewModel(err.ToString()));
            }
        }
    }
}