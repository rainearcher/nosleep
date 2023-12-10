using System;
using System.IO;
using System.Runtime.InteropServices;
using NoSleep;

namespace NoSleep;

public class ConfigFile
{

    public static void save_ac_dc_sleep_after_seconds(int acSeconds, int dcSeconds)
    {
        if (!exists())
            create_config();
        string[] contents = File.ReadAllLines(appDataFilePath);
        contents[0] = acSeconds.ToString();
        contents[1] = dcSeconds.ToString();
        File.WriteAllLines(appDataFilePath, contents);
    }

    public static void save_sleep_on_off(bool onOff)
    {
        if (!exists())
            create_config();
        string[] contents = File.ReadAllLines(appDataFilePath);
        contents[2] = onOff.ToString();
        File.WriteAllLines(appDataFilePath, contents);
    }

    public static void read_ac_dc_sleep_after_seconds(out int acSeconds, out int dcSeconds)
    {
        string[] lines = File.ReadAllLines(appDataFilePath);
        acSeconds = int.Parse(lines[0]);
        dcSeconds = int.Parse(lines[1]);
    }

    public static bool read_sleep_on_off()
    {
        string[] contents = File.ReadAllLines(appDataFilePath);
        return Boolean.Parse(contents[2]);
    }

    public static bool exists()
    {
        return File.Exists(appDataFilePath);
    }

    private static void create_config()
    {
        string[] contents = { "", "", "" };
        File.WriteAllLines(appDataFilePath, contents);
    }

    static private string userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    static private string appDataFolderPath = Path.Combine(userPath, "NoSleep");
    static private string appDataFilePath= Path.Combine(appDataFolderPath, "NoSleep.cfg");

}
