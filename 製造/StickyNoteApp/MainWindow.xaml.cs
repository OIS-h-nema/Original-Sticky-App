using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Media = System.Windows.Media;

namespace StickyNoteApp;

public partial class MainWindow : Window
{
    private readonly AppSettings _settings;
    private DateTime _lastClickTime = DateTime.MinValue;
    private const int DoubleClickThreshold = 300;

    public MainWindow(AppSettings settings)
    {
        InitializeComponent();
        _settings = settings;
        ApplySettings(_settings);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplyWindowSettings();
        RenderMarkdown();
    }

    public void ApplySettings(AppSettings settings)
    {
        _settings.CopyFrom(settings);
        ApplyWindowSettings();
        ApplyFontSettings();
        RenderMarkdown();
    }

    private void ApplyWindowSettings()
    {
        Left = _settings.X;
        Top = _settings.Y;
        Width = _settings.Width;
        Height = _settings.Height;
        ClampToWorkArea();
    }

    private void ApplyFontSettings()
    {
        Media.FontFamily fontFamily;
        try
        {
            fontFamily = new Media.FontFamily(_settings.FontFamily);
        }
        catch
        {
            fontFamily = new Media.FontFamily(Constants.DefaultFontFamily);
        }

        var fontSize = _settings.FontSize < 1 ? Constants.DefaultFontSize : _settings.FontSize;
        var fontColor = TryGetBrush(_settings.FontColor) ?? TryGetBrush(Constants.DefaultFontColor) ?? Media.Brushes.Black;

        ViewText.FontFamily = fontFamily;
        ViewText.FontSize = fontSize;
        ViewText.Foreground = fontColor;

        EditText.FontFamily = fontFamily;
        EditText.FontSize = fontSize;
        EditText.Foreground = fontColor;
    }

    private void OnViewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2 ||
            (DateTime.Now - _lastClickTime).TotalMilliseconds < DoubleClickThreshold)
        {
            var clickPoint = e.GetPosition(ViewText);
            var editPoint = ViewText.TranslatePoint(clickPoint, EditText);
            SwitchToEditMode(editPoint);
            e.Handled = true;
            return;
        }

        _lastClickTime = DateTime.Now;
        if (!_settings.IsLocked)
        {
            DragMove();
            UpdatePosition();
            _ = SettingsService.SaveAsync(_settings);
            e.Handled = true;
        }
    }

    private void OnEditLostFocus(object sender, RoutedEventArgs e)
    {
        SaveAndSwitchToViewMode();
    }

    private void OnEditKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            EditText.Text = _settings.Content;
            SwitchToViewMode();
        }
    }

    private void SwitchToEditMode(System.Windows.Point? caretPoint = null)
    {
        ViewScroller.Visibility = Visibility.Collapsed;
        EditScroller.Visibility = Visibility.Visible;
        EditText.Text = _settings.Content;
        EditText.Focus();
        if (caretPoint.HasValue)
        {
            var index = EditText.GetCharacterIndexFromPoint(caretPoint.Value, true);
            EditText.CaretIndex = index >= 0 ? index : EditText.Text.Length;
        }
        else
        {
            EditText.CaretIndex = EditText.Text.Length;
        }
    }

    private void SaveAndSwitchToViewMode()
    {
        _settings.Content = EditText.Text;
        RenderMarkdown();
        SwitchToViewMode();
        _ = SettingsService.SaveAsync(_settings);
    }

    private void SwitchToViewMode()
    {
        EditScroller.Visibility = Visibility.Collapsed;
        ViewScroller.Visibility = Visibility.Visible;
    }

    private void RenderMarkdown()
    {
        ViewText.Inlines.Clear();
        var fontFamily = ViewText.FontFamily;
        var fontSize = ViewText.FontSize;
        var fontColor = ViewText.Foreground;

        var inlines = MarkdownRenderer.Render(_settings.Content, fontFamily, fontSize, fontColor).ToList();
        foreach (var inline in inlines)
        {
            ViewText.Inlines.Add(inline);
        }
    }

    private void UpdatePosition()
    {
        ClampToWorkArea();
        _settings.X = Left;
        _settings.Y = Top;
    }

    private static Media.Brush? TryGetBrush(string colorValue)
    {
        try
        {
            return (Media.Brush)new Media.BrushConverter().ConvertFromString(colorValue)!;
        }
        catch
        {
            return null;
        }
    }

    private void OnWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Handled || EditScroller.Visibility == Visibility.Visible || _settings.IsLocked)
        {
            return;
        }

        DragMove();
        UpdatePosition();
        _ = SettingsService.SaveAsync(_settings);
    }

    private void ClampToWorkArea()
    {
        var workArea = SystemParameters.WorkArea;
        var maxLeft = Math.Max(workArea.Left, workArea.Right - Width);
        var maxTop = Math.Max(workArea.Top, workArea.Bottom - Height);

        Left = Math.Max(workArea.Left, Math.Min(Left, maxLeft));
        Top = Math.Max(workArea.Top, Math.Min(Top, maxTop));
    }
}
