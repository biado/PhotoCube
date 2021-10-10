using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using ObjectCubeServer.Models.DomainClasses;


namespace BenchmarkServer
{
    public class RestClient
    {
        private static readonly HttpClient _client = new HttpClient();
        public async Task GetNodeByID()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var id = "5";
            await _client.GetStringAsync($"https://localhost:5001/api/node/{id}");
        }
    }
}