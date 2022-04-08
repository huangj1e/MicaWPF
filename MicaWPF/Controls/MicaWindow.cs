﻿namespace MicaWPF.Controls;

/// <summary>
/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:MicaWPF.Controls"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:MicaWPF.Controls;assembly=MicaWPF.Controls"
///
/// You will also need to add a project reference from the project where the XAML file lives
/// to this project and Rebuild to avoid compilation errors:
///
///     Right click on the target project in the Solution Explorer and
///     "Add Reference"->"Projects"->[Browse to and select this project]
///
///
/// Step 2)
/// Go ahead and use your control in the XAML file.
///
///     <MyNamespace:MicaWindow/>
///
/// </summary>
public class MicaWindow : Window
{
    private readonly DynamicThemeService _dynamicThemeService;

    public static readonly DependencyProperty AccentProperty = DependencyProperty.Register(
        "Accent", typeof(SolidColorBrush),
        typeof(MicaWindow)
        );

    public bool IsThemeAware { get; set; } = true;
    public bool IsWaitingForManualThemeChange { get; set; } = false;
    public bool UseWindowsAccentColor { get; set; } = true;
    public WindowsTheme Theme { get; set; } = WindowsTheme.Auto;
    public BackdropType SystemBackdropType { get; set; } = BackdropType.Mica;
    public int CaptionHeight { get; set; } = 20;

    public SolidColorBrush? Accent
    {
        get => (SolidColorBrush)GetValue(AccentProperty);
        set => SetValue(AccentProperty, value);
    }

    static MicaWindow()
    {
        if (OsHelper.GlobalOsVersion is not OsVersion.Windows11Before22523 and not OsVersion.Windows11After22523)
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MicaWindow), new FrameworkPropertyMetadata(typeof(MicaWindow)));
        }
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property.Name is nameof(IsThemeAware))
        {
            _dynamicThemeService.SetThemeAware(IsThemeAware, SystemBackdropType);
        }

        if (e.Property.Name is nameof(IsWaitingForManualThemeChange))
        {
            _dynamicThemeService.AwaitManualThemeChange(IsWaitingForManualThemeChange, SystemBackdropType);
        }

        if (e.Property.Name is nameof(Theme) or nameof(SystemBackdropType) or nameof(CaptionHeight))
        {
            this.EnableMica(Theme, SystemBackdropType, CaptionHeight);
            ThemeHelper.SetThemeBrushes(this, Theme, UseWindowsAccentColor);
            if (e.Property.Name is nameof(SystemBackdropType))
            {
                _dynamicThemeService.SetThemeAware(false);
                _dynamicThemeService.AwaitManualThemeChange(false);

                _dynamicThemeService.SetThemeAware(IsThemeAware, SystemBackdropType);
                _dynamicThemeService.AwaitManualThemeChange(IsWaitingForManualThemeChange, SystemBackdropType);
            }
        }

        if (e.Property.Name is nameof(Accent) or nameof(Background) or nameof(Foreground) or nameof(UseWindowsAccentColor)) 
        {
            ThemeHelper.SetThemeBrushes(this, Theme, UseWindowsAccentColor);
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        Loaded += MicaWindow_Loaded;
    }

    public void SetDefaultColor()
    {
        if (Accent is null)
        {
            Accent = DefaultColorHelper.GetThemedColor(Theme, "Accent");
        }

        if (Background is null)
        {
            Background = DefaultColorHelper.GetThemedColor(Theme, "Background");
        }

        if (Foreground is null || ((SolidColorBrush)Foreground).Color == Colors.Black)
        {
            Foreground = DefaultColorHelper.GetThemedColor(Theme, "Foreground");
        }
    }

    private void MicaWindow_Loaded(object sender, RoutedEventArgs e)
    {
        this.EnableMica(Theme, SystemBackdropType, CaptionHeight);
        SetDefaultColor();
        ThemeHelper.SetThemeBrushes(this, Theme, UseWindowsAccentColor);
        _dynamicThemeService.SetThemeAware(IsThemeAware, SystemBackdropType);
        _dynamicThemeService.AwaitManualThemeChange(IsWaitingForManualThemeChange, SystemBackdropType);
    }

    public MicaWindow()
    {
        _dynamicThemeService = new(this);
        CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
        CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
    }

    private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode is ResizeMode.CanResize or ResizeMode.CanResizeWithGrip;
    }

    private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ResizeMode != ResizeMode.NoResize;
    }

    private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MaximizeWindow(this);
    }

    private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
    {
        SystemCommands.RestoreWindow(this);
    }
}
