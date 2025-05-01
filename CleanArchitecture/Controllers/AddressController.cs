using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress(AddAddressDto addAddress) 
        {
            return Ok(await _addressService.AddCompanyUserAddress(addAddress));
        }

        [HttpGet("GetAddressesByCurrunt")]
        public async Task<IActionResult> GetAddressByLoggedIUser()
        {
            return Ok(await _addressService.GetCompanyAddress());
        }

    }
}
