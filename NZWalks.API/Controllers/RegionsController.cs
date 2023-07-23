﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    
    //https://localhost:1234/api/Regions
    [Route("api/[controller]")]
    [ApiController]
  
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, 
            IMapper mapper, ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }
        //Get All Regions
        //GET: https://localhost:portnumber/api/regions

        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll() 
        {
            try
            {
                
                //Get Data from Database - Domain models
                var regionsDomain = await regionRepository.GetAllAsync();
                //Return DTOs
                logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regionsDomain)}");

                return Ok(mapper.Map<List<RegionDto>>(regionsDomain));

            }
            catch(Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
            
            
          

            /*   //map Domain Models to DTOs
               var regionsDto = new List<RegionDto>();
               foreach(var regionDomain in regionsDomain) 
               {
                   regionsDto.Add(new RegionDto()
                   {
                       Id = regionDomain.Id,
                       Code = regionDomain.Code,
                       Name = regionDomain.Name,
                       RegionImageUrl = regionDomain.RegionImageUrl
                   });
               }*/




          
        }

        //Get Single Region (Get Region By ID)
        //GEt: https://localhost:portnumber/api/regions/{id}

        [HttpGet]
        [Route("{id:Guid}")]
       // [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute]Guid id) 
        {
            //var region = dbContext.Regions.Find(id);
            //Get Region Domain Model from Database

            var regionDomain = await regionRepository.GetByIdAsync(id);

            if(regionDomain == null) 
            {
                return NotFound();  
            }

            //Map/Convert Region Domain Model to Region DTO
            

            //Return DTO back to client

            return Ok(mapper.Map<RegionDto>(regionDomain));
        }

        //POST To Create New Region
        //POST: https://localhost:portnumber/api/regions

        [HttpPost]
        [ValidateModel]
       // [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto) 
        {
           
                //Map or Convert DTO to Domain Model
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);


                //Use Domain MOdel to create Region
                regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);


                //Map Domain Model back to DTO
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);

           

            
        }


        //Update Region
        //PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {


                //Map DTO to Domain Model
                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                //Check if region exists
                regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();

                }



                //Convert Domain Model to DTO

                return Ok(mapper.Map<RegionDto>(regionDomainModel));

           
          


            
        }


        //Delete Region
        //DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
      //  [Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);

            if(regionDomainModel == null)
            {
                return NotFound();
            }

            
         ;

            //return deleted Region back
            //map Domain Model to DTO

           

            return Ok(mapper.Map<RegionDto>(regionDomainModel));


        }
    }
}
