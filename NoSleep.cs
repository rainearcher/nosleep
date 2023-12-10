using System;
using System.Windows.Forms;
using RunOnStartup;

namespace NoSleep;
public class NoSleepIcon
{
    static void Main()
    {
        if (!Startup.RunOnStartup())
        {
            Environment.Exit(1);
        }
        Console.WriteLine("started NoSleep");
        var noSleep = new NoSleepIcon();
        Application.Run();
    }

    public NoSleepIcon()
    {
        if (ConfigFile.exists())
            allowedToSleep = ConfigFile.read_sleep_on_off();
        init_exit_menu_item();
        init_sleep_on_off_sleep_menu_item();
        init_context_menu();
        init_notify_icon();
        update_icon_text();
        init_powercfg();
    }

    private void init_exit_menu_item()
    {
        exitMenuItem = new ToolStripMenuItem {
            Text = "E&xit"
        };
        exitMenuItem.Click += new EventHandler(exit_menu_item_click);

    }

    private void init_sleep_on_off_sleep_menu_item()
    {
        sleepOnOffMenuItem = new ToolStripMenuItem();
        sleepOnOffMenuItem.Click += new EventHandler(sleep_on_off_menu_item_click);

    }

    private void update_icon_text()
    {
        if (allowedToSleep)
        {
            notifyIcon.Text = "NoSleep: PC is allowed to sleep.";
            sleepOnOffMenuItem.Text = "Keep PC Awake!";
        }
        else
        {
            notifyIcon.Text = "NoSleep: PC will stay awake.";
            sleepOnOffMenuItem.Text = "Allow PC to sleep.";
        }
    }

    private void init_context_menu()
    {
        contextMenu = new ContextMenuStrip();
        contextMenu.Items.AddRange(
            new ToolStripMenuItem[] {sleepOnOffMenuItem, exitMenuItem});

    }

    private void init_notify_icon()
    {
        notifyIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("images/moon.ico"),
            ContextMenuStrip = contextMenu,
            Visible = true
        };
    }

    private void init_powercfg()
    {
        powerCfg = new WindowsPowercfgConnector();
        if (allowedToSleep)
            powerCfg.reenable_sleep();
        else
            powerCfg.disable_sleep();
        powerCfg.debug_read_powercfg();
    }

    private void exit_menu_item_click(object Sender, EventArgs e)
    {
        notifyIcon.Dispose();
        Application.Exit();
        Startup.RemoveFromStartup();
        powerCfg.reenable_sleep();
    }

    private void sleep_on_off_menu_item_click(object Sender, EventArgs e)
    {
        if (allowedToSleep)
        {
            allowedToSleep = false;
            powerCfg.disable_sleep();
        }
        else
        {
            allowedToSleep = true;
            powerCfg.reenable_sleep();
        }
        update_icon_text();
        ConfigFile.save_sleep_on_off(allowedToSleep);
    }

    private WindowsPowercfgConnector powerCfg;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;
    private ToolStripMenuItem sleepOnOffMenuItem;
    private ToolStripMenuItem exitMenuItem;
    private bool allowedToSleep = true;
}

