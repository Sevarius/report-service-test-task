using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReportService.Application.Clients;
using ReportService.Application.Repositories;
using ReportService.Infrastructure.Clients;
using ReportService.Infrastructure.Options;
using ReportService.Infrastructure.Repositories;

namespace ReportService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddLogging(builder => builder.AddConsole());

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddHttpClient();
            
            services.AddScoped<IEmployeeRepository>(_ => new EmployeeRepository(Configuration.GetConnectionString("DataAccess")));
            services.AddSingleton(Configuration.GetSection("Clients:BuhService").Get<BuhClientOptions>());
            services.AddSingleton(Configuration.GetSection("Clients:SalaryService").Get<SalaryClientOptions>());
            services.AddScoped<ISalaryClient, SalaryClient>();
            services.AddScoped<IBuhClient, BuhClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
