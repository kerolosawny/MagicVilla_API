using AutoMapper;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using MagicVilla_VillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/v{version:apiversion}/VillaNumberAPI")]
    [ApiController]
    //[ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class VillaNumberController :ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;


        public VillaNumberController(IVillaNumberRepository dbVillaNumber,IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVillaNumber = dbVillaNumber;
            _dbVilla = dbVilla;
            _mapper = mapper;
            this._response = new();
        }
        [HttpGet("GetString")]
        //[MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "Kerolos", "Awny" };
        }
    }
}
