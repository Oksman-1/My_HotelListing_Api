using HotelListing.Data;

namespace HotelListing.Repository.RepositoryContracts;

public interface IUnitOfWork : IDisposable
{
	IGenericRepository<Country> Countries { get; }
	IGenericRepository<Hotel> Hotels { get; }
	Task Save();


}
  