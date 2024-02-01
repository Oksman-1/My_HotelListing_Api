using HotelListing.Models;

namespace HotelListing.Services;

public interface IAuthManager
{
	Task<bool> Validateuser(LoginUserDto userDto);
	Task<string> CreateToken();
}
