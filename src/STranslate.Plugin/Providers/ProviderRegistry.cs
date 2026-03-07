using System.Collections.Concurrent;

namespace STranslate.Plugin.Providers;

/// <summary>
/// Provider 注册表（基础脚手架）。
/// </summary>
public class ProviderRegistry
{
    private readonly ConcurrentDictionary<string, IProvider> _providers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 注册或覆盖 Provider。
    /// </summary>
    public void Register(IProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(provider.Descriptor.ProviderId);

        _providers[provider.Descriptor.ProviderId] = provider;
    }

    /// <summary>
    /// 尝试解析 Provider。
    /// </summary>
    public bool TryResolve<TProvider>(string providerId, out TProvider? provider)
        where TProvider : class, IProvider
    {
        provider = default;

        if (!_providers.TryGetValue(providerId, out var value))
            return false;

        provider = value as TProvider;
        return provider is not null;
    }

    /// <summary>
    /// 获取指定类型的 Provider。
    /// </summary>
    public IReadOnlyCollection<TProvider> GetProviders<TProvider>()
        where TProvider : class, IProvider
    {
        return _providers.Values.OfType<TProvider>().ToArray();
    }
}
