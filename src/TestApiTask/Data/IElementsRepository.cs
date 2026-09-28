namespace TestApiTask.Data;


public record ElementRecord(string? AttributeValue, string Html);

public interface IElementsRepository
{
    Task SaveElementsAsync(IReadOnlyCollection<ElementRecord> elements, CancellationToken cancellationToken);
}
