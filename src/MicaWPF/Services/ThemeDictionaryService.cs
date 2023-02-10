﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MicaWPF.Services;

public sealed class ThemeDictionaryService : INotifyPropertyChanged, IThemeDictionaryService
{
    private static Uri? _currentThemeSource;

    public event PropertyChangedEventHandler? PropertyChanged;

    public static ThemeDictionaryService Current { get; } = new();

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private ThemeDictionaryService()
    {
    }

    /// <summary>
    /// get current theme resource dictionary
    /// </summary>
    private static List<ResourceDictionary?> GetThemeResourceDictionary()
    {
        return (from dictionary in Application.Current.Resources.MergedDictionaries
                where dictionary.Contains("MicaWPF.Colors.ApplicationBackgroundColor")
                select dictionary).ToList();
    }

    /// <summary>
    /// set the current theme source
    /// </summary>
    /// <param name="source"></param>
    public void SetThemeSource(Uri source)
    {
        lock (source)
        {
            _currentThemeSource = source;

            var oldThemes = GetThemeResourceDictionary();
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            WindowHelper.RefreshAllWindowsContents();

            dictionaries.Add(new ResourceDictionary
            {
                Source = source
            });

            foreach (var oldTheme in oldThemes)
            {
                _ = dictionaries.Remove(oldTheme);
            }
        }
    }

    /// <summary>
    /// current theme source
    /// </summary>
    public Uri? ThemeSource
    {
        get => _currentThemeSource;
        set
        {
            if (value != null)
            {
                SetThemeSource(value);
                OnPropertyChanged();
            }
        }
    }
}