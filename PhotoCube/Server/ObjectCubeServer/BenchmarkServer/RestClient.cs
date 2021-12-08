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
            
            var id = r.Next(1, maxNodeId);
            var id2 = r.Next(1, maxNodeId);

            await _client.GetStringAsync($"https://localhost:5001/api/cell/?&filters=[{{\"type\":\"node\",\"ids\":[{id}]}},{{\"type\":\"node\",\"ids\":[{id2}]}}]");
        }
        
        public async Task GetCubeObjects()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            await _client.GetStringAsync($"https://localhost:5001/api/CubeObject");
        }
        
        public async Task GetCubeObjectById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var id = r.Next(1,maxCubeObjectId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/CubeObject/{id}");
        }
        
        public async Task GetCubeObjectTags()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var id = r.Next(1,maxCubeObjectId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/CubeObject/{id}/tags");
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
        
        //TODO /api/Node/{tagname}
        
        public async Task GetNodeParentById()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var nodeId = r.Next(1, maxNodeId);
            
            await _client.GetStringAsync($"https://localhost:5001/api/Node/{nodeId}/parent");
        }
        
        public async Task GetNodeChildrenById()
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
        
        // TODO /api/Tag/{name}
        
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
        
        //TODO /api/Tagset/{name}
        
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
        
        public async Task GetTimeline() {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var id = r.Next(1, maxNodeId);
            var id2 = r.Next(1, maxNodeId);

            await _client.GetStringAsync($"https://localhost:5001/api/timeline/?&filters=[{{\"type\":\"node\",\"ids\":[{id}]}},{{\"type\":\"node\",\"ids\":[{id2}]}}]");
        }
    }
}