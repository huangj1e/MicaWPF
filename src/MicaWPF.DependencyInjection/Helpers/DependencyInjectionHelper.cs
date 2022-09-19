﻿using MicaWPF.DependencyInjection.Options;
using MicaWPF.DependencyInjection.Services;
using MicaWPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicaWPF.DependencyInjection.Helpers;
public static class DependencyInjectionHelper
{
    public static IHostBuilder UseMicaWPF(this IHostBuilder builder, Action<MicaWPFOptions>? options = null)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var cfg = new MicaWPFOptions();

        options?.Invoke(cfg);

        _ = builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton(cfg);
            services.AddSingleton<IThemeService, ThemeServiceDI>();
            services.AddSingleton<IAccentColorService, AccentColorServiceDI>();
        });

        return builder;
    }
}
