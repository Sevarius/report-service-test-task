using System;
using System.Collections.Generic;
using ReportService.Models.PocoModels;

namespace TestProject1;

internal static class Fakers
{
    private static readonly Random Random = new Random();
    private static readonly IList<int> UsedInns = new List<int>();
    private static string GetRandomInn()
    {
        int inn;
        do
        {
            inn = Random.Next(100000000, 999999999);
        } while (UsedInns.Contains(inn));
        UsedInns.Add(inn);
        return inn.ToString();
    }

    public static DepartmentPocoModel[] GetDepartments()
    {
        var departmentsQuantity = Random.Next(1, 10);
        var departments = new DepartmentPocoModel[departmentsQuantity];
        for (var i = 0; i < departmentsQuantity; i++)
        {
            departments[i] = GetDepartmentPocoModel();
        }
        return departments;
    }
    
    public static EmployeePocoModel[] GetEmployees()
    {
        var employeesQuantity = Random.Next(1, 10);
        var employees = new EmployeePocoModel[employeesQuantity];
        for (var i = 0; i < employeesQuantity; i++)
        {
            employees[i] = GetEmployeePocoModel();
        }
        return employees;
    }
    
    public static string GetBuhCode()
    {
        return Random.Next(1000, 9999).ToString();
    }

    public static decimal GetSalary()
    {
        return Random.Next(100_000, 800_000);
    }
    
    private static DepartmentPocoModel GetDepartmentPocoModel()
    {
        var id = Guid.NewGuid();
        return new DepartmentPocoModel
        {
            Id = id,
            Name = $"Name_{id}"
        };
    }
    
    private static EmployeePocoModel GetEmployeePocoModel()
    {
        return new EmployeePocoModel
        {
            Name = $"Name_{Guid.NewGuid()}",
            Inn = GetRandomInn()
        };
    }
}
