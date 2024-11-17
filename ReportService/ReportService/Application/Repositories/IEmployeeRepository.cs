using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReportService.Models.PocoModels;

namespace ReportService.Application.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IReadOnlyCollection<DepartmentPocoModel>> ListActiveDepartmentsAsync();
        Task<IReadOnlyCollection<EmployeePocoModel>> ListEmployeesByDepartmentIdAsync(Guid departmentId);
    }
}
