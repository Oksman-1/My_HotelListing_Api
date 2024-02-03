using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Repository.RepositoryContracts;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[ApiVersion("2.0", Deprecated = true)]	
[Route("api/country")]
[ApiController]
public class CountryV2Controller : ControllerBase
{
	private readonly DatabaseContext _databaseContext;

	public CountryV2Controller(DatabaseContext databaseContext)
	{
		_databaseContext = databaseContext;
	}

	
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetCountries()
	{

		
		return Ok(_databaseContext.Countries);


	}
}
