using System.IO;
using EnsureThat;
using MediatR;

namespace ReportService.Application.Commands
{
    public sealed class CreateReport : IRequest<Stream>
    {
        public CreateReport(int month, int year)
        {
            EnsureArg.IsGt(month, 0, nameof(month));
            EnsureArg.IsLte(month, 12, nameof(month));
            EnsureArg.IsGt(year, 0, nameof(year));
            
            this.Month = month;
            this.Year = year;
        }
        
        public int Month { get; }
        public int Year { get; }
    }
}
