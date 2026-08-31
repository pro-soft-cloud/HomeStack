using System.Diagnostics.CodeAnalysis;
using System.Net;
using Ardalis.Result;
using HomeStack.Core.Models;
using HomeStack.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MockQueryable.Moq;
using Xunit;

namespace HomeStack.Logic.Tests;

[ExcludeFromCodeCoverage]
public class HostInstanceManagerTests
{
	private readonly Mock<ILogger<HostInstanceManager>> _loggerMock = new();
	private readonly Mock<HomeStackDbContext> _dbContextMock;

	public HostInstanceManagerTests()
	{
		var options = new DbContextOptionsBuilder<HomeStackDbContext>().Options;
		_dbContextMock = new Mock<HomeStackDbContext>(options);
	}

	private static HostInstance CreateHostInstance(Guid? systemId = null, string displayName = "Test-Host") =>
		new()
		{
			SystemId = systemId ?? Guid.NewGuid(),
			DisplayName = displayName,
			IP = IPAddress.Parse("127.0.0.1")
		};

	private HostInstanceManager CreateSut() => new(_loggerMock.Object, _dbContextMock.Object);

	private void SetupHostInstances(List<HostInstance> hostInstances)
	{
		var mockSet = hostInstances.AsQueryable().BuildMockDbSet();

		_dbContextMock
			.Setup(x => x.Set<HostInstance>())
			.Returns(mockSet.Object);
	}

	[Fact]
	public async Task GetAllHostInstancesAsync_WithInvalidPageNumber_ReturnsInvalid()
	{
		var sut = CreateSut();

		var result = await sut.GetAllHostInstancesAsync(pageNumber: 0, pageSize: 10);

		Assert.True(result.Status == ResultStatus.Invalid);
	}

	[Fact]
	public async Task GetAllHostInstancesAsync_WithInvalidPageSize_ReturnsInvalid()
	{
		var sut = CreateSut();

		var result = await sut.GetAllHostInstancesAsync(pageNumber: 1, pageSize: 0);

		Assert.True(result.Status == ResultStatus.Invalid);
	}

	[Fact]
	public async Task GetAllHostInstancesAsync_WithValidPaging_ReturnsPagedResult()
	{
		var hostInstances = new List<HostInstance>
		{
			CreateHostInstance(displayName: "Beta"),
			CreateHostInstance(displayName: "Alpha"),
			CreateHostInstance(displayName: "Gamma")
		};

		SetupHostInstances(hostInstances);

		var sut = CreateSut();

		var result = await sut.GetAllHostInstancesAsync(pageNumber: 1, pageSize: 2);

		Assert.True(result.IsSuccess);
		Assert.Equal(2, result.Value.Count);
		Assert.Equal("Alpha", result.Value[0].DisplayName);
		Assert.Equal("Beta", result.Value[1].DisplayName);
		Assert.Equal(3, result.PagedInfo.TotalRecords);
		Assert.Equal(2, result.PagedInfo.TotalPages);
	}

	[Fact]
	public async Task GetAllHostInstancesAsync_WhenExceptionThrown_ReturnsCriticalError()
	{
		_dbContextMock
			.Setup(x => x.Set<HostInstance>())
			.Throws(new InvalidOperationException("boom"));

		var sut = CreateSut();

		var result = await sut.GetAllHostInstancesAsync(pageNumber: 1, pageSize: 10);

		Assert.Equal(ResultStatus.CriticalError, result.Status);
	}

	[Fact]
	public async Task GetBySystemIdAsync_WhenFound_ReturnsSuccess()
	{
		var systemId = Guid.NewGuid();
		var hostInstances = new List<HostInstance> { CreateHostInstance(systemId) };

		SetupHostInstances(hostInstances);

		var sut = CreateSut();

		var result = await sut.GetBySystemIdAsync(systemId);

		Assert.True(result.IsSuccess);
		Assert.Equal(systemId, result.Value!.SystemId);
	}

	[Fact]
	public async Task GetBySystemIdAsync_WhenNotFound_ReturnsNotFound()
	{
		SetupHostInstances([]);

		var sut = CreateSut();

		var result = await sut.GetBySystemIdAsync(Guid.NewGuid());

		Assert.True(result.IsNotFound());
	}

	[Fact]
	public async Task GetBySystemIdAsync_WhenExceptionThrown_ReturnsCriticalError()
	{
		_dbContextMock
			.Setup(x => x.Set<HostInstance>())
			.Throws(new InvalidOperationException("boom"));

		var sut = CreateSut();

		var result = await sut.GetBySystemIdAsync(Guid.NewGuid());

		Assert.Equal(ResultStatus.CriticalError, result.Status);
	}

	[Fact]
	public async Task AddAsync_WithValidItem_ReturnsCreated()
	{
		var mockSet = new List<HostInstance>().AsQueryable().BuildMockDbSet();

		_dbContextMock
			.Setup(x => x.Set<HostInstance>())
			.Returns(mockSet.Object);

		_dbContextMock
			.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(1);

		var sut = CreateSut();
		var item = CreateHostInstance();

		var result = await sut.AddAsync(item);

		Assert.Equal(ResultStatus.Created, result.Status);
		mockSet.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<HostInstance>>(l => l.Single() == item), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task AddAsync_WhenExceptionThrown_ReturnsCriticalError()
	{
		var mockSet = new List<HostInstance>().AsQueryable().BuildMockDbSet();

		_dbContextMock
			.Setup(x => x.Set<HostInstance>())
			.Returns(mockSet.Object);

		_dbContextMock
			.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new InvalidOperationException("boom"));

		var sut = CreateSut();

		var result = await sut.AddAsync(CreateHostInstance());

		Assert.Equal(ResultStatus.CriticalError, result.Status);
	}

	[Fact]
	public async Task UpdateAsync_WhenItemExists_UpdatesAndReturnsSuccess()
	{
		var systemId = Guid.NewGuid();
		var existing = CreateHostInstance(systemId, "Old-Name");
		var hostInstances = new List<HostInstance> { existing };

		SetupHostInstances(hostInstances);

		_dbContextMock
			.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(1);

		var sut = CreateSut();
		var updateItem = CreateHostInstance(systemId, "New-Name");

		var result = await sut.UpdateAsync(updateItem);

		Assert.True(result.IsSuccess);
		Assert.Equal("New-Name", result.Value.DisplayName);
	}

	[Fact]
	public async Task UpdateAsync_WhenItemDoesNotExist_ReturnsNotFound()
	{
		SetupHostInstances([]);

		var sut = CreateSut();

		var result = await sut.UpdateAsync(CreateHostInstance());

		Assert.True(result.IsNotFound());
	}

	[Fact]
	public async Task UpdateAsync_WhenExceptionThrown_ReturnsCriticalError()
	{
		var systemId = Guid.NewGuid();
		var existing = CreateHostInstance(systemId);
		SetupHostInstances([existing]);

		_dbContextMock
			.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
			.ThrowsAsync(new InvalidOperationException("boom"));

		var sut = CreateSut();

		var result = await sut.UpdateAsync(CreateHostInstance(systemId));

		Assert.Equal(ResultStatus.CriticalError, result.Status);
	}
}
