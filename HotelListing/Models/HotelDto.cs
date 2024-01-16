using HotelListing.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.Models;

public class CreateHotelDto
{
	[Required]
	[StringLength(maximumLength: 150, ErrorMessage = "Hotel Name is Too Long")]
	public string? Name { get; set; }

	[Required]
	[StringLength(maximumLength: 50, ErrorMessage = "Address is Too Long")]
	public string? Address { get; set; }

	[Required]
	[Range(1, 5)]
	public double Rating { get; set; }

	[Required]
	public int CountryId { get; set; }
	
}


public class HotelDto : CreateHotelDto
{
	public int Id { get; set; }
	public CountryDto Country { get; set; }		 

}
