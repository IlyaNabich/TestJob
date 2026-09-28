using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestApiTask.Models;
using TestApiTask.Services;

namespace TestApiTask.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ElementsController(IElementsService elementsService, IValidator<ElementsRequest> validator)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ElementsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ElementsResponse>> Process(
        [FromBody] ElementsRequest? request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || request is null)
        {
            var modelErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m));

            var message = string.Join(" ", modelErrors);
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Не удалось разобрать тело запроса как JSON-объект.";
            }

            return Ok(BuildValidationErrorResponse(message));
        }
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var message = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Ok(BuildValidationErrorResponse(message));
        }

        var response = await elementsService.ProcessAsync(request, cancellationToken);
        return Ok(response);
    }
    private static ElementsResponse BuildValidationErrorResponse(string message) => new()
    {
        IsError = 1,
        ErrorCode = ErrorCodes.ValidationError,
        ErrorMessage = message,
        ElementsCount = 0,
        EmailsCount = 0,
        Url = string.Empty,
        DecryptedPlainText = string.Empty,
        ElementsAttrList = new List<string>(),
        EmailsList = new List<string>()
    };
}
