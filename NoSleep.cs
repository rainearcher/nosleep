using System;
using System.Windows.Forms;


public class NoSleep
{
    static void Main()
    {
        var icon = new NoSleep();
        Application.Run();
    }

    public NoSleep()
    {
        notifyIcon = new NotifyIcon();
        contextMenu = new ContextMenuStrip();
        menuItem = new ToolStripMenuItem();

        contextMenu.Items.AddRange(
            new ToolStripMenuItem[] {menuItem});

        menuItem.Text = "E&xit";
        menuItem.Click += new EventHandler(this.menu_item_click);

        notifyIcon.Icon = new System.Drawing.Icon("images/moon.ico");

        notifyIcon.ContextMenuStrip = contextMenu;

        notifyIcon.Text = "NoSleep";
        notifyIcon.Visible = true;
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

