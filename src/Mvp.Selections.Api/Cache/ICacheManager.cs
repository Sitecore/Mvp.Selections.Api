namespace Mvp.Selections.Api.Cache;

public interface ICacheManager
{
    bool TryGet<T>(string key, out T? value);

    T Set<T>(CacheManager.CacheCollection collection, string key, T value);

    void Clear(CacheManager.CacheCollection collection);

    string GetMvpProfileSearchResultsKey(
        string? text = null,
        IList<short>? mvpTypeIds = null,
        IList<short>? years = null,
        IList<short>? countryIds = null,
        bool onlyFinalized = true);
}