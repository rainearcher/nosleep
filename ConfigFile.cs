using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NoSleep;

public class ConfigFile
{

    public static void save_ac_dc_sleep_after_seconds(int acSeconds, int dcSeconds)
    {
        string acString = $"USER_AC_SLEEP_AFTER_INDEX {acSeconds}";
        string dcString = $"USER_DC_SLEEP_AFTER_INDEX {dcSeconds}";
        string[] dataFileContents = {acString, dcString};
        File.WriteAllLines(appDataFilePath, dataFileContents);
    }

    public static void read_ac_dc_sleep_after_seconds(out int acSeconds, out int dcSeconds)
    {
        using (StreamReader dataFile = new StreamReader(appDataFilePath))
        {
            string acLine = dataFile.ReadLine();
            string dcLine = dataFile.ReadLine();

            string acSleepAfterIndex = acLine.Split(' ')[1];
            string dcSleepAfterIndex = dcLine.Split(' ')[1];

            acSeconds = int.Parse(acSleepAfterIndex);
            dcSeconds = int.Parse(dcSleepAfterIndex);
        }
    }

    public static bool exists()
    {
        return File.Exists(appDataFilePath);
    }

    static private string userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    static private string appDataFolderPath = Path.Combine(userPath, "NoSleep");
    static private string appDataFilePath= Path.Combine(appDataFolderPath, "NoSleep.cfg");

}
