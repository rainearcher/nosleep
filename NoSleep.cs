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

    public void set_plugged_in_sleep_after_seconds(int seconds)
    {
        set_sleep_after_seconds(seconds, PowerWriteACValueIndex);
    }

    public void set_on_battery_sleep_after_seconds(int seconds)
    {
        set_sleep_after_seconds(seconds, PowerWriteDCValueIndex);
    }

    private int get_sleep_after_seconds(PowerReadValueFunction readValueFunction)
    {
        var activePolicyGuid = get_active_scheme_guid();
        var type = 0;
        var value = 0;
        var valueSize = 4u;
        readValueFunction(IntPtr.Zero, ref activePolicyGuid,
            ref GUID_SLEEP_SUBGROUP, ref GUID_STANDBYIDLE,
            ref type, ref value, ref valueSize);

        return value;
    }

    private void set_sleep_after_seconds(int seconds, PowerWriteValueIndexFunction powerWriteValue)
    {
        var activePolicyGuid = get_active_scheme_guid();
        powerWriteValue(
            IntPtr.Zero,
            ref activePolicyGuid,
            ref GUID_SLEEP_SUBGROUP,
            ref GUID_STANDBYIDLE,
            seconds);
    }

    private Guid get_active_scheme_guid()
    {
        var activePolicyGuidPTR = IntPtr.Zero;
        PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

        return Marshal.PtrToStructure<Guid>(activePolicyGuidPTR);

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

    private delegate uint PowerWriteValueIndexFunction(
        IntPtr RootPowerKey,
        ref Guid SchemeGuid,
        ref Guid SubGroupOfPowerSettingsGuid,
        ref Guid PowerSettingGuid,
        int AcValueIndex);

    [DllImport("powrprof.dll")]
    static extern uint PowerWriteACValueIndex(
        IntPtr RootPowerKey,
        ref Guid SchemeGuid,
        ref Guid SubGroupOfPowerSettingsGuid,
        ref Guid PowerSettingGuid,
        int AcValueIndex);

    [DllImport("powrprof.dll")]
    static extern uint PowerWriteDCValueIndex(
        IntPtr RootPowerKey,
        ref Guid SchemeGuid,
        ref Guid SubGroupOfPowerSettingsGuid,
        ref Guid PowerSettingGuid,
        int AcValueIndex);

    private static Guid GUID_SLEEP_SUBGROUP =
        new Guid("238c9fa8-0aad-41ed-83f4-97be242c8f20");
    private static Guid GUID_STANDBYIDLE =
        new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");

}

public class NoSleep
{
    static void Main()
    {
        var noSleep = new NoSleep();
        noSleep.debug_read_powercfg();
        Application.Run();
    }

    public NoSleep()
    {
        init_exit_menu_item();
        init_sleep_on_off_sleep_menu_item();
        init_context_menu();
        init_notify_icon();
        init_powercfg();
    }

    public void debug_read_powercfg()
    {
        Console.WriteLine("started NoSleep");
        var oldPluggedInValue = powerCfg.get_plugged_in_sleep_after_seconds();
        var oldOnBatteryValue = powerCfg.get_on_battery_sleep_after_seconds();
        Console.WriteLine($"when plugged in pc will sleep after {oldPluggedInValue} seconds");
        Console.WriteLine($"on battery power pc will sleep after {oldOnBatteryValue} seconds");
        powerCfg.set_plugged_in_sleep_after_seconds(0);
        powerCfg.set_on_battery_sleep_after_seconds(240);
        Console.WriteLine($"when plugged in pc will sleep after {powerCfg.get_plugged_in_sleep_after_seconds()} seconds");
        Console.WriteLine($"on battery power pc will sleep after {powerCfg.get_on_battery_sleep_after_seconds()} seconds");
        powerCfg.set_plugged_in_sleep_after_seconds(oldPluggedInValue);
        powerCfg.set_on_battery_sleep_after_seconds(oldOnBatteryValue);
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
        powerCfg = new WindowsPowerCFG();
    }

    private void exit_menu_item_click(object Sender, EventArgs e)
    {
        notifyIcon.Dispose();
        Application.Exit();
    }

    private void sleep_on_off_menu_item_click(object Sender, EventArgs e)
    {
        powerCfg.set_on_battery_sleep_after_seconds(0);
        powerCfg.set_plugged_in_sleep_after_seconds(0);
        Console.WriteLine($"computer will now never sleep");
        sleepOnOffMenuItem.Text = "PC will stay awake.";
        notifyIcon.Text = "NoSleep: PC will stay awake.";
    }

    private WindowsPowerCFG powerCfg;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;
    private ToolStripMenuItem sleepOnOffMenuItem;
    private ToolStripMenuItem exitMenuItem;
}

