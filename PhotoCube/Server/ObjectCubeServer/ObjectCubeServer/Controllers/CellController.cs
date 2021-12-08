﻿#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ObjectCubeServer.Models.Contexts;
using ObjectCubeServer.Models.DomainClasses;
using ObjectCubeServer.Models.PublicClasses;
using ObjectCubeServer.Services;

namespace ObjectCubeServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CellController : ControllerBase
    {
        private readonly QueryGenerationService queryGenerationService = new();
        private readonly ObjectContext coContext;

        public CellController(ObjectContext coContext)
        {
            this.coContext = coContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicCubeObject>>> Get(string filters)
        {
            bool filtersDefined = filters != null;
            //Parsing:
            List<ParsedFilter>? filtersList =
                filtersDefined ? JsonConvert.DeserializeObject<List<ParsedFilter>>(filters) : null;
            
            List<PublicCubeObject> cubeobjects = 
                await coContext.PublicCubeObjects.FromSqlRaw(queryGenerationService.generateSQLQueryForObjects(filtersList)).ToListAsync();
            return Ok(cubeobjects);
        }
        
    
    }
}
