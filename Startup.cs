using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;

namespace RunOnStartup;

public class Startup
{
    static string taskName = "NoSleep";

    public static bool RunOnStartup()
    {
        if (IsScheduled())
            return true;
        return Schedule();
    }

    public static bool RemoveFromStartup()
    {
        return UnSchedule();
    }


    private static bool IsScheduled()
    {
        using (TaskService taskService = new TaskService())
            return (taskService.RootFolder.AllTasks.Any(t => t.Name == taskName));
    }

    private static bool IsUserAdministrator()
    {
        WindowsIdentity identity = WindowsIdentity.GetCurrent();
        WindowsPrincipal principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private static bool Schedule()
    {
        string strExeFilePath = Application.ExecutablePath;

        if (strExeFilePath is null) return true;

        var userId = WindowsIdentity.GetCurrent().Name;

        using (TaskDefinition td = TaskService.Instance.NewTask())
        {
            td.RegistrationInfo.Description = "NoSleep Auto Start";
            td.Triggers.Add(new LogonTrigger { UserId = userId, Delay = TimeSpan.FromSeconds(1) });
            td.Actions.Add(strExeFilePath);

            Console.WriteLine(strExeFilePath);
            Console.Write(userId);

            try
            {
                TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("can't startup task");
                return false;
            }
        }
    }

    private static bool UnSchedule()
    {
        using (TaskService taskService = new TaskService())
        {
            try
            {
                taskService.RootFolder.DeleteTask(taskName);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("can't remove task");
                return false;
            }
        }
    }

    private static long lastAdmin;

    private static void RunAsAdmin(string? param = null)
    {

        if (Math.Abs(DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastAdmin) < 2000) return;
        lastAdmin = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        // Check if the current user is an administrator
        if (!IsUserAdministrator())
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Arguments = param;
            startInfo.Verb = "runas";
            try
            {
                Process.Start(startInfo);
                Application.Exit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
