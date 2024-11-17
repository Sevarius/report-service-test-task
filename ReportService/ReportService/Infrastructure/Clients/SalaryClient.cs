using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EnsureThat;
using Newtonsoft.Json;
using ReportService.Application.Clients;
using ReportService.Infrastructure.Options;

namespace ReportService.Infrastructure.Clients
{
    internal sealed class SalaryClient : ISalaryClient
    {
        public SalaryClient(IHttpClientFactory httpClientFactory, SalaryClientOptions options)
        {
            EnsureArg.IsNotNull(httpClientFactory, nameof(httpClientFactory));
            EnsureArg.IsNotNull(options, nameof(options));
            EnsureArg.IsNotNull(options.BaseUrl, nameof(options.BaseUrl));

            this._httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = options.BaseUrl;
        }
        
        private readonly HttpClient _httpClient;
        
        public async Task<decimal> GetEmployeeSalaryAsync(string employeeInn, string buhCode)
        {
            EnsureArg.IsNotNullOrWhiteSpace(employeeInn, nameof(employeeInn));
            EnsureArg.IsNotNullOrWhiteSpace(buhCode, nameof(buhCode));

            using (var message = new HttpRequestMessage(HttpMethod.Post, $"/empcode/{employeeInn}"))
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(buhCode), Encoding.UTF8, "application/json");
                
                using (var response = await this._httpClient.SendAsync(message))
                {
                    response.EnsureSuccessStatusCode();
                    return decimal.TryParse(await response.Content.ReadAsStringAsync(), out var result)
                        ? result
                        : throw new InvalidOperationException("Failed to parse salary");
                }
            }
        }
    }
}
