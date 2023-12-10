using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;


public class WindowsPowerCFG
{
    public int get_plugged_in_sleep_after_seconds()
    {
        return get_sleep_after_seconds(PowerReadACValue);
    }
    public int get_on_battery_sleep_after_seconds()
    {
        return get_sleep_after_seconds(PowerReadDCValue);
    }

    private int get_sleep_after_seconds(PowerReadValueFunction readValueFunction)
    {
        var activePolicyGuidPTR = IntPtr.Zero;
        PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

        var activePolicyGuid = Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);
        var type = 0;
        var value = 0;
        var valueSize = 4u;
        readValueFunction(IntPtr.Zero, ref activePolicyGuid,
            ref GUID_SLEEP_SUBGROUP, ref GUID_STANDBYIDLE,
            ref type, ref value, ref valueSize);

        return value;
    }

    [DllImport("powrprof.dll")]
    static extern uint PowerGetActiveScheme(
        IntPtr UserRootPowerKey,
        ref IntPtr ActivePolicyGuid);

    private delegate uint PowerReadValueFunction(
        IntPtr RootPowerKey,
        ref Guid SchemeGuid,
        ref Guid SubGroupOfPowerSettingGuid,
        ref Guid PowerSettingGuid,
        ref int Type,
        ref int Buffer,
        ref uint BufferSize);

    [DllImport("powrprof.dll")]
    static extern uint PowerReadACValue(
        IntPtr RootPowerKey,
        ref Guid SchemeGuid,
        ref Guid SubGroupOfPowerSettingGuid,
        ref Guid PowerSettingGuid,
        ref int Type,
        ref int Buffer,
        ref uint BufferSize);

    [DllImport("powrprof.dll")]
    static extern uint PowerReadDCValue(
        IntPtr RootPowerKey,
        ref Guid SchemeGuid,
        ref Guid SubGroupOfPowerSettingGuid,
        ref Guid PowerSettingGuid,
        ref int Type,
        ref int Buffer,
        ref uint BufferSize);

    private static Guid GUID_SLEEP_SUBGROUP =
        new Guid("238c9fa8-0aad-41ed-83f4-97be242c8f20");
    private static Guid GUID_STANDBYIDLE =
        new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");

}

public class NoSleep
{
    static void Main()
    {
        var powerCfg = new WindowsPowerCFG();
        Console.WriteLine("started NoSleep");
        Console.WriteLine($"when plugged in pc will sleep after {powerCfg.get_plugged_in_sleep_after_seconds()} seconds");
        Console.Write($"on battery power pc will sleep after {powerCfg.get_on_battery_sleep_after_seconds()} seconds");
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

