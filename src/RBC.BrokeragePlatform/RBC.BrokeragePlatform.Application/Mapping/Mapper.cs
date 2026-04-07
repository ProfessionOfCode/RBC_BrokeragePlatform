using RBC.BrokeragePlatform.Application.Interfaces.Mapping;
using RBC.BrokeragePlatform.Domain.Entities;
using RBC.BrokeragePlatform.SharedCore.DTOs;

namespace RBC.BrokeragePlatform.Application.Mapping;

/// <summary>
/// Custom mapper implementation that handles mapping between domain entities and DTOs.
/// This mapper provides explicit mapping methods for all supported entity-DTO pairs.
/// Gracefully handles null inputs and empty collections without throwing exceptions.
/// </summary>
public class Mapper : IMapper
{
    /// <summary>
    /// Maps a single source object to a destination object.
    /// Supports mapping between Account/AccountDto, Position/PositionDto, and Equity/EquityDto.
    /// Returns null if the source is null instead of throwing an exception.
    /// </summary>
    public TDestination? Map<TSource, TDestination>(TSource? source)
        where TSource : class
        where TDestination : class
    {
        // Gracefully return null if source is null
        if (source == null)
            return null;

        return (source, typeof(TDestination)) switch
        {
            (Account account, Type t) when t == typeof(AccountDto) => MapAccountToAccountDto(account) as TDestination ?? throw new InvalidOperationException(),
            (AccountDto accountDto, Type t) when t == typeof(Account) => MapAccountDtoToAccount(accountDto) as TDestination ?? throw new InvalidOperationException(),
            (Position position, Type t) when t == typeof(PositionDto) => MapPositionToPositionDto(position) as TDestination ?? throw new InvalidOperationException(),
            (PositionDto positionDto, Type t) when t == typeof(Position) => MapPositionDtoToPosition(positionDto) as TDestination ?? throw new InvalidOperationException(),
            (Equity equity, Type t) when t == typeof(EquityDto) => MapEquityToEquityDto(equity) as TDestination ?? throw new InvalidOperationException(),
            (EquityDto equityDto, Type t) when t == typeof(Equity) => MapEquityDtoToEquity(equityDto) as TDestination ?? throw new InvalidOperationException(),
            _ => throw new NotSupportedException($"Mapping from {typeof(TSource).Name} to {typeof(TDestination).Name} is not supported.")
        };
    }

    /// <summary>
    /// Maps a collection of source objects to a collection of destination objects.
    /// Gracefully handles null collections by returning an empty enumerable.
    /// Filters out null items in the collection and only maps non-null items.
    /// </summary>
    public IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource>? source)
        where TSource : class
        where TDestination : class
    {
        // Return empty collection if source is null
        if (source == null)
            return [];

        // Filter null items and map non-null items
        return [.. source
            .Where(item => item != null)
            .Select(item => Map<TSource, TDestination>(item)!)            
            .Where(item => item != null)!];
    }

    #region Account Mappings

    private AccountDto MapAccountToAccountDto(Account source) => new()
    {
        AccountId = source.AccountId,
        ClientName = source.ClientName,
        AccountNumber = source.AccountNumber,
        CashBalance = source.CashBalance
    };

    private Account MapAccountDtoToAccount(AccountDto source) => new Account
    {
        AccountId = source.AccountId,
        ClientName = source.ClientName,
        AccountNumber = source.AccountNumber,
        CashBalance = source.CashBalance
    };

    #endregion

    #region Position Mappings

    private PositionDto MapPositionToPositionDto(Position source)
    {
        return new PositionDto
        {
            PositionId = source.PositionId,
            AccountId = source.AccountId,
            EquityId = source.EquityId,
            Symbol = source.Symbol,
            Quantity = source.Quantity,
            AverageCostPerShare = source.AverageCostPerShare,
            CurrentPrice = 0, // This is populated separately after mapping
            CurrentValue = 0  // This is calculated separately after mapping
        };
    }

    private Position MapPositionDtoToPosition(PositionDto source)
    {
        return new Position
        {
            PositionId = source.PositionId,
            AccountId = source.AccountId,
            EquityId = source.EquityId,
            Symbol = source.Symbol,
            Quantity = source.Quantity,
            AverageCostPerShare = source.AverageCostPerShare
        };
    }

    #endregion

    #region Equity Mappings

    private EquityDto MapEquityToEquityDto(Equity source)
    {
        return new EquityDto
        {
            EquityId = source.EquityId,
            Symbol = source.Symbol,
            CurrentPrice = source.CurrentPrice
        };
    }

    private Equity MapEquityDtoToEquity(EquityDto source)
    {
        return new Equity
        {
            EquityId = source.EquityId,
            Symbol = source.Symbol,
            CurrentPrice = source.CurrentPrice
        };
    }

    #endregion
}
