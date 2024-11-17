using EnsureThat;

namespace ReportService.Domain
{
    internal sealed class Employee
    {
        public Employee(string name, decimal salary)
        {
            EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));
            EnsureArg.IsGte(salary, 0, nameof(salary));
            
            this.Name = name;
            this.Salary = salary;
        }
        
        public string Name { get; }
        public decimal Salary { get; }
    }
}
