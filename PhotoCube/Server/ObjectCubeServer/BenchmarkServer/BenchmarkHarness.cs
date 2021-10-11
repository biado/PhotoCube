using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace BenchmarkServer
{
    [HtmlExporter]
    public class BenchmarkHarness
    {
        [Params(10)] 
        public int iterationCount;

        private readonly RestClient restClient = new RestClient();
        
        [Benchmark]
        public async Task GetCellPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCell();
            }
        }
        
        [Benchmark]
        public async Task GetCubeObjectPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObject();
            }
        }
        
        [Benchmark]
        public async Task GetCubeObjectByIdtPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObjectById();
            }
        }
        
        [Benchmark]
        public async Task GetCubeObjectFromTagIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObjectFromTagId();
            }
        }
        
        [Benchmark]
        public async Task GetCubeObjectFrom2TagIdsPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObjectFrom2TagIds();
            }
        }
        
        [Benchmark]
        public async Task GetCubeObjectFrom3TagIdsPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObjectFrom3TagIds();
            }
        }
        
        [Benchmark]
        public async Task GetCubeObjectFromTagIdWithOTRPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObjectFromTagIdWithOTR();
            }
        }
        
        [Benchmark]
        public async Task GetHierarchyPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetHierarchy();
            }
        }
        
        [Benchmark]
        public async Task GetHierarchyByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetHierarchyById();
            }
        }
        
        [Benchmark]
        public async Task GetNodesPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodes();
            }
        }

        [Benchmark]
        public async Task GetNodeByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodeByID();
            }
        }
        
        [Benchmark]
        public async Task GetNodeParentByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodeParentById();
            }
        }
        
        [Benchmark]
        public async Task GetNodechildrenByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodechildrenById();
            }
        }
        
        [Benchmark]
        public async Task GetPhotoByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetPhotoById();
            }
        }
        
        [Benchmark]
        public async Task GetTagsPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetTags();
            }
        }
        
        [Benchmark]
        public async Task GetTagByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetTagById();
            }
        }
        
        [Benchmark]
        public async Task GetTagsetPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetTagset();
            }
        }
        
        [Benchmark]
        public async Task GetTagsetByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetTagsetById();
            }
        }
        
        [Benchmark]
        public async Task GetThumbnailPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetThumbnail();
            }
        }
        
        [Benchmark]
        public async Task GetThumbnailByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetThumbnailById();
            }
        }
    }
}