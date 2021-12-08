using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace BenchmarkServer
{
    [HtmlExporter]
    public class BenchmarkHarness
    {
        [Params(100)] 
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
        
        //[Benchmark]
        public async Task GetCubeObjectPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObjects();
            }
        }
        
        //[Benchmark]
        public async Task GetCubeObjectByIdtPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObjectById();
            }
        }
        
        //[Benchmark]
        public async Task GetCubeObjectFromTagIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetCubeObjectTags();
            }
        }
        
        //[Benchmark]
        public async Task GetHierarchyPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetHierarchy();
            }
        }
        
        //[Benchmark]
        public async Task GetHierarchyByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetHierarchyById();
            }
        }
        
        //[Benchmark]
        public async Task GetNodesPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodes();
            }
        }

        //[Benchmark]
        public async Task GetNodeByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodeByID();
            }
        }
        
        //[Benchmark]
        public async Task GetNodeParentByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodeParentById();
            }
        }
        
        //[Benchmark]
        public async Task GetNodechildrenByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetNodeChildrenById();
            }
        }
        
        //[Benchmark]
        public async Task GetPhotoByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetPhotoById();
            }
        }
        
        //[Benchmark]
        public async Task GetTagsPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetTags();
            }
        }
        
        //[Benchmark]
        public async Task GetTagByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetTagById();
            }
        }
        
        //[Benchmark]
        public async Task GetTagsetPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetTagset();
            }
        }
        
        //[Benchmark]
        public async Task GetTagsetByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetTagsetById();
            }
        }
        
        //[Benchmark]
        public async Task GetThumbnailPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetThumbnail();
            }
        }
        
        //[Benchmark]
        public async Task GetThumbnailByIdPayloadAsync()
        {
            for(int i = 0; i < iterationCount; i++)
            {
                await restClient.GetThumbnailById();
            }
        }
    }
}