using System.Net;
using ProSoft.HomeStack.Core.Models;

namespace ProSoft.HomeStack.Database.Contracts.Engines;

public interface IHomeStackInstanceEngine
{
	Task<HomeStackInstance?> GetAsync(Guid systemId, CancellationToken cancellationToken);

	Task<HomeStackInstance?> GetAsync(int id, CancellationToken cancellationToken);

	Task<HomeStackInstance?> GetAsync(string displayName, CancellationToken cancellationToken);

	Task<HomeStackInstance?> GetAsync(IPAddress ipAddress, CancellationToken cancellationToken);

	Task<List<HomeStackInstance>> GetAllAsync(int skip, int take, CancellationToken cancellationToken);
}