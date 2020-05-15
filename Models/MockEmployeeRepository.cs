using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class MockEmployeeRepository : IEmployeeRepository
    {

        private List<Employee> _employeeList;
        public MockEmployeeRepository()
        {
            _employeeList = new List<Employee>()

            {
                new Employee() { Id = 1, Name = "George", Department=Dept.HR, Email="george@noemail.com"},
                new Employee() { Id = 2, Name = "Sam", Department=Dept.IT, Email="asdfdgeddge@noemail.com"},
                new Employee() { Id = 3, Name = "Robert", Department=Dept.Payroll, Email="gddge@noemail.com"}

            };

        }

        public Employee Add(Employee employeeAdd)
        {
            employeeAdd.Id = _employeeList.Max(e => e.Id) + 1;
            _employeeList.Add(employeeAdd);
             return employeeAdd;
        }

        public Employee Delete(int id)
        {
            Employee e = _employeeList.FirstOrDefault(e => e.Id == id);
            if (e != null) {
                _employeeList.Remove(e);
            }
            return e;
        }

        public Employee Update(Employee employeeUpdate)
        {
            Employee e = _employeeList.FirstOrDefault(e => e.Id == employeeUpdate.Id);
            if (e != null)
            {
                e.Name = employeeUpdate.Name;
                e.Email = employeeUpdate.Email;
                e.Department = employeeUpdate.Department;
            }
            return e;
        }

        IEnumerable<Employee> IEmployeeRepository.GetAllEmployees()
        {
            return _employeeList;
        }

        Employee IEmployeeRepository.GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(x => x.Id == Id);
        }
    }
}
