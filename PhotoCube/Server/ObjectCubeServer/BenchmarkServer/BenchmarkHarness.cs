using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace BenchmarkServer
{
    [HtmlExporter]
    public class BenchmarkHarness
    {
        [Params(100)] 
        private int iterationCount;

        private readonly RestClient restClient = new RestClient();

        [Benchmark]
        public async Task GetNodeByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodeByID();
            }
        }
    }
}