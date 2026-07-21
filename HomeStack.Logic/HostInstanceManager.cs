using HomeStack.Core.Models;
using HomeStack.Database;
using HomeStack.Logic.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProSoft.Result;

namespace HomeStack.Logic;

public sealed class HostInstanceManager : IHostInstanceManager
{
	private readonly ILogger<HostInstanceManager> _logger;
	private readonly HomeStackDbContext _dbContext;

	public HostInstanceManager(ILogger<HostInstanceManager> logger, HomeStackDbContext dbContext)
	{
		_logger = logger;
		_dbContext = dbContext;
	}

	public async Task<List<HostInstance>> GetAllHostInstancesAsync(CancellationToken cancellationToken)
	{
		var result = _dbContext.Set<HostInstance>().ToList();

		await Task.CompletedTask;
		
		return result;
	}

	public Task<HostInstance?> GetBySystemIdAsync(Guid systemId, CancellationToken cancellationToken)
	{
		return _dbContext.Set<HostInstance>().FirstOrDefaultAsync(x => x.SystemId == systemId, cancellationToken);
	}

	public async Task<HostInstance> AddAsync(HostInstance item, CancellationToken cancellationToken)
	{
		var result = await AddRangeAsync([item], cancellationToken);

		return result.First();
	}

	public async Task<List<HostInstance>> AddRangeAsync(List<HostInstance> listItems, CancellationToken cancellationToken)
	{
		await _dbContext.Set<HostInstance>().AddRangeAsync(listItems, cancellationToken);
		await _dbContext.SaveChangesAsync(cancellationToken);

		return listItems;
	}

	public async Task<Result<HostInstance>> UpdateAsync(HostInstance item, CancellationToken cancellationToken)
	{
		var dbItem = await GetBySystemIdAsync(item.SystemId, cancellationToken);

		if (dbItem == null)
		{
			var result = new Result<HostInstance>
			(
				dbItem,
				ResultStatus.Failure,
				[
					new Message(MessageCategory.Technical, MessageType.Error, $"HostInstance with SystemId '{item.SystemId}' not found.")
				]
			);
			return result;
		}

		dbItem.IP = item.IP;
		dbItem.DisplayName = item.DisplayName;
		dbItem.ValidFrom = item.ValidFrom;
		dbItem.ValidTo = item.ValidTo;
		dbItem.LastUpdatedAt = item.LastUpdatedAt;
		dbItem.LastUpdatedBy = item.LastUpdatedBy;

		var updatedItem = _dbContext.Set<HostInstance>().Update(dbItem);

		await _dbContext.SaveChangesAsync(cancellationToken);

		return new Result<HostInstance>(updatedItem.Entity, ResultStatus.Success);
	}
}
