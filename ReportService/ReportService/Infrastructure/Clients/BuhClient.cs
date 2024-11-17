using System;
using System.Net.Http;
using System.Threading.Tasks;
using EnsureThat;
using ReportService.Application.Clients;
using ReportService.Infrastructure.Options;

namespace ReportService.Infrastructure.Clients
{
    internal sealed class BuhClient : IBuhClient
    {
        public BuhClient(IHttpClientFactory httpClientFactory, BuhClientOptions options)
        {
            EnsureArg.IsNotNull(httpClientFactory, nameof(httpClientFactory));
            EnsureArg.IsNotNull(options, nameof(options));
            EnsureArg.IsNotNull(options.BaseUrl, nameof(options.BaseUrl));

            this._httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = options.BaseUrl;
        }

        private readonly HttpClient _httpClient;

        public async Task<string> GetEmployeeCodeAsync(string employeeInn)
        {
            EnsureArg.IsNotNullOrWhiteSpace(employeeInn, nameof(employeeInn));

            using (var response = await this._httpClient.GetAsync($"/inn/{employeeInn}"))
            {
                response.EnsureSuccessStatusCode();

                var code = await response.Content.ReadAsStringAsync();
                
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new InvalidOperationException("Failed to get employee code");
                }
                
                return code;
            }
        }
    }
}
