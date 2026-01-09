using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace StickyNoteApp;

public partial class SettingsWindow : Window
{
    private static readonly Regex HexColorPattern = new("^#([0-9a-fA-F]{6})$");
    private readonly AppSettings _workingCopy;

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();
        _workingCopy = settings.Clone();
        LoadSettings();
    }

    public AppSettings UpdatedSettings => _workingCopy;

    private void LoadSettings()
    {
        WidthBox.Text = _workingCopy.Width.ToString(CultureInfo.InvariantCulture);
        HeightBox.Text = _workingCopy.Height.ToString(CultureInfo.InvariantCulture);

        FontFamilyBox.ItemsSource = Fonts.SystemFontFamilies;
        FontFamilyBox.SelectedItem = Fonts.SystemFontFamilies.FirstOrDefault(f => f.Source == _workingCopy.FontFamily);
        if (FontFamilyBox.SelectedItem is null)
        {
            _workingCopy.FontFamily = Constants.DefaultFontFamily;
            FontFamilyBox.SelectedItem = Fonts.SystemFontFamilies.FirstOrDefault(
                f => f.Source == _workingCopy.FontFamily);
        }

        FontSizeBox.Text = _workingCopy.FontSize.ToString(CultureInfo.InvariantCulture);
        FontColorBox.Text = _workingCopy.FontColor;
        UpdateColorPreview(_workingCopy.FontColor);

        IsLockedCheck.IsChecked = _workingCopy.IsLocked;
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        _workingCopy.Width = ParseDouble(WidthBox.Text, Constants.DefaultWidth);
        _workingCopy.Height = ParseDouble(HeightBox.Text, Constants.DefaultHeight);
        _workingCopy.Width = Math.Max(_workingCopy.Width, Constants.MinWidth);
        _workingCopy.Height = Math.Max(_workingCopy.Height, Constants.MinHeight);

        if (FontFamilyBox.SelectedItem is FontFamily selectedFamily)
        {
            _workingCopy.FontFamily = selectedFamily.Source;
        }
        else
        {
            _workingCopy.FontFamily = Constants.DefaultFontFamily;
        }

        _workingCopy.FontSize = Math.Max(ParseDouble(FontSizeBox.Text, Constants.DefaultFontSize), 1);

        var colorValue = FontColorBox.Text.Trim();
        if (HexColorPattern.IsMatch(colorValue))
        {
            _workingCopy.FontColor = colorValue;
        }
        else
        {
            _workingCopy.FontColor = Constants.DefaultFontColor;
        }

        _workingCopy.IsLocked = IsLockedCheck.IsChecked == true;

        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void OnFontColorChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        UpdateColorPreview(FontColorBox.Text.Trim());
    }

    private static double ParseDouble(string value, double fallback)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : fallback;
    }

    private void UpdateColorPreview(string hexColor)
    {
        try
        {
            var brush = (Brush)new BrushConverter().ConvertFromString(hexColor)!;
            ColorPreview.Fill = brush;
        }
        catch
        {
            ColorPreview.Fill = Brushes.Transparent;
        }
    }
}
