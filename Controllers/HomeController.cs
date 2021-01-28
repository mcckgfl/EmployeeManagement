using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection;
using EmployeeManagement.Security;

namespace EmployeeManagement.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger logger;
        private readonly IDataProtector _dataProtector;

        public HomeController(IEmployeeRepository employeeRepository,
            IWebHostEnvironment webHostEnvironment,
            ILogger<HomeController> logger,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dpps)

        {
            _employeeRepository = employeeRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
            _dataProtector = dataProtectionProvider.CreateProtector(dpps.EmployeeIdRouteValue);
        }

        [AllowAnonymous]
        public ViewResult Index() {

            var model = _employeeRepository.GetAllEmployees()
                            .Select(e => 
                            {
                                e.EncryptedId = _dataProtector.Protect(e.Id.ToString());
                                return e;
                            });

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
                string uniqueFileName = ProcessUploadedFile(employeeCreateViewModel);

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
        [AllowAnonymous]
        public ViewResult Details(string id)
        {

            //throw new Exception("error here");

            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogCritical("Critical Log");

            int decryptedId = Convert.ToInt32(_dataProtector.Unprotect(id));

            Employee e = _employeeRepository.GetEmployee(decryptedId);

            if (e == null) 
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", decryptedId);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = e,
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
           
            Employee editEmployee = _employeeRepository.GetEmployee(id);

            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel()
            {
                Id = editEmployee.Id,
                Name = editEmployee.Name,
                Email = editEmployee.Email,
                Department = editEmployee.Department,
                ExistingPhotoPath = editEmployee.PhotoPath
            };

            return View(employeeEditViewModel);
        }


        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel employeeEditViewModel)
        {
            if (ModelState.IsValid)
            {
                Employee e = _employeeRepository.GetEmployee(employeeEditViewModel.Id);
                e.Name = employeeEditViewModel.Name;
                e.Email = employeeEditViewModel.Email;
                e.Department = employeeEditViewModel.Department;

                if (employeeEditViewModel.Photo != null)
                {
                    if (employeeEditViewModel.ExistingPhotoPath != null) {
                        string fileToDelete = Path.Combine(webHostEnvironment.WebRootPath, "images", employeeEditViewModel.ExistingPhotoPath);
                        System.IO.File.Delete(fileToDelete);
                    }
                   e.PhotoPath = ProcessUploadedFile(employeeEditViewModel);
                }

                _employeeRepository.Update(e);
                return RedirectToAction("index");

            };

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = "";

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Photo.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
