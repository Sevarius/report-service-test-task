using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.Extensions.Logging;
using ReportService.Application.Clients;
using ReportService.Application.Commands;
using ReportService.Application.Repositories;
using ReportService.Domain;

namespace ReportService.Application.CommandHandlers
{
    internal sealed class CreateReportCommandHandler : IRequestHandler<CreateReport, Stream>
    {
        public CreateReportCommandHandler(
            IEmployeeRepository employeeRepository,
            IBuhClient buhClient,
            ISalaryClient salaryClient,
            ILogger<CreateReportCommandHandler> logger)
        {
            EnsureArg.IsNotNull(employeeRepository, nameof(employeeRepository));
            EnsureArg.IsNotNull(buhClient, nameof(buhClient));
            EnsureArg.IsNotNull(salaryClient, nameof(salaryClient));
            EnsureArg.IsNotNull(logger, nameof(logger));
            
            this._employeeRepository = employeeRepository;
            this._buhClient = buhClient;
            this._salaryClient = salaryClient;
            this._logger = logger;
        }

        private static readonly CultureInfo Ru = CultureInfo.GetCultureInfoByIetfLanguageTag("ru");
        
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBuhClient _buhClient;
        private readonly ISalaryClient _salaryClient;
        private readonly ILogger<CreateReportCommandHandler> _logger;

        public async Task<Stream> Handle(CreateReport command, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(command, nameof(command));
            
            var report = new Report(GetReportName(command.Month, command.Year));
            
            var departments = await this._employeeRepository.ListActiveDepartmentsAsync()
                .ConfigureAwait(false);
            
            _logger.LogInformation("Found {DepartmentsCount} departments", departments.Count);

            foreach (var departmentModel in departments)
            {
                var departmentEmployees = await this._employeeRepository
                    .ListEmployeesByDepartmentIdAsync(departmentModel.Id)
                    .ConfigureAwait(false);
                
                _logger.LogInformation(
                    "Found {EmployeesCount} employees in {DepartmentName} department",
                    departmentEmployees.Count,
                    departmentModel.Name);
                
                var department = new Department(departmentModel.Name);

                foreach (var employee in departmentEmployees)
                {
                    var buhCode = await this._buhClient.GetEmployeeCodeAsync(employee.Inn)
                        .ConfigureAwait(false);
                    var salary = await this._salaryClient.GetEmployeeSalaryAsync(employee.Inn, buhCode)
                        .ConfigureAwait(false);
                    
                    department.AddEmployee(new Employee(employee.Name, salary));
                }
                
                report.AddDepartment(department);
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(report.GetReportContent()));
        }

        private static string GetReportName(int month, int year)
        {
            var monthName = new DateTime(year, month, 1).ToString("MMMM", Ru);
            return $"{char.ToUpper(monthName[0], Ru)}{monthName.Substring(1)} {year}";
        }
    }
}
