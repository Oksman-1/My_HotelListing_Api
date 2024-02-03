using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Repository.RepositoryContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers;

[Route("api/Hotel")]
[ApiController]
public class HotelController : ControllerBase
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<HotelController> _logger;
	private readonly IMapper _mapper;


	public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
		_mapper = mapper;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetHotels()
	{
		
			var hotels = await _unitOfWork.Hotels.GetAll();
			var results = _mapper.Map<IList<HotelDto>>(hotels);

			return Ok(results);		
	}


	[Authorize(Roles = "Administrator")]
	[HttpGet("{id:int}", Name = "GetHotel")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetHotel(int id)
	{
		
			var hotel = await _unitOfWork.Hotels.Get(q => q.Id.Equals(id), new List<string> { "Country" });
			var result = _mapper.Map<HotelDto>(hotel);

			return Ok(result);	

	}


	//[Authorize(Roles = "Administrator")]
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDto hotelDto)
	{
		if (!ModelState.IsValid)
		{
			_logger.LogError($"Invalid Post Attempt in {nameof(CreateHotel)}");
			return BadRequest(ModelState);
		}
		
			var hotel = _mapper.Map<Hotel>(hotelDto);
			await _unitOfWork.Hotels.Insert(hotel);
			await _unitOfWork.Save();

			return CreatedAtRoute("GetHotel", new {id = hotel.Id}, hotel);		

	}


	[Authorize(Roles = "Administrator")]
	[HttpPut("{id:int}")]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDto hotelDto)
	{
		if (!ModelState.IsValid || id < 1)
		{
			_logger.LogError($"Invalid UPDATE Attempt in {nameof(UpdateHotel)}");
			return BadRequest(ModelState);
		}
		
			var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
			if(hotel == null)
			{
				_logger.LogError($"Invalid UPDATE Attempt in {nameof(UpdateHotel)}");
				return BadRequest("Submitted data is Invalid");
			}

			_mapper.Map(hotelDto, hotel);
			_unitOfWork.Hotels.Update(hotel);	
			await _unitOfWork.Save();	
			
			return NoContent();		

	}


	[Authorize]
	[HttpDelete("{id:int}")]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> DeleteHotel(int id)
	{
		if (id < 1)
		{
			_logger.LogError($"Invalid DELETE attempt in {nameof(DeleteHotel)}");
			return BadRequest();
		}

		
			var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
			if (hotel == null)
			{
				_logger.LogError($"Invalid DELETE Attempt in {nameof(DeleteHotel)}");
				return BadRequest("Submitted data is Invalid");
			}

			await _unitOfWork.Hotels.Delete(id);
			await _unitOfWork.Save();

			return NoContent();
		
	}
}
