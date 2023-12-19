using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;

namespace RunOnStartup;

public static class Startup
{
    static string taskName = "NoSleep";

    public static void RunOnStartup()
    {
        if (IsScheduled())
        {
            UnSchedule();
        }
        Schedule();
    }

    public static void RemoveFromStartup()
    {
        UnSchedule();
    }

    public static bool IsScheduled()
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

    private static void Schedule()
    {

        string strExeFilePath = Application.ExecutablePath;

        if (strExeFilePath is null) return;

        var userId = WindowsIdentity.GetCurrent().Name;

        using (TaskDefinition td = TaskService.Instance.NewTask())
        {

            td.RegistrationInfo.Description = "G-Helper Auto Start";
            td.Triggers.Add(new LogonTrigger { UserId = userId, Delay = TimeSpan.FromSeconds(1) });
            td.Actions.Add(strExeFilePath);

            if (IsUserAdministrator())
                td.Principal.RunLevel = TaskRunLevel.Highest;

            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;

            Debug.WriteLine(strExeFilePath);
            Debug.WriteLine(userId);

            try
            {
                TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
            }
            catch (Exception e)
            {
                if (IsUserAdministrator())
                    MessageBox.Show("Can't create a start up task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);

            }
        }

    }

    public static void UnSchedule()
    {
        using (TaskService taskService = new TaskService())
        {
            try
            {
                taskService.RootFolder.DeleteTask(taskName);
            }
            catch (Exception e)
            {
                if (IsUserAdministrator())
                    MessageBox.Show("Can't remove task. Try running Task Scheduler by hand and manually deleting GHelper task if it exists there.", "Scheduler Error", MessageBoxButtons.OK);

            }
        }
    }
}
