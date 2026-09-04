using Ardalis.Result;
using HomeStack.Core.Models;
using HomeStack.Database;
using HomeStack.Logic.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

	public async Task<PagedResult<List<HostInstance>>> GetAllHostInstancesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
	{
		if (pageNumber < 1 || pageSize < 1)
		{
			var validations = new List<ValidationError>
			{
				new() { Identifier = nameof(pageSize), ErrorMessage = "pageNumber and pageSize must be greater than 0." }
			};

			return Result<List<HostInstance>>.Invalid(validations).ToPagedResult(new PagedInfo(pageNumber, pageSize, 0, 0));
		}

		try
		{
			var totalRecords = await _dbContext.Set<HostInstance>().LongCountAsync(cancellationToken);

			var result = await _dbContext.Set<HostInstance>()
				.OrderBy(o => o.DisplayName)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(cancellationToken);

			var totalPages = totalRecords == 0
				? 0
				: (totalRecords + pageSize - 1) / pageSize;

			var pagedInfo = new PagedInfo(pageNumber, pageSize, totalPages, totalRecords);

			return Result<List<HostInstance>>.Success(result).ToPagedResult(pagedInfo);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error loading HostInstances (Page {PageNumber}, Size {PageSize})", pageNumber, pageSize);

			return Result<List<HostInstance>>.CriticalError($"Error loading HostInstances (Page {pageNumber}, Size {pageSize})").ToPagedResult(new PagedInfo(pageNumber, pageSize, 0, 0));
		}
	}

	public async Task<Result<HostInstance?>> GetBySystemIdAsync(Guid systemId, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await _dbContext.Set<HostInstance>().FirstOrDefaultAsync(x => x.SystemId == systemId, cancellationToken);

			return result == null
				? Result<HostInstance?>.NotFound($"HostInstance with SystemId '{systemId}' not found.")
				: Result<HostInstance?>.Success(result);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error loading HostInstance with SystemId {SystemId}", systemId);

			return Result<HostInstance?>.CriticalError($"Error loading HostInstance with SystemId {systemId}");
		}
	}

	public async Task<Result<HostInstance>> AddAsync(HostInstance item, CancellationToken cancellationToken = default)
	{
		var result = await AddRangeAsync([item], cancellationToken);

		return result.IsSuccess
			? Result<HostInstance>.Created(result.Value.First())
			: Result<HostInstance>.CriticalError([.. result.Errors]);
	}

	public async Task<Result<List<HostInstance>>> AddRangeAsync(List<HostInstance> listItems, CancellationToken cancellationToken = default)
	{
		try
		{
			await _dbContext.Set<HostInstance>().AddRangeAsync(listItems, cancellationToken);
			await _dbContext.SaveChangesAsync(cancellationToken);

			return Result<List<HostInstance>>.Success(listItems);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding one or more HostInstances");

			return Result<List<HostInstance>>.CriticalError("Error adding one or more HostInstances.");
		}
	}

	public async Task<Result<HostInstance>> UpdateAsync(HostInstance item, CancellationToken cancellationToken = default)
	{
		try
		{
			var dbItem = await GetBySystemIdAsync(item.SystemId, cancellationToken);

			if (dbItem is not { IsSuccess: true, Value: { } existingHostInstance })
			{
				return dbItem.IsNotFound()
					? Result<HostInstance>.NotFound($"HostInstance with SystemId '{item.SystemId}' not found.")
					: Result<HostInstance>.CriticalError([.. dbItem.Errors]);
			}

			existingHostInstance.IP = item.IP;
			existingHostInstance.DisplayName = item.DisplayName;
			existingHostInstance.ValidFrom = item.ValidFrom;
			existingHostInstance.ValidTo = item.ValidTo;
			existingHostInstance.LastUpdatedAt = item.LastUpdatedAt;
			existingHostInstance.LastUpdatedBy = item.LastUpdatedBy;

			_dbContext.Set<HostInstance>().Update(existingHostInstance);

			await _dbContext.SaveChangesAsync(cancellationToken);

			return Result<HostInstance>.Success(existingHostInstance);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error updating HostInstance with SystemId {SystemId}", item.SystemId);

			return Result<HostInstance>.CriticalError($"Error updating HostInstance with SystemId {item.SystemId}");
		}
	}

	public async Task<Result> DeleteAsync(Guid systemId, CancellationToken cancellationToken = default)
	{
		try
		{
			var dbItem = await GetBySystemIdAsync(systemId, cancellationToken);

			if (dbItem is not { Status: ResultStatus.Ok, Value: not null })
				return Result.NotFound($"HostInstance with SystemId '{systemId}' not found.");

			_dbContext.Set<HostInstance>().Remove(dbItem.Value);

			await _dbContext.SaveChangesAsync(cancellationToken);

			return Result.Success();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting HostInstance with SystemId {SystemId}", systemId);
			
			return Result.Error($"Error deleting HostInstance with SystemId {systemId}");
		}
	}
}
