# Adaptive Taskbar for Windows™ ![Adaptive Taskbar for Windows icon](https://raw.githubusercontent.com/cprcrack/AdaptiveTaskbar/master/icon64.ico)

Adaptive Taskbar is a lightweight app that automatically switches between big or small Windows taskbar depending on the current *main* screen size.

This is useful, for example, when you have a lot of pinned icons in the taskbar that fit in a big screen (i.e. your external screen) but do not fit in your small screen (i.e. your laptop's built-in screen) unless you set Windows' taskbar size to be small. In this case Adaptive Taskbar will automatically change this setting when you connect or disconnect your external screen, so you don't have to.

## Compatibility

Adaptive Taskbar has been developed and tested for Windows 10 (including Creators Update), but it should be compatible with Windows 7/8/10.

## Downloading and installing

**To install Adaptive Taskbar, please download and run the latest `Setup.exe` file from the [latest release](https://github.com/cprcrack/AdaptiveTaskbar/releases/latest).**

A dialog box will confirm the successful installation, and from that point the app will be running in the background. It will also automatically start on Windows startup just after you log in.

## Testing

If you want, you can test that Adaptive Taskbar is working by changing your *main* screen resolution to be smaller or equal-or-bigger than the thresold (by default 1920), assuming your Windows' DPI setting is set to 100%.

## Customizing the thresold

You can choose the resolution thresold for the screen to be considered big by editing the `AdaptiveTaskbar.exe.config` file located in `C:\YOUR_USER\AppData\Local\AdaptiveTaskbar\app-LAST_APP_VERSION\` after installation.

You will have to open the file with a text editor and modify the default value, 1920, to something else. The value represents the minimum horizontal resolution of the screen for it to be considered big.

Note that this resolution already takes into account Windows' DPI setting, so even if a small 13 inch screen's native resolution is 1920, if the DPI setting is set for example to 150% (which is a common thing), the actual resolution reported would be 1280, so the screen will be considered small without any modification.

## Uninstalling

As Adaptive Taskbar doesn't have any user interface, the only way to uninstall the app is via the "Apps and features" window from the Control Panel.

## License and contributing

Adaptive Taskbar is free and open source. The source code is released under the MIT License. Contributions are very welcome. Be sure to star this project on GitHub, [report bugs or suggest new ideas](https://github.com/cprcrack/AdaptiveTaskbar/issues/new).

## Legal

Windows is a trademark of Microsoft Corp., registered in the U.S. and other countries. Adaptive Taskbar is an independent project developed by Cristian Perez and has no relationship to Windows or Microsoft Corp.
