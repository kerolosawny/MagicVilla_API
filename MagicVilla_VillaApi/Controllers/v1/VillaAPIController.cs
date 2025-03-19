using AutoMapper;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Logging;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaApi.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiversion}/VillaAPI")]
    [ApiController]
    [ApiVersion("1.0")]
    public class VillaAPIController : ControllerBase
    {
        //private readonly ILogger<VillaAPIController> _logger;
        //private readonly ILogging _logger;
        //ILogging logger -- ILogger<VillaAPIController> _logger in ctor
        //private readonly ApplicationDbContext _context;
        protected APIResponse _response;
        private readonly IVillaRepository _context;
        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ResponseCache(CacheProfileName= "Default30")]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name ="filterOccupency")] int? occupency,
            [FromQuery]string? search,  int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Villa> VillaList;
                if (occupency > 0)
                {
                    VillaList  = await _context.GetAllAsync(v=>v.Occupancy==occupency , pageSize:pageSize , pageNumber:pageNumber);
                }
                else
                {
                    VillaList = await _context.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    VillaList = VillaList.Where(v => v.Name.ToLower().Contains(search));// ||  v.Amenity.ToLower().Contains(search) );
                }
                Pagination pagination = new() { PageNumber = pageNumber , PageSize = pageSize };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _response.Result = _mapper.Map<List<VillaDTO>>(VillaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpGet("{Id:int}", Name = "GetVilla")]
        //[Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(200,Type=typeof(VillaDTO))] //can't remember what is this for 
        //[ResponseCache(Location =ResponseCacheLocation.None , NoStore =true)] //no store for error 
        public async Task<ActionResult<APIResponse>> GetVilla(int Id)
        {
            try
            {

                if (Id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var Villa = await _context.GetAsync(v => v.Id == Id);
                if (Villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaDTO>(Villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                //if (!ModelState.IsValid )
                //{
                //    return BadRequest(ModelState);
                //}

                if (await _context.GetAsync(v => v.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa already exists!");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }
                //if (villaDTO.Id > 0)
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError);
                //}
                Villa villa = _mapper.Map<Villa>(createDTO);

                await _context.CreateAsync(villa);
                //await _context.SaveAsync(); isn't needed

                _response.Result = _mapper.Map<VillaDTO>(villa); //mapping to villadto not createdto because 1st has Id to map vith villa
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        //if id in the next line differs from id arameter in delete method it will be a problem
        [HttpDelete("{Id:int}", Name = "DeleteVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult<APIResponse>> DeleteVilla(int Id)
        {
            try
            {

                if (Id == 0)
                {
                    return BadRequest();
                }
                var villa = await _context.GetAsync(v => v.Id == Id);
                if (villa == null)
                {
                    return NotFound();
                }
                await _context.RemoveAsync(villa);
                //await _context.SaveAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpPut("{Id:int}", Name = "UpdateVilla")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<APIResponse>> UpdateVilla(int Id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || updateDTO.Id != Id)
                {
                    return BadRequest();
                }
                //var Villa = _context.villas.FirstOrDefault(v => v.Id==Id); not needed because ef knows which record from id

                Villa model = _mapper.Map<Villa>(updateDTO);

                await _context.UpdateAsync(model);
                //await _context.SaveAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpPatch("{Id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdatePartialVilla(int Id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || Id == null)
            {
                return BadRequest();
            }
            var Villa = await _context.GetAsync(v => v.Id == Id, tracked: false);
            if (Villa == null)
            {
                return NotFound();
            }
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(Villa);

            patchDTO.ApplyTo(villaDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa model = _mapper.Map<Villa>(villaDTO);

            await _context.UpdateAsync(model);
            await _context.SaveAsync();

            return NoContent();
        }

    }
}
