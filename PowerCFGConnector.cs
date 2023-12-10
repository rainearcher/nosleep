using System;
using System.IO;
using System.Runtime.InteropServices;
using NoSleep;

public class WindowsPowercfgConnector
{
    public WindowsPowercfgConnector()
    {
        if (!ConfigFile.exists())
        {
            userPluggedInSleepAfterSeconds = get_plugged_in_sleep_after_seconds();
            userOnBatterySleepAfterSeconds = get_on_battery_sleep_after_seconds();
            ConfigFile.save_ac_dc_sleep_after_seconds(userPluggedInSleepAfterSeconds, userOnBatterySleepAfterSeconds);

        }
        else
        {
            ConfigFile.read_ac_dc_sleep_after_seconds(out userPluggedInSleepAfterSeconds, out userOnBatterySleepAfterSeconds);
        }
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
        set_on_battery_sleep_after_seconds(0);
        Console.WriteLine($"when plugged in pc will sleep after {get_plugged_in_sleep_after_seconds()} seconds");
        Console.WriteLine($"on battery power pc will sleep after {get_on_battery_sleep_after_seconds()} seconds");
        set_plugged_in_sleep_after_seconds(userPluggedInSleepAfterSeconds);
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

    private int userPluggedInSleepAfterSeconds;
    private int userOnBatterySleepAfterSeconds;

}
