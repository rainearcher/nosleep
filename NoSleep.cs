using System;
using System.Windows.Forms;

public class NoSleepIcon
{
    static void Main()
    {
        Console.WriteLine("started NoSleep");
        var noSleep = new NoSleepIcon();
        Application.Run();
    }

    public NoSleepIcon()
    {
        init_exit_menu_item();
        init_sleep_on_off_sleep_menu_item();
        init_context_menu();
        init_notify_icon();
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
        sleepOnOffMenuItem = new ToolStripMenuItem {
            Text = "Keep PC Awake!"
        };
        sleepOnOffMenuItem.Click += new EventHandler(sleep_on_off_menu_item_click);

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
            Text = "NoSleep: PC is allowed to sleep.",
            Visible = true
        };
    }

    private void init_powercfg()
    {
        powerCfg = new WindowsPowercfgConnector();
        powerCfg.debug_read_powercfg();
    }

    private void exit_menu_item_click(object Sender, EventArgs e)
    {
        notifyIcon.Dispose();
        Application.Exit();
    }

    private void sleep_on_off_menu_item_click(object Sender, EventArgs e)
    {
        if (allowedToSleep)
        {
            allowedToSleep = false;
            powerCfg.disable_sleep();
            Console.WriteLine($"computer will now never sleep");
            sleepOnOffMenuItem.Text = "Allow PC to sleep.";
            notifyIcon.Text = "NoSleep: PC will stay awake.";
        }
        else
        {
            allowedToSleep = true;
            powerCfg.reenable_sleep();
            Console.WriteLine("computer is allowed to sleep");
            sleepOnOffMenuItem.Text = "Keep PC Awake!";
            notifyIcon.Text = "NoSleep: PC is allowed to sleep.";
        }
    }

    private WindowsPowercfgConnector powerCfg;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;
    private ToolStripMenuItem sleepOnOffMenuItem;
    private ToolStripMenuItem exitMenuItem;
    private bool allowedToSleep = true;
}

