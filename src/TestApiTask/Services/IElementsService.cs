using TestApiTask.Models;

namespace TestApiTask.Services;

public interface IElementsService
{
    Task<ElementsResponse> ProcessAsync(ElementsRequest request, CancellationToken cancellationToken);
}
