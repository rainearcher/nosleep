using System;
using System.Windows.Forms;
using RunOnStartup;

namespace NoSleep;
public class NoSleepApp
{
    static void Main()
    {
        if (!Startup.RunOnStartup())
        {
            Environment.Exit(1);
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
    }

    private void enable_sleep()
    {
        allowedToSleep = true;
        powerCfg.reenable_sleep();
        icon.set_allow_to_sleep();
    }

    public void exit()
    {
        Startup.RemoveFromStartup();
        powerCfg.reenable_sleep();
        Application.Exit();
    }

    private void init_allowed_to_sleep()
    {
        if (ConfigFile.exists())
            allowedToSleep = ConfigFile.read_sleep_on_off();
        else
            allowedToSleep = true;
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

    private NoSleepIcon icon;
    private WindowsPowercfgConnector powerCfg;
    private bool allowedToSleep = true;
}
