using HomeStack.Core.Models;
using ProSoft.Result;

namespace HomeStack.Logic.Contracts;

public interface IHostInstanceManager
{
	Task<List<HostInstance>> GetAllHostInstancesAsync(CancellationToken cancellationToken);

	Task<HostInstance?> GetBySystemIdAsync(Guid systemId, CancellationToken cancellationToken);

	Task<HostInstance> AddAsync(HostInstance item, CancellationToken cancellationToken);

	Task<List<HostInstance>> AddRangeAsync(List<HostInstance> listItems, CancellationToken cancellationToken);

	Task<Result<HostInstance>> UpdateAsync(HostInstance item, CancellationToken cancellationToken);
}