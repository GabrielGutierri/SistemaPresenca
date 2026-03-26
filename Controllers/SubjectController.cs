using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class SubjectController : BaseController<SubjectViewModel>
    {
        public SubjectController()
        {
            DAO = new SubjectDAO();
            GeneratesNextId = true;
        }

        protected override void ValidateData(SubjectViewModel subject, string operation)
        {
            base.ValidateData(subject, operation);
            if (string.IsNullOrEmpty(subject.Description))
                ModelState.AddModelError("Description", "Required field.");
            if (subject.CourseLoad <= 0)
                ModelState.AddModelError("CourseLoad", "Required field.");
            if (subject.TeacherCode <= 0)
                ModelState.AddModelError("TeacherCode", "Please select a teacher.");
        }

        protected override void PopulateViewData(string operation, SubjectViewModel model)
        {
            base.PopulateViewData(operation, model);
            PrepareTeacherDropdown();
        }

        private void PrepareTeacherDropdown()
        {
            TeacherDAO teacherDAO = new TeacherDAO();
            var teachers = teacherDAO.GetListing();
            List<SelectListItem> teacherList = new List<SelectListItem>();

            teacherList.Add(new SelectListItem("Select a Teacher...", "0"));
            foreach (var teacher in teachers)
            {
                SelectListItem item = new SelectListItem(teacher.Name, teacher.ID.ToString());
                teacherList.Add(item);
            }
            ViewBag.Teachers = teacherList;
        }

        public override IActionResult Delete(int id)
        {
            try
            {
                ClassDAO classDAO = new ClassDAO();
                foreach (var item in classDAO.GetListing())
                    if (item.SubjectCode == id)
                    {
                        TempData["AlertMessage"] = "Cannot delete. Subject has open classes.";
                        return RedirectToAction("Index", "ListingQuery");
                    }
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