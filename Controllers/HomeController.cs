using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
            IWebHostEnvironment webHostEnvironment)
        {
            _employeeRepository = employeeRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public ViewResult Index() {

            var model = _employeeRepository.GetAllEmployees();
            return View(model);
                
        }
        public JsonResult DetailsAsJson()
        {
            Employee model = _employeeRepository.GetEmployee(1);
            return Json(model);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel employeeCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = "";
                if (employeeCreateViewModel.Photo != null) 
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + employeeCreateViewModel.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder + uniqueFileName);
                    employeeCreateViewModel.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Employee newEmployee = new Employee
                {
                    Name = employeeCreateViewModel.Name,
                    Email = employeeCreateViewModel.Email,
                    Department = employeeCreateViewModel.Department,
                    PhotoPath = uniqueFileName
                };
                
                _employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });

            };
            
            return View();
        }

        public ViewResult Details(int id)
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = _employeeRepository.GetEmployee(id),
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }

    }
}
