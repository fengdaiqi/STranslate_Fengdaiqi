using System.Text.Json;

namespace STranslate.Plugin.Providers;

/// <summary>
/// OpenAI 兼容翻译 Provider 基类（脚手架）。
/// </summary>
public abstract class OpenAiCompatibleTranslationProviderBase : ITranslationProvider
{
    protected OpenAiCompatibleTranslationProviderBase(IHttpService httpService)
    {
        HttpService = httpService;
    }

    protected IHttpService HttpService { get; }

    public abstract ProviderDescriptor Descriptor { get; }

    /// <summary>
    /// API Key。
    /// </summary>
    protected abstract string ApiKey { get; }

    /// <summary>
    /// 兼容 OpenAI 的 Chat Completions 地址。
    /// </summary>
    protected virtual string Endpoint => "https://api.openai.com/v1/chat/completions";

    /// <summary>
    /// 模型名称。
    /// </summary>
    protected abstract string Model { get; }

    public virtual async Task<TranslateResult> TranslateAsync(TranslateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = BuildPayload(request);
            var options = BuildRequestOptions();
            var response = await HttpService.PostAsync<JsonElement>(Endpoint, payload, options, cancellationToken);
            var translated = ParseTranslatedText(response);

            return new TranslateResult
            {
                IsSuccess = !string.IsNullOrWhiteSpace(translated),
                Text = translated ?? string.Empty,
            };
        }
        catch (Exception ex)
        {
            return new TranslateResult
            {
                IsSuccess = false,
                Text = ex.Message,
            };
        }
    }

    protected virtual object BuildPayload(TranslateRequest request)
    {
        var prompt = $"Translate the following text from {request.SourceLang} to {request.TargetLang}. Return only translated text.\n\n{request.Text}";

        return new
        {
            model = Model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0,
        };
    }

    protected virtual Options BuildRequestOptions()
    {
        return new Options
        {
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {ApiKey}",
            },
        };
    }

    protected virtual string? ParseTranslatedText(JsonElement? response)
    {
        if (response is null)
            return null;

        if (!response.Value.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            return null;

        var first = choices[0];
        if (!first.TryGetProperty("message", out var message))
            return null;

        if (!message.TryGetProperty("content", out var content))
            return null;

        return content.GetString();
    }
}
