using System.Threading.Tasks;

namespace ReportService.Application.Clients
{
    public interface IBuhClient
    {
        Task<string> GetEmployeeCodeAsync(string employeeInn);
    }
}
