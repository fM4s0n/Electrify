﻿using Serilog;

namespace Electrify.AdminUi;

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
