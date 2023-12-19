using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace NoSleep;

public static class ConfigFile
{

    static ConfigFile()
    {
        if (!Directory.Exists(appDataFolderPath))
            Directory.CreateDirectory(appDataFolderPath);

        if (exists())
        {
            string text = File.ReadAllText(appDataFilePath);
            try
            {
                config = JsonSerializer.Deserialize<Dictionary<string, object>>(text);
            }
            catch (Exception e)
            {
                Console.WriteLine("broken config");
                remove_config();
            }
        }
    }

    public static void save_ac_dc_sleep_after_seconds(int acSeconds, int dcSeconds)
    {
        config["acSeconds"] = acSeconds;
        config["dcSeconds"] = dcSeconds;
        write();
    }

    public static void save_sleep_on_off(bool onOff)
    {
        config["onOff"] = onOff;
        write();
    }

    public static void read_ac_dc_sleep_after_seconds(out int acSeconds, out int dcSeconds)
    {
        if (exists())
        {
            acSeconds = int.Parse(config["acSeconds"].ToString());
            dcSeconds = int.Parse(config["dcSeconds"].ToString());
        }
        else
        {
            acSeconds = 600;
            dcSeconds = 240;
        }

    }

    public static bool read_sleep_on_off()
    {
        if (config.ContainsKey("onOff"))
            return Boolean.Parse(config["onOff"].ToString());
        return true;
    }

    public static bool exists()
    {
        return File.Exists(appDataFilePath);
    }

    private static void remove_config()
    {
        File.Delete(appDataFilePath);
    }

    private static void write()
    {
        string jsonString = JsonSerializer.Serialize(config);
        File.WriteAllText(appDataFilePath, jsonString);
    }

    private static Dictionary<string, object> config = new Dictionary<string, object>();
    static private string userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    static private string appDataFolderPath = Path.Combine(userPath, "NoSleep");
    static private string appDataFilePath= Path.Combine(appDataFolderPath, "config.json");

}
