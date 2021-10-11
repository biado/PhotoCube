using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace BenchmarkServer
{
    public class RestClient
    {
        private static readonly HttpClient _client = new HttpClient();
        //currently hardcoded in calls to use random distribution within id bounds
        private Random r = new Random(42);
        // currently hardcoded to PSQL.sql
        private const int maxNodeId = 8842;
        private const int maxTagId = 10689;
        private const int maxTagsetId = 20;
        private const int maxCubeObjectId = 191524;
        private const int maxHierarchyId = 3;

        public async Task GetCell()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var yAxisType = "node";
            var yAxisId = r.Next(1, maxNodeId);
            var zAxisType = "tagset";
            var zAxisId = r.Next(1, maxTagsetId);

            await _client.GetStringAsync($"https://localhost:5001/api/Cell?yAxis.Type={yAxisType}&yAxis.Id={yAxisId}&zAxis.Type={zAxisType}&zAxis.Id={zAxisId}");
        }
        
        public async Task GetCubeObject()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            await _client.GetStringAsync($"https://localhost:5001//api/CubeObject");
        }
        
        public async Task GetCubeObjectById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var id = r.Next(1,maxCubeObjectId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/CubeObject/{id}");
        }
        
        public async Task GetCubeObjectFromTagId()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tagId = r.Next(1, maxTagId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/CubeObject/FromTagId/{tagId}");
        }
        
        public async Task GetCubeObjectFrom2TagIds()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tagId = r.Next(1, maxTagId);
            var tagId2 = r.Next(1, maxTagId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/CubeObject/From2TagIds/{tagId}/{tagId2}");
        }
        
        public async Task GetCubeObjectFrom3TagIds()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tagId = r.Next(1, maxTagId);
            var tagId2 = r.Next(1, maxTagId);
            var tagId3 = r.Next(1, maxTagId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/CubeObject/From2TagIds/{tagId}/{tagId2}/{tagId3}");
        }
        
        public async Task GetCubeObjectFromTagIdWithOTR()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tagId = r.Next(1, maxTagId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/CubeObject/FromTagIdWithOTR/{tagId}");
        }
        
        public async Task GetHierarchy()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await _client.GetStringAsync($"https://localhost:5001/api/Hierarchy");
        }
        
        public async Task GetHierarchyById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var id = r.Next(1, maxHierarchyId);

            await _client.GetStringAsync($"https://localhost:5001/api/Hierarchy/{id}");
        }
        
        public async Task GetNodes()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await _client.GetStringAsync($"https://localhost:5001/api/Node");
        }
        
        public async Task GetNodeByID()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var id = r.Next(1, maxNodeId);
            await _client.GetStringAsync($"https://localhost:5001/api/Node/{id}");
        }
        
        public async Task GetNodeParentById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var nodeId = r.Next(1, maxNodeId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/Node/{nodeId}/parent");
        }
        
        public async Task GetNodechildrenById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var nodeId = r.Next(1, maxNodeId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/Node/{nodeId}/children");
        }
        
        public async Task GetPhotoById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var id = r.Next(1, maxCubeObjectId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/Photo/{id}");
        }
        
        public async Task GetTags()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await _client.GetStringAsync($"https://localhost:5001/api/Tag");
        }
        
        public async Task GetTagById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var id = r.Next(1, maxTagId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/Tag/{id}");
        }
        
        public async Task GetTagset()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await _client.GetStringAsync($"https://localhost:5001/api/Tagset");
        }
        
        public async Task GetTagsetById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var id = r.Next(1, maxTagsetId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/Tagset/{id}");
        }
        
        public async Task GetThumbnail()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            await _client.GetStringAsync($"https://localhost:5001/api/Thumbnail");
        }
        
        public async Task GetThumbnailById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var id = r.Next(1, maxCubeObjectId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/Thumbnail/{id}");
        }
    }
}