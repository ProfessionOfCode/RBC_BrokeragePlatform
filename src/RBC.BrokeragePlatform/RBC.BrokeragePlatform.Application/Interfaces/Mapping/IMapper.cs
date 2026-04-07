namespace RBC.BrokeragePlatform.Application.Interfaces.Mapping;

/// <summary>
/// Defines a contract for mapping between domain entities and DTOs.
/// Provides methods to map individual objects and collections between source and destination types.
/// Gracefully handles null inputs and returns nullable outputs.
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Maps a single source object to a destination object.
    /// Returns null if the source is null.
    /// </summary>
    /// <typeparam name="TSource">The source object type.</typeparam>
    /// <typeparam name="TDestination">The destination object type.</typeparam>
    /// <param name="source">The source object to map. Can be null.</param>
    /// <returns>A new destination object with values mapped from the source, or null if source is null.</returns>
    TDestination? Map<TSource, TDestination>(TSource? source)
        where TSource : class
        where TDestination : class;

    /// <summary>
    /// Maps a collection of source objects to destination objects.
    /// Returns an empty collection if the source is null.
    /// Skips null items within the collection.
    /// </summary>
    /// <typeparam name="TSource">The source collection element type.</typeparam>
    /// <typeparam name="TDestination">The destination collection element type.</typeparam>
    /// <param name="source">The collection of source objects to map. Can be null.</param>
    /// <returns>An enumerable of destination objects with values mapped from the source collection. Returns empty enumerable if source is null or contains only null items.</returns>
    IEnumerable<TDestination> MapCollection<TSource, TDestination>(IEnumerable<TSource>? source)
        where TSource : class
        where TDestination : class;
}
