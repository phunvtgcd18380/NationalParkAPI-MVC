using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/Trails")]
    [ApiController]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepository;
        private readonly IMapper _mapper;
        public TrailsController(IMapper mapper, ITrailRepository trailRepository)
        {
            _mapper = mapper;
            _trailRepository = trailRepository;
        }

        [HttpGet]
        public IActionResult GetTrails()
        {
            var objFromDb = _trailRepository.GetTrails();
            var objDto = new List<TrailDto>();
            foreach(var obj in objFromDb)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }
            
            return Ok(objDto);
        }
        [HttpGet("{trailId:int}",Name = "GetTrail")]
        public IActionResult GetTrail(int trailId)
        {
            var objFromDb = _trailRepository.GetTrail(trailId);
            if (objFromDb == null)
            {
                return NotFound();
            }
                var objDto = _mapper.Map<TrailDto>(objFromDb);
                return Ok(objDto);
        }
        [HttpPost]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_trailRepository.TrailExsits(trailDto.Name))
            {
                ModelState.AddModelError("", "Trail Exists!");
                return StatusCode(404, ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDto);
            if (!_trailRepository.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"wrong when saving the record {trailObj.Name} ");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetTrail", new { trailId = trailObj.Id }, trailObj);
        }
        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }
            var trailObj = _mapper.Map<Trail>(trailDto);
            if (!_trailRepository.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"wrong when updating the record {trailObj.Name} ");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!_trailRepository.TrailExsits(trailId))
            {
                return NotFound();
            }
            var trailObj = _trailRepository.GetTrail(trailId);
            if (!_trailRepository.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"wrong when Delete the record {trailObj.Name} ");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
