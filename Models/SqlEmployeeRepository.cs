using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class SqlEmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext context;

        public SqlEmployeeRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Employee Add(Employee employeeAdd)
        {
            context.Employees.Add(employeeAdd);
            context.SaveChanges();
            return employeeAdd;
        }

        public Employee Delete(int id)
        {
            Employee e = context.Employees.Find(id);
            context.Employees.Remove(e);
            context.SaveChanges();
            return e;
        }


        public IEnumerable<Employee> GetAllEmployees()
        {
            return context.Employees;
        }

        public Employee GetEmployee(int id)
        {
            return context.Employees.Find(id);
        }

        public Employee Update(Employee employeeUpdate)
        {
            var e = context.Employees.Attach(employeeUpdate);
            e.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return employeeUpdate;
        }
    }
}
