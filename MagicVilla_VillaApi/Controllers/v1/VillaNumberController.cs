using AutoMapper;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaApi.Controllers.v1
{
    [Route("api/v{version:apiversion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("1.0")] //, Deprecated= true
    //[ApiVersion("2.0")]
    public class VillaNumberController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;


        public VillaNumberController(IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVillaNumber = dbVillaNumber;
            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = new();
        }

        

        [HttpGet]
        //[MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumbersList = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbersList);
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

        //[HttpGet]
        //[MapToApiVersion("2.0")]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        [HttpGet("{Id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int Id)
        {
            try
            {
                if (Id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == Id);
                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                        if ((int)_response.StatusCode <200 ||  (int)_response.StatusCode > 299) //handling response IsSuccess
                            {
                                 _response.IsSuccess =false;
                            }
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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

        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if (await _dbVillaNumber.GetAsync(v => v.VillaNo == createDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number already exists!");
                    return BadRequest(ModelState);
                }

                if (await _dbVilla.GetAsync(v => v.Id == createDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Id is not valid!");
                    return BadRequest(ModelState);
                }
                if (createDTO == null) // check first , or not needed
                {
                    return BadRequest(createDTO);
                }
                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
                await _dbVillaNumber.CreateAsync(villaNumber);
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVillaNumber", new { Id = createDTO.VillaNo }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int Id)
        {
            try
            {

                if (Id == 0)
                {
                    return BadRequest();
                }
                VillaNumber villaNumber = await _dbVillaNumber.GetAsync(v => v.VillaNo == Id);

                if (villaNumber == null)
                {
                    return NotFound();
                }
                await _dbVillaNumber.RemoveAsync(villaNumber);
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




        //[HttpPut("Id" , Name = "UpdateVillaNumber")] //this makes the problem
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int Id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {

                if (updateDTO == null || Id != updateDTO.VillaNo)
                {
                    //ModelState.AddModelError("ErrorMessages", "It is not the same Id"); this line is mine
                    return BadRequest();
                }
                if (await _dbVilla.GetAsync(v => v.Id == updateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Id is Invalid");
                    return BadRequest(ModelState);
                }

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(updateDTO);
                await _dbVillaNumber.UpdateAsync(villaNumber);

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
    }
}
