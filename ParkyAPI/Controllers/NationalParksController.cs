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
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepository;
        private readonly IMapper _mapper;
        public NationalParksController(IMapper mapper,INationalParkRepository npRepository)
        {
            _mapper = mapper;
            _npRepository = npRepository;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objFromDb = _npRepository.GetNationalParks();
            var objDto = new List<NationalParkDto>();
            foreach(var obj in objFromDb)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }
            
            return Ok(objDto);
        }
        [HttpGet("{nationalParkId:int}",Name = "GetNationalPark")]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var objFromDb = _npRepository.GetNationalPark(nationalParkId);
            if (objFromDb == null)
            {
                return NotFound();
            }
                var objDto = _mapper.Map<NationalParkDto>(objFromDb);
                return Ok(objDto);
        }
        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepository.NationalParkExsits(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepository.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"wrong when saving the record {nationalParkObj.Name} ");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetNationalPark", new { nationalParkId = nationalParkObj.id }, nationalParkObj);
        }
        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepository.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"wrong when updating the record {nationalParkObj.Name} ");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_npRepository.NationalParkExsits(nationalParkId))
            {
                return NotFound();
            }
            var nationalParkObj = _npRepository.GetNationalPark(nationalParkId);
            if (!_npRepository.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"wrong when Delete the record {nationalParkObj.Name} ");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
