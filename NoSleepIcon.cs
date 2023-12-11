using System;
using System.Windows.Forms;

namespace NoSleep;
public class NoSleepIcon
{
    public NoSleepIcon(NoSleepApp parent)
    {
        this.parent = parent;
        init_exit_menu_item();
        init_sleep_on_off_sleep_menu_item();
        init_context_menu();
        init_notify_icon();
    }

    ~NoSleepIcon()
    {
        notifyIcon.Dispose();
    }

    public void set_stay_awake()
    {
        notifyIcon.Text = "NoSleep: PC will stay awake.";
        sleepOnOffMenuItem.Text = "Allow PC to sleep.";
    }

    public void set_allow_to_sleep()
    {
        notifyIcon.Text = "NoSleep: PC is allowed to sleep.";
        sleepOnOffMenuItem.Text = "Keep PC Awake!";
    }

    private void init_exit_menu_item()
    {
        exitMenuItem = new ToolStripMenuItem
        {
            Text = "E&xit"
        };
        exitMenuItem.Click += new EventHandler(exit_menu_item_click);

    }

    private void init_sleep_on_off_sleep_menu_item()
    {
        sleepOnOffMenuItem = new ToolStripMenuItem();
        sleepOnOffMenuItem.Click += new EventHandler(sleep_on_off_menu_item_click);

    }

    private void init_context_menu()
    {
        contextMenu = new ContextMenuStrip();
        contextMenu.Items.AddRange(
            new ToolStripMenuItem[] { sleepOnOffMenuItem, exitMenuItem });

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

    private void exit_menu_item_click(object Sender, EventArgs e)
    {
        parent.exit();
    }

    private void sleep_on_off_menu_item_click(object Sender, EventArgs e)
    {
        parent.sleep_on_off_click();
    }

    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;
    private ToolStripMenuItem sleepOnOffMenuItem;
    private ToolStripMenuItem exitMenuItem;
    private NoSleepApp parent;
}

