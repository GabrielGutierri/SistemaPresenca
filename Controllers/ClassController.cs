using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TrabalhoInterdisciplinar.DAO;
using TrabalhoInterdisciplinar.Models;

namespace TrabalhoInterdisciplinar.Controllers
{
    public class ClassController : BaseController<ClassViewModel>
    {
        public ClassController()
        {
            DAO = new ClassDAO();
            GeneratesNextId = true;
        }

        protected override void ValidateData(ClassViewModel classItem, string operation)
        {
            base.ValidateData(classItem, operation);
            if (string.IsNullOrEmpty(classItem.Content))
                ModelState.AddModelError("Content", "Required field.");
            if (classItem.ClassDateTime > DateTime.MaxValue)
                ModelState.AddModelError("ClassDateTime", "Invalid date.");
            if (classItem.SubjectCode <= 0)
                ModelState.AddModelError("SubjectCode", "Please select a subject.");
        }

        protected override void PopulateViewData(string operation, ClassViewModel model)
        {
            base.PopulateViewData(operation, model);
            model.ClassDateTime = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            PrepareSubjectDropdown();
        }

        private void PrepareSubjectDropdown()
        {
            SubjectDAO subjectDAO = new SubjectDAO();
            var subjects = subjectDAO.GetListing();
            List<SelectListItem> subjectList = new List<SelectListItem>();

            subjectList.Add(new SelectListItem("Select a Subject...", "0"));
            foreach (var subject in subjects)
            {
                SelectListItem item = new SelectListItem(subject.Description, subject.ID.ToString());
                subjectList.Add(item);
            }
            ViewBag.Subjects = subjectList;
        }
    }
}