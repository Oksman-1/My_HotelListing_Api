using HotelListing.Data;
using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models;

public class CreateCountryDto
{
	[Required]
	[StringLength(maximumLength: 50, ErrorMessage = "Country Name is Too Long")]
	public string? Name { get; set; }

	[Required]
	[StringLength(maximumLength: 2, ErrorMessage = "Short Country Name is Too Long")]
	public string? ShortName { get; set; }
}


public class CountryDto : CreateCountryDto
{
	public int Id { get; set; }
	public IList<HotelDto>? Hotels { get; set; }

}
