namespace Mvp.Selections.Api.Model.Request;

public class SearchOperationResult<T>
    : OperationResult<SearchResult<T>>
{
    public new SearchResult<T> Result => base.Result ??= new SearchResult<T>();
}