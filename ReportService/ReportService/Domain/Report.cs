using System.Linq;
using System.Text;
using EnsureThat;

namespace ReportService.Domain
{
    internal sealed class Report
    {
        public Report(string name)
        {
            EnsureArg.IsNotNullOrWhiteSpace(name, nameof(name));

            this.ReportContentBuilder = new StringBuilder();
            this.ReportContentBuilder.AppendLine();
            this.ReportContentBuilder.AppendLine(name);
        }

        private const string Delimiter = "---";
        private const string DepartmentTotalSalary = "Всего по отделу";
        private const string TotalSalary = "Всего по предприятию";

        private StringBuilder ReportContentBuilder { get; }
        private decimal TotalSalaryValue { get; set; }

        public void AddDepartment(Department department)
        {
            EnsureArg.IsNotNull(department, nameof(department));
            
            var maxLengthName = department.GetEmployees().Max(e => e.Name.Length);
            maxLengthName = maxLengthName > DepartmentTotalSalary.Length ? maxLengthName : DepartmentTotalSalary.Length;

            this.ReportContentBuilder.AppendLine();
            this.ReportContentBuilder.AppendLine(Delimiter);
            this.ReportContentBuilder.AppendLine(department.Name);
            foreach (var employee in department.GetEmployees())
            {
                this.ReportContentBuilder.AppendLine();
                this.ReportContentBuilder.AppendLine($"{employee.Name.PadRight(maxLengthName)}\t{employee.Salary}р");
            }
            var departmentTotalSalary = department.GetTotalSalary();
            this.ReportContentBuilder.AppendLine();
            this.ReportContentBuilder.AppendLine($"{DepartmentTotalSalary.PadRight(maxLengthName)}\t{departmentTotalSalary}р");
            this.TotalSalaryValue += departmentTotalSalary;
        }

        public string GetReportContent()
        {
            this.ReportContentBuilder.AppendLine();
            this.ReportContentBuilder.AppendLine(Delimiter);
            this.ReportContentBuilder.AppendLine();
            this.ReportContentBuilder.AppendLine($"{TotalSalary}\t{this.TotalSalaryValue}р");
            return this.ReportContentBuilder.ToString();
        }
    }
}
