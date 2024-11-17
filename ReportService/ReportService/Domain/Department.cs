using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace ReportService.Domain
{
    internal sealed class Department
    {
        public Department(string name)
        {
            EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

            this.Name = name;
            this.Employees = new List<Employee>();
        }

        public string Name { get; }
        private IList<Employee> Employees { get; }

        public void AddEmployee(Employee employee)
        {
            EnsureArg.IsNotNull(employee, nameof(employee));

            this.Employees.Add(employee);
        }

        public IReadOnlyCollection<Employee> GetEmployees()
        {
            return this.Employees.ToArray();
        }

        public decimal GetTotalSalary()
        {
            return this.Employees.Sum(employee => employee.Salary);
        }
    }
}
