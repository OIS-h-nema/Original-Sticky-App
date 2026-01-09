using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace StickyNoteApp;

public partial class App : System.Windows.Application
{
    private NotifyIcon? _notifyIcon;
    private MainWindow? _mainWindow;
    private AppSettings? _settings;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _settings = SettingsService.Load();
        _settings.ValidateAndFix();

        InitializeNotifyIcon();

        _mainWindow = new MainWindow(_settings);
        _mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (_settings is not null)
        {
            SettingsService.Save(_settings);
        }

        _notifyIcon?.Dispose();
        base.OnExit(e);
    }

    private void InitializeNotifyIcon()
    {
        var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "tray_icon.ico");
        Icon icon;
        try
        {
            icon = new Icon(iconPath);
        }
        catch
        {
            icon = SystemIcons.Application;
        }

        _notifyIcon = new NotifyIcon
        {
            Icon = icon,
            Text = "StickyNote",
            Visible = true,
            ContextMenuStrip = new ContextMenuStrip()
        };

        _notifyIcon.ContextMenuStrip.Items.Add("設定", null, OnSettingsClick);
        _notifyIcon.ContextMenuStrip.Items.Add("終了", null, OnExitClick);
        _notifyIcon.DoubleClick += (_, _) => _mainWindow?.Activate();
    }

    private void OnSettingsClick(object? sender, EventArgs e)
    {
        if (_settings is null || _mainWindow is null)
        {
            return;
        }

        var settingsWindow = new SettingsWindow(_settings)
        {
            Owner = _mainWindow
        };

        if (settingsWindow.ShowDialog() == true)
        {
            _settings.CopyFrom(settingsWindow.UpdatedSettings);
            _settings.ValidateAndFix();
            SettingsService.Save(_settings);
            _mainWindow.ApplySettings(_settings);
        }
    }

    private void OnExitClick(object? sender, EventArgs e)
    {
        if (_settings is not null)
        {
            SettingsService.Save(_settings);
        }

        _notifyIcon?.Dispose();
        Shutdown();
    }
}
