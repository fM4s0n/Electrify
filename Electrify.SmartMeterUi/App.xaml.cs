﻿using Serilog;

namespace Electrify.SmartMeterUi;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }

    protected override void OnSleep()
    {
        Log.CloseAndFlush();
        base.OnSleep();
    }
}
