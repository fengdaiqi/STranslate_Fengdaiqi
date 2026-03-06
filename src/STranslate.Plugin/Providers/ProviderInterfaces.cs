namespace STranslate.Plugin.Providers;

/// <summary>
/// Provider 类型。
/// </summary>
public enum ProviderType
{
    Translation,
    Ocr,
    Tts,
}

/// <summary>
/// Provider 元信息。
/// </summary>
public sealed record ProviderDescriptor(
    string ProviderId,
    string DisplayName,
    ProviderType ProviderType,
    string? Description = null,
    string? Version = null);

/// <summary>
/// Provider 通用接口。
/// </summary>
public interface IProvider
{
    /// <summary>
    /// Provider 元信息。
    /// </summary>
    ProviderDescriptor Descriptor { get; }
}

/// <summary>
/// 翻译 Provider 接口。
/// </summary>
public interface ITranslationProvider : IProvider
{
    Task<TranslateResult> TranslateAsync(TranslateRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// OCR Provider 接口。
/// </summary>
public interface IOcrProvider : IProvider
{
    Task<OcrResult> RecognizeAsync(OcrRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// TTS Provider 接口。
/// </summary>
public interface ITtsProvider : IProvider
{
    Task<byte[]> SynthesizeAsync(string text, CancellationToken cancellationToken = default);
}
