# Provider 脚手架与集成点说明

本文档对应本次新增的最小化 Provider 脚手架，仅提供“可落地扩展点”，不改变当前插件加载与服务执行流程。

## 新增内容

- `STranslate.Plugin/Providers/ProviderInterfaces.cs`
  - 定义 `IProvider`、`ITranslationProvider`、`IOcrProvider`、`ITtsProvider`
  - 定义 `ProviderType` 与 `ProviderDescriptor`
- `STranslate.Plugin/Providers/ProviderRegistry.cs`
  - 提供基础注册、解析、按类型枚举能力
- `STranslate.Plugin/Providers/OpenAiCompatibleTranslationProviderBase.cs`
  - 提供 OpenAI 兼容翻译 Provider 的请求构建/解析骨架
- `STranslate.Plugin/Providers/WechatOcrProviderBase.cs`
  - 提供微信 OCR 风格 Provider 的输入校验和统一异常收敛骨架
- `STranslate.Plugin/Providers/QwenTtsProviderBase.cs`
  - 提供 Qwen TTS 风格 Provider 的最小请求骨架

## 集成点（保持向后兼容）

当前主流程仍基于 `ITranslatePlugin` / `IOcrPlugin` / `ITtsPlugin`，本次未替换也未修改现有调用链：

1. **插件创建与生命周期**
   - 现有入口：`STranslate/Core/ServiceManager.cs` 中 `CreateService` 仍通过 `PluginMetaData.CreatePluginService()` 创建插件实例。
2. **翻译执行链路**
   - 现有入口：`STranslate/ViewModels/MainWindowViewModel.cs` 中以 `ITranslatePlugin` 执行翻译。
3. **OCR 执行链路**
   - 现有入口：`STranslate/ViewModels/MainWindowViewModel.cs`、`OcrWindowViewModel.cs`、`ImageTranslateWindowViewModel.cs` 仍以 `IOcrPlugin` 执行识别。
4. **TTS 执行链路**
   - 现有入口：`STranslate/ViewModels/MainWindowViewModel.cs` 仍以 `ITtsPlugin` 执行播放。

## 推荐接入方式（后续增量）

- 在具体插件内部组合 Provider（而非替换插件接口）：
  - 翻译插件：`ITranslatePlugin.TranslateAsync` 内委托给 `ITranslationProvider`
  - OCR 插件：`IOcrPlugin.RecognizeAsync` 内委托给 `IOcrProvider`
  - TTS 插件：`ITtsPlugin.PlayAudioAsync` 内委托给 `ITtsProvider` + `IAudioPlayer`
- 使用 `ProviderRegistry` 做插件内多 Provider 路由（按服务商、模型或配置切换）。

> 以上方式可做到对现有 UI、ServiceSettings、插件包格式零侵入。
