using System;
using System.Windows.Forms;


public class NoSleep
{
    static void Main()
    {
        new NoSleep();
        Application.Run();
    }

    public NoSleep()
    {
        init_items_in_order();
    }

    private void init_items_in_order()
    {
        init_menu_item();
        init_context_menu();
        init_notify_icon();
    }

    private void init_menu_item()
    {
        menuItem = new ToolStripMenuItem {
            Text = "E&xit"
        };
        menuItem.Click += new EventHandler(menu_item_click);

    }

    private void init_context_menu()
    {
        contextMenu = new ContextMenuStrip();
        contextMenu.Items.AddRange(
            new ToolStripMenuItem[] {menuItem});

    }

    private void init_notify_icon()
    {
        notifyIcon = new NotifyIcon
        {
            Icon = new System.Drawing.Icon("images/moon.ico"),
            ContextMenuStrip = contextMenu,
            Text = "NoSleep",
            Visible = true
        };
    }

    private void menu_item_click(object Sender, EventArgs e)
    {
        notifyIcon.Dispose();
        Application.Exit();
    }

    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;
    private ToolStripMenuItem menuItem;
}

