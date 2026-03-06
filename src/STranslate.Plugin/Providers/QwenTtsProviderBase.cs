namespace STranslate.Plugin.Providers;

/// <summary>
/// Qwen TTS Provider 基类（脚手架）。
/// </summary>
public abstract class QwenTtsProviderBase : ITtsProvider
{
    protected QwenTtsProviderBase(IHttpService httpService)
    {
        HttpService = httpService;
    }

    protected IHttpService HttpService { get; }

    public abstract ProviderDescriptor Descriptor { get; }

    protected abstract string ApiKey { get; }

    protected virtual string Model => "qwen-tts";

    protected virtual string Endpoint => "https://dashscope.aliyuncs.com/compatible-mode/v1/audio/speech";

    protected virtual string Voice => "Chelsie";

    public virtual Task<byte[]> SynthesizeAsync(string text, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            model = Model,
            input = text,
            voice = Voice,
        };

        var options = new Options
        {
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {ApiKey}",
            },
        };

        return HttpService.PostAsBytesAsync(Endpoint, payload, options, cancellationToken);
    }
}
