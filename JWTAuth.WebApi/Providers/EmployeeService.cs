using JWTAuth.WebApi.Interfaces;
using JWTAuth.WebApi.Models;
using JWTAuth.WebApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace JWTAuth.WebApi.Providers
{
    public class EmployeeService : IEmployeeService
    {
        private readonly DatabaseContext context;

        public EmployeeService(DatabaseContext context)
        {
            this.context = context;
        }

        public List<Employee> GetEmployeeDetails()
        {
            try
            {
                return context.Employees.ToList();
            }
            catch
            {
                throw;
            }
        }

        public Employee GetEmployeeDetails(int id)
        {
            try
            {
                Employee? employee = context.Employees.Find(id);
                if (employee != null)
                {
                    return employee;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public void AddEmployee(Employee employee)
        {
            try
            {
                if (string.IsNullOrEmpty(employee.Password))
                    throw new Exception("Password cannot be empty");

                string encryptedPass = Crypto.encryptPassword(employee.Password);
                employee.Password = encryptedPass;
                context.Employees.Add(employee);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            try
            {
                context.Entry(employee).State = EntityState.Modified;
                context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public Employee DeleteEmployee(int id)
        {
            try
            {
                Employee? employee = context.Employees.Find(id);

                if (employee != null)
                {
                    context.Employees.Remove(employee);
                    context.SaveChanges();
                    return employee;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool CheckEmployee(int id)
        {
            return context.Employees.Any(e => e.EmployeeID == id);
        }
    }
}
