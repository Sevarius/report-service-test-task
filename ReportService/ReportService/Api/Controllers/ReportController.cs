using System.Threading.Tasks;
using EnsureThat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReportService.Application.Commands;

namespace ReportService.Api.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : Controller
    {
        public ReportController(IMediator mediator)
        {
            EnsureArg.IsNotNull(mediator, nameof(mediator));
            
            this._mediator = mediator;
        }
        
        private readonly IMediator _mediator;
        
        [HttpGet]
        [Route("{year}/{month}")]
        public async Task<IActionResult> DownloadReport(int year, int month)
        {
            var command = new CreateReport(month, year);
            
            var reportStream = await this._mediator.Send(command);
            
            return this.File(reportStream, "application/octet-stream", "report.txt");
        }
    }
}
