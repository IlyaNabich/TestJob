using FluentValidation;
using TestApiTask.Models;

namespace TestApiTask.Validators;


public class ElementsRequestValidator : AbstractValidator<ElementsRequest>
{
    public ElementsRequestValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty()
            .WithMessage("Параметр 'selector' отсутствует или пуст.");

        RuleFor(x => x.Attribute)
            .NotEmpty()
            .WithMessage("Параметр 'attribute' отсутствует или пуст.");

        RuleFor(x => x.UrlB64)
            .NotEmpty()
            .WithMessage("Параметр 'url_b64' отсутствует или пуст.");

        RuleFor(x => x.PageB64)
            .NotEmpty()
            .WithMessage("Параметр 'page_b64' отсутствует или пуст.");

        RuleFor(x => x.EncryptedTextBytesB64)
            .NotEmpty()
            .WithMessage("Параметр 'encrypted_text_bytes_b64' отсутствует или пуст.");

        RuleFor(x => x.KeyBytesB64)
            .NotEmpty()
            .WithMessage("Параметр 'key_bytes_b64' отсутствует или пуст.");
    }
}
