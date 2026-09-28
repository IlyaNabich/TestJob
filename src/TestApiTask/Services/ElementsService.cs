using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using TestApiTask.Data;
using TestApiTask.Models;

namespace TestApiTask.Services;

public class ElementsService(IElementsRepository repository, ILogger<ElementsService> logger)
    : IElementsService
{
    private static readonly Regex EmailRegex = new(
        @"[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public async Task<ElementsResponse> ProcessAsync(ElementsRequest request, CancellationToken cancellationToken)
    {
        string url;
        try
        {
            url = DecodeBase64ToUtf8String(request.UrlB64!);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ошибка декодирования url_b64");
            return BuildErrorResponse(ErrorCodes.UrlBase64DecodeError, ex.Message);
        }
        
        string page;
        try
        {
            page = DecodeBase64ToUtf8String(request.PageB64!);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Ошибка декодирования page_b64");
            return BuildErrorResponse(ErrorCodes.PageBase64DecodeError, ex.Message);
        }
        
        try
        {
            var (elementsCount, attrList, elementRecords) =
                await ExtractElementsAsync(page, request.Selector!, request.Attribute!, cancellationToken);

            await repository.SaveElementsAsync(elementRecords, cancellationToken);

            var emailsList = ExtractEmails(page);

            var decryptedPlainText = DecryptAesEcbNoPadding(
                request.EncryptedTextBytesB64!,
                request.KeyBytesB64!);

            return new ElementsResponse
            {
                IsError = 0,
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty,
                ElementsCount = elementsCount,
                EmailsCount = emailsList.Count,
                Url = url,
                DecryptedPlainText = decryptedPlainText,
                ElementsAttrList = attrList,
                EmailsList = emailsList
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обработки запроса");
            return BuildErrorResponse(ErrorCodes.OtherError, ex.Message);
        }
    }
    private static string DecodeBase64ToUtf8String(string base64)
    {
        var bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }
    private static async Task<(int Count, List<string> AttrValues, List<ElementRecord> Records)> ExtractElementsAsync(
        string page,
        string selector,
        string attribute,
        CancellationToken cancellationToken)
    {
        var browsingContext = BrowsingContext.New(Configuration.Default);
        using var document = await browsingContext.OpenAsync(
            request => request.Content(page),
            cancellationToken);

        IHtmlCollection<IElement> elements = document.QuerySelectorAll(selector);

        var attrValues = new List<string>(elements.Length);
        var records = new List<ElementRecord>(elements.Length);

        foreach (var element in elements)
        {
            var attrValue = element.GetAttribute(attribute) ?? string.Empty;
            attrValues.Add(attrValue);
            records.Add(new ElementRecord(attrValue, element.OuterHtml));
        }

        return (elements.Length, attrValues, records);
    }
    private static List<string> ExtractEmails(string page)
    {
        return EmailRegex.Matches(page)
            .Select(m => m.Value)
            .ToList();
    }
    private static string DecryptAesEcbNoPadding(string encryptedTextBytesB64, string keyBytesB64)
    {
        var keyBytes = Convert.FromBase64String(keyBytesB64);
        var cipherBytes = Convert.FromBase64String(encryptedTextBytesB64);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        aes.Key = keyBytes;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
    private static ElementsResponse BuildErrorResponse(string errorCode, string errorMessage) => new()
    {
        IsError = 1,
        ErrorCode = errorCode,
        ErrorMessage = errorMessage,
        ElementsCount = 0,
        EmailsCount = 0,
        Url = string.Empty,
        DecryptedPlainText = string.Empty,
        ElementsAttrList = new List<string>(),
        EmailsList = new List<string>()
    };
}
