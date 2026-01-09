using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace StickyNoteApp;

public class AppSettings : INotifyPropertyChanged
{
    private static readonly Regex HexColorPattern = new("^#([0-9a-fA-F]{6})$");
    public event PropertyChangedEventHandler? PropertyChanged;

    private double _x = 100;
    private double _y = 100;
    private double _width = Constants.DefaultWidth;
    private double _height = Constants.DefaultHeight;
    private bool _isLocked;

    private string _content = string.Empty;
    private string _fontFamily = Constants.DefaultFontFamily;
    private double _fontSize = Constants.DefaultFontSize;
    private string _fontColor = Constants.DefaultFontColor;

    public double X
    {
        get => _x;
        set => SetField(ref _x, value);
    }

    public double Y
    {
        get => _y;
        set => SetField(ref _y, value);
    }

    public double Width
    {
        get => _width;
        set => SetField(ref _width, value);
    }

    public double Height
    {
        get => _height;
        set => SetField(ref _height, value);
    }

    public bool IsLocked
    {
        get => _isLocked;
        set => SetField(ref _isLocked, value);
    }

    public string Content
    {
        get => _content;
        set => SetField(ref _content, value);
    }

    public string FontFamily
    {
        get => _fontFamily;
        set => SetField(ref _fontFamily, value);
    }

    public double FontSize
    {
        get => _fontSize;
        set => SetField(ref _fontSize, value);
    }

    public string FontColor
    {
        get => _fontColor;
        set => SetField(ref _fontColor, value);
    }

    public void ValidateAndFix()
    {
        Content ??= string.Empty;
        FontFamily ??= Constants.DefaultFontFamily;
        FontColor ??= Constants.DefaultFontColor;

        if (!HexColorPattern.IsMatch(FontColor))
        {
            FontColor = Constants.DefaultFontColor;
        }

        if (FontSize < 1)
        {
            FontSize = Constants.DefaultFontSize;
        }

        Width = Math.Max(Width, Constants.MinWidth);
        Height = Math.Max(Height, Constants.MinHeight);
    }

    public AppSettings Clone()
    {
        return new AppSettings
        {
            X = X,
            Y = Y,
            Width = Width,
            Height = Height,
            IsLocked = IsLocked,
            Content = Content,
            FontFamily = FontFamily,
            FontSize = FontSize,
            FontColor = FontColor
        };
    }

    public void CopyFrom(AppSettings source)
    {
        X = source.X;
        Y = source.Y;
        Width = source.Width;
        Height = source.Height;
        IsLocked = source.IsLocked;
        Content = source.Content;
        FontFamily = source.FontFamily;
        FontSize = source.FontSize;
        FontColor = source.FontColor;
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(name);
    }
}
