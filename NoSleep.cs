using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;


public class WindowsPowerCFG
{

    public WindowsPowerCFG()
    {
        userPluggedInSleepAfterSeconds = get_plugged_in_sleep_after_seconds();
        userOnBatterySleepAfterSeconds = get_on_battery_sleep_after_seconds();
    }

    public void disable_sleep()
    {
        set_on_battery_sleep_after_seconds(0);
        set_plugged_in_sleep_after_seconds(0);
    }

    public void reenable_sleep()
    {
        set_on_battery_sleep_after_seconds(userOnBatterySleepAfterSeconds);
        set_plugged_in_sleep_after_seconds(userPluggedInSleepAfterSeconds);
    }

    public void debug_read_powercfg()
    {
        Console.WriteLine($"when plugged in pc will sleep after {userPluggedInSleepAfterSeconds} seconds");
        Console.WriteLine($"on battery power pc will sleep after {userOnBatterySleepAfterSeconds} seconds");
        set_plugged_in_sleep_after_seconds(0);
        set_on_battery_sleep_after_seconds(240);
        Console.WriteLine($"when plugged in pc will sleep after {get_plugged_in_sleep_after_seconds()} seconds");
        Console.WriteLine($"on battery power pc will sleep after {get_on_battery_sleep_after_seconds()} seconds");
        set_plugged_in_sleep_after_seconds(userOnBatterySleepAfterSeconds);
        set_on_battery_sleep_after_seconds(userOnBatterySleepAfterSeconds);
    }

    private int get_plugged_in_sleep_after_seconds()
    {
        return get_sleep_after_seconds(PowerReadACValue);
    }
    private int get_on_battery_sleep_after_seconds()
    {
        return get_sleep_after_seconds(PowerReadDCValue);
    }

    private void set_plugged_in_sleep_after_seconds(int seconds)
    {
        set_sleep_after_seconds(seconds, PowerWriteACValueIndex);
    }

    private void set_on_battery_sleep_after_seconds(int seconds)
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

    private int userPluggedInSleepAfterSeconds;
    private int userOnBatterySleepAfterSeconds;

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
        Console.WriteLine("started NoSleep");
        var noSleep = new NoSleep();
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

    private WindowsPowerCFG powerCfg;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;
    private ToolStripMenuItem sleepOnOffMenuItem;
    private ToolStripMenuItem exitMenuItem;
    private bool allowedToSleep = true;
}

