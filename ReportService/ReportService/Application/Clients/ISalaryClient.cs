using System.Threading.Tasks;

namespace ReportService.Application.Clients
{
    public interface ISalaryClient
    {
        Task<decimal> GetEmployeeSalaryAsync(string employeeInn, string buhCode);
    }
}
