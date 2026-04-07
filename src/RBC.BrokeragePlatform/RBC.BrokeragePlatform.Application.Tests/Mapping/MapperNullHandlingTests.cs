using RBC.BrokeragePlatform.Application.Mapping;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.Application.Tests.Mapping;

public class MapperNullHandlingTests
{
    private readonly Mapper _mapper = new();

    #region Map<T> Null Handling Tests

    [Fact]
    public void Map_ReturnsNull_WhenSourceAccountIsNull()
    {
        // Arrange
        Account? source = null;

        // Act
        var result = _mapper.Map<Account, AccountDto>(source);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Map_ReturnsNull_WhenSourcePositionIsNull()
    {
        // Arrange
        Position? source = null;

        // Act
        var result = _mapper.Map<Position, PositionDto>(source);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Map_ReturnsNull_WhenSourceEquityIsNull()
    {
        // Arrange
        Equity? source = null;

        // Act
        var result = _mapper.Map<Equity, EquityDto>(source);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Map_ReturnsNull_WhenSourceAccountDtoIsNull()
    {
        // Arrange
        AccountDto? source = null;

        // Act
        var result = _mapper.Map<AccountDto, Account>(source);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Map_ReturnsNull_WhenSourcePositionDtoIsNull()
    {
        // Arrange
        PositionDto? source = null;

        // Act
        var result = _mapper.Map<PositionDto, Position>(source);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Map_ReturnsNull_WhenSourceEquityDtoIsNull()
    {
        // Arrange
        EquityDto? source = null;

        // Act
        var result = _mapper.Map<EquityDto, Equity>(source);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Map_SuccessfullyMapsAccount_WhenSourceIsNotNull()
    {
        // Arrange
        var source = new Account
        {
            AccountId = 1,
            ClientName = "John Doe",
            AccountNumber = "ACC001",
            CashBalance = 10000m
        };

        // Act
        var result = _mapper.Map<Account, AccountDto>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(source.AccountId, result.AccountId);
        Assert.Equal(source.ClientName, result.ClientName);
        Assert.Equal(source.AccountNumber, result.AccountNumber);
        Assert.Equal(source.CashBalance, result.CashBalance);
    }

    #endregion

    #region MapCollection<T> Null Collection Handling Tests

    [Fact]
    public void MapCollection_ReturnsEmpty_WhenSourceCollectionIsNull()
    {
        // Arrange
        IEnumerable<Account>? source = null;

        // Act
        var result = _mapper.MapCollection<Account, AccountDto>(source);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MapCollection_ReturnsEmpty_WhenSourceCollectionOfPositionsIsNull()
    {
        // Arrange
        IEnumerable<Position>? source = null;

        // Act
        var result = _mapper.MapCollection<Position, PositionDto>(source);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MapCollection_ReturnsEmpty_WhenSourceCollectionOfEquitiesIsNull()
    {
        // Arrange
        IEnumerable<Equity>? source = null;

        // Act
        var result = _mapper.MapCollection<Equity, EquityDto>(source);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MapCollection_ReturnsEmpty_WhenSourceCollectionIsEmptyList()
    {
        // Arrange
        var source = new List<Account>();

        // Act
        var result = _mapper.MapCollection<Account, AccountDto>(source);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MapCollection_ReturnsEmpty_WhenSourceCollectionIsEmptyArray()
    {
        // Arrange
        var source = Array.Empty<Position>();

        // Act
        var result = _mapper.MapCollection<Position, PositionDto>(source);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region MapCollection<T> Null Items in Collection Tests

    [Fact]
    public void MapCollection_FiltersNullItems_WhenCollectionContainsNullAccounts()
    {
        // Arrange
        var source = new List<Account?>
        {
            new() { AccountId = 1, ClientName = "Account 1", AccountNumber = "ACC001", CashBalance = 1000m },
            null,
            new() { AccountId = 2, ClientName = "Account 2", AccountNumber = "ACC002", CashBalance = 2000m }
        };

        // Act
        var result = _mapper.MapCollection<Account, AccountDto>(source.Where(a => a != null).Cast<Account>().ToList());

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, item => Assert.NotNull(item));
    }

    [Fact]
    public void MapCollection_FiltersNullItems_WhenCollectionContainsNullPositions()
    {
        // Arrange
        var source = new List<Position?>
        {
            new() { PositionId = 1, AccountId = 1, EquityId = 1, Symbol = "AAPL", Quantity = 10, AverageCostPerShare = 100m },
            null,
            new() { PositionId = 2, AccountId = 1, EquityId = 2, Symbol = "MSFT", Quantity = 5, AverageCostPerShare = 300m }
        };

        // Act
        var result = _mapper.MapCollection<Position, PositionDto>(source.Where(p => p != null).Cast<Position>().ToList());

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, item => Assert.NotNull(item));
    }

    #endregion

    #region MapCollection<T> Successful Mapping Tests

    [Fact]
    public void MapCollection_SuccessfullyMapsAccounts_WhenSourceIsPopulated()
    {
        // Arrange
        var source = new List<Account>
        {
            new() { AccountId = 1, ClientName = "Account 1", AccountNumber = "ACC001", CashBalance = 1000m },
            new() { AccountId = 2, ClientName = "Account 2", AccountNumber = "ACC002", CashBalance = 2000m },
            new() { AccountId = 3, ClientName = "Account 3", AccountNumber = "ACC003", CashBalance = 3000m }
        };

        // Act
        var result = _mapper.MapCollection<Account, AccountDto>(source).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        for (int i = 0; i < source.Count; i++)
        {
            Assert.Equal(source[i].AccountId, result[i].AccountId);
            Assert.Equal(source[i].ClientName, result[i].ClientName);
        }
    }

    [Fact]
    public void MapCollection_SuccessfullyMapsPositions_WhenSourceIsPopulated()
    {
        // Arrange
        var source = new List<Position>
        {
            new() { PositionId = 1, AccountId = 1, EquityId = 1, Symbol = "AAPL", Quantity = 10, AverageCostPerShare = 100m },
            new() { PositionId = 2, AccountId = 1, EquityId = 2, Symbol = "MSFT", Quantity = 5, AverageCostPerShare = 300m }
        };

        // Act
        var result = _mapper.MapCollection<Position, PositionDto>(source).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(source[0].PositionId, result[0].PositionId);
        Assert.Equal(source[0].Symbol, result[0].Symbol);
        Assert.Equal(source[1].PositionId, result[1].PositionId);
        Assert.Equal(source[1].Symbol, result[1].Symbol);
    }

    [Fact]
    public void MapCollection_SuccessfullyMapsEquities_WhenSourceIsPopulated()
    {
        // Arrange
        var source = new List<Equity>
        {
            new() { EquityId = 1, Symbol = "AAPL", CurrentPrice = 150m },
            new() { EquityId = 2, Symbol = "MSFT", CurrentPrice = 300m }
        };

        // Act
        var result = _mapper.MapCollection<Equity, EquityDto>(source).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(source[0].EquityId, result[0].EquityId);
        Assert.Equal(source[0].Symbol, result[0].Symbol);
        Assert.Equal(source[1].EquityId, result[1].EquityId);
        Assert.Equal(source[1].Symbol, result[1].Symbol);
    }

    #endregion

    #region Reverse Mapping Null Handling Tests

    [Fact]
    public void Map_ReturnsNull_WhenSourceAccountDtoIsNullForReverseMapping()
    {
        // Arrange
        AccountDto? source = null;

        // Act
        var result = _mapper.Map<AccountDto, Account>(source);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void MapCollection_ReturnsEmpty_WhenSourceAccountDtoCollectionIsNullForReverseMapping()
    {
        // Arrange
        IEnumerable<AccountDto>? source = null;

        // Act
        var result = _mapper.MapCollection<AccountDto, Account>(source);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void MapCollection_SuccessfullyReverseMapsDtos_WhenSourceIsPopulated()
    {
        // Arrange
        var source = new List<AccountDto>
        {
            new() { AccountId = 1, ClientName = "Account 1", AccountNumber = "ACC001", CashBalance = 1000m },
            new() { AccountId = 2, ClientName = "Account 2", AccountNumber = "ACC002", CashBalance = 2000m }
        };

        // Act
        var result = _mapper.MapCollection<AccountDto, Account>(source).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(source[0].AccountId, result[0].AccountId);
        Assert.Equal(source[0].ClientName, result[0].ClientName);
    }

    #endregion
}
