namespace STranslate.Plugin.Providers;

/// <summary>
/// 微信 OCR Provider 基类（脚手架）。
/// </summary>
public abstract class WechatOcrProviderBase : IOcrProvider
{
    public abstract ProviderDescriptor Descriptor { get; }

    public abstract IEnumerable<LangEnum> SupportedLanguages { get; }

    public async Task<OcrResult> RecognizeAsync(OcrRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRequest(request);
            return await RecognizeCoreAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            return new OcrResult().Fail(ex.Message);
        }
    }

    protected abstract Task<OcrResult> RecognizeCoreAsync(OcrRequest request, CancellationToken cancellationToken);

    protected virtual void ValidateRequest(OcrRequest request)
    {
        if (request.ImageData is null || request.ImageData.Length == 0)
            throw new ArgumentException("ImageData 不能为空", nameof(request));

        if (!SupportedLanguages.Contains(request.Language))
            throw new NotSupportedException($"当前 Provider 不支持语言: {request.Language}");
    }
}
