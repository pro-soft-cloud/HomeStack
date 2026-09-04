using Ardalis.Result;
using HomeStack.Core.Models;

namespace HomeStack.Logic.Contracts;

public interface IHostInstanceManager
{
	Task<PagedResult<List<HostInstance>>> GetAllHostInstancesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

	Task<Result<HostInstance?>> GetBySystemIdAsync(Guid systemId, CancellationToken cancellationToken = default);

	Task<Result<HostInstance>> AddAsync(HostInstance item, CancellationToken cancellationToken = default);

	Task<Result<List<HostInstance>>> AddRangeAsync(List<HostInstance> listItems, CancellationToken cancellationToken = default);

	Task<Result<HostInstance>> UpdateAsync(HostInstance item, CancellationToken cancellationToken = default);

	Task<Result> DeleteAsync(Guid systemId, CancellationToken cancellationToken = default);
}