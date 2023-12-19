using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RunOnStartup;

namespace NoSleep;
public class NoSleepApp
{
    static void Main()
    {
        Startup.RunOnStartup();

        if (hideConsole)
        {
            hide_console();
        }
        Console.WriteLine("started NoSleep");
        new NoSleepApp();
        Application.Run();
    }

    public NoSleepApp()
    {
        init_allowed_to_sleep();
        init_powercfg();
        init_icon();
    }
    
    private static void hide_console()
    {
        var handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);
    }

    public void sleep_on_off_click()
    {
        if (allowedToSleep)
        {
            disable_sleep();
        }
        else
        {
            enable_sleep();
        }
    }
    
    private void disable_sleep()
    {
        allowedToSleep = false;
        powerCfg.disable_sleep();
        icon.set_stay_awake();
        ConfigFile.save_sleep_on_off(false);
    }

    private void enable_sleep()
    {
        allowedToSleep = true;
        powerCfg.reenable_sleep();
        icon.set_allow_to_sleep();
        ConfigFile.save_sleep_on_off(true);
    }

    public void exit()
    {
        Startup.RemoveFromStartup();
        powerCfg.reenable_sleep();
        Application.Exit();
    }

    private void init_allowed_to_sleep()
    {
        allowedToSleep = ConfigFile.read_sleep_on_off();
    }

    private void init_powercfg()
    {
        powerCfg = new WindowsPowercfgConnector();
        if (allowedToSleep)
            powerCfg.reenable_sleep();
        else
            powerCfg.disable_sleep();
    }

    private void init_icon()
    {
        icon = new NoSleepIcon(this);
        if (allowedToSleep)
            icon.set_allow_to_sleep();
        else
            icon.set_stay_awake();
    }

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;

    private NoSleepIcon icon;
    private WindowsPowercfgConnector powerCfg;
    private bool allowedToSleep = true;
    private static bool hideConsole = true;
}
