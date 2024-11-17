using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using ReportService.Application.Clients;
using ReportService.Application.Commands;
using ReportService.Application.CommandHandlers;
using ReportService.Application.Repositories;
using ReportService.Models.PocoModels;
using Xunit;

namespace TestProject1;

public sealed class CreateReportServiceTests
{
    [Fact]
    public async Task Should_Create_Report()
    {
        // Arrange
        var employeeRepositoryMoq = new Mock<IEmployeeRepository>();
        employeeRepositoryMoq
            .Setup(moq => moq.ListActiveDepartmentsAsync())
            .ReturnsAsync(Fakers.GetDepartments());
        employeeRepositoryMoq
            .Setup(moq => moq.ListEmployeesByDepartmentIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Fakers.GetEmployees());
        
        var buhClientMoq = new Mock<IBuhClient>();
        buhClientMoq
            .Setup(moq => moq.GetEmployeeCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(Fakers.GetBuhCode());
        
        var salaryClientMoq = new Mock<ISalaryClient>();
        salaryClientMoq
            .Setup(moq => moq.GetEmployeeSalaryAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(Fakers.GetSalary());
        
        var command = new CreateReport(11, 2024);
        
        // Act & Assert
        _ = await InvokeCommandAsync(
            command,
            employeeRepositoryMoq.Object,
            salaryClientMoq.Object,
            buhClientMoq.Object);
    }

    [Fact]
    private async Task Should_Create_Exact_Report()
    {
        // Arrange
        var departments = new[]
        {
            new DepartmentPocoModel { Id = Guid.NewGuid(), Name = "Бэк" },
            new DepartmentPocoModel { Id = Guid.NewGuid(), Name = "Фронт" }
        };
        var backEmployees = new[]
        {
            new EmployeePocoModel { Inn = "1", Name = "Иванов" },
            new EmployeePocoModel { Inn = "2", Name = "Петров" }
        };
        var frontEmployees = new[]
        {
            new EmployeePocoModel { Inn = "3", Name = "Сидоров" },
            new EmployeePocoModel { Inn = "4", Name = "Кузнецов" }
        };
        
        var employeeRepositoryMoq = new Mock<IEmployeeRepository>();
        employeeRepositoryMoq
            .Setup(moq => moq.ListActiveDepartmentsAsync())
            .ReturnsAsync(departments);
        employeeRepositoryMoq
            .Setup(moq => moq.ListEmployeesByDepartmentIdAsync(It.Is<Guid>(id => id == departments[0].Id)))
            .ReturnsAsync(backEmployees);
        employeeRepositoryMoq
            .Setup(moq => moq.ListEmployeesByDepartmentIdAsync(It.Is<Guid>(id => id == departments[1].Id)))
            .ReturnsAsync(frontEmployees);
        
        var buhClientMoq = new Mock<IBuhClient>();
        buhClientMoq
            .Setup(moq => moq.GetEmployeeCodeAsync(It.Is<string>(inn => inn == backEmployees[0].Inn)))
            .ReturnsAsync("1111");
        buhClientMoq
            .Setup(moq => moq.GetEmployeeCodeAsync(It.Is<string>(inn => inn == backEmployees[1].Inn)))
            .ReturnsAsync("2222");
        buhClientMoq
            .Setup(moq => moq.GetEmployeeCodeAsync(It.Is<string>(inn => inn == frontEmployees[0].Inn)))
            .ReturnsAsync("3333");
        buhClientMoq
            .Setup(moq => moq.GetEmployeeCodeAsync(It.Is<string>(inn => inn == frontEmployees[1].Inn)))
            .ReturnsAsync("4444");
        
        var salaryClientMoq = new Mock<ISalaryClient>();
        salaryClientMoq
            .Setup(moq => moq.GetEmployeeSalaryAsync(It.Is<string>(inn => inn == backEmployees[0].Inn), It.Is<string>(code => code == "1111")))
            .ReturnsAsync(100_000);
        salaryClientMoq
            .Setup(moq => moq.GetEmployeeSalaryAsync(It.Is<string>(inn => inn == backEmployees[1].Inn), It.Is<string>(code => code == "2222")))
            .ReturnsAsync(200_000);
        salaryClientMoq
            .Setup(moq => moq.GetEmployeeSalaryAsync(It.Is<string>(inn => inn == frontEmployees[0].Inn), It.Is<string>(code => code == "3333")))
            .ReturnsAsync(300_000);
        salaryClientMoq
            .Setup(moq => moq.GetEmployeeSalaryAsync(It.Is<string>(inn => inn == frontEmployees[1].Inn), It.Is<string>(code => code == "4444")))
            .ReturnsAsync(400_000);
        
        var command = new CreateReport(11, 2024);
        
        // Act
        var reportStream = await InvokeCommandAsync(
            command,
            employeeRepositoryMoq.Object,
            salaryClientMoq.Object,
            buhClientMoq.Object);
        
        // Assert
        using var reader = new StreamReader(reportStream);
        var reportContent = await reader.ReadToEndAsync();
        
        Assert.Equal(reportContent, ExpectedReportContent);   
    }

    private const string ExpectedReportContent = @"
Ноябрь 2024

---
Бэк

Иванов         	100000р

Петров         	200000р

Всего по отделу	300000р

---
Фронт

Сидоров        	300000р

Кузнецов       	400000р

Всего по отделу	700000р

---

Всего по предприятию	1000000р
";
    
    private static Task<Stream> InvokeCommandAsync(
        CreateReport command,
        IEmployeeRepository employeeRepositoryMoq,
        ISalaryClient salaryClientMoq,
        IBuhClient buhClientMoq)
    {
        var loggerMoq = new Mock<ILogger<CreateReportCommandHandler>>();
        var handler = new CreateReportCommandHandler(employeeRepositoryMoq, buhClientMoq, salaryClientMoq, loggerMoq.Object);
        return handler.Handle(command, CancellationToken.None);
    }
}
