using System;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using CoolProject;
using System.Diagnostics;
using CheckingPaymentService;
using System.Collections.Generic;

static class Program
{
    static void Main(string[] args)
    {
        var commands = Utils.ParseArguments(args);

        // Restart program and run as admin(Required to install/remove/stop/start the service)
        if (!Utils.IsRunAsAdmin() && (commands.ContainsKey("-install") || commands.ContainsKey("-uninstall") || commands.ContainsKey("-start") || commands.ContainsKey("-stop")))
        {
            LogHelper.WriteEvent("Restart program and run as admin.", EventType.Info);
            
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName)
            {
                Verb = "runas",
                Arguments = string.Join(" ", args)
            };
            try
            {
                Process.Start(startInfo);
            }
            catch
            {
                LogHelper.WriteEvent("This operation requires administrative rights. Please run the application as an administrator.", EventType.Error);
            }
            return;
        }

        if (commands.Count > 0)
        {
            SetCommands(commands);
            return;
        }
        else
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CurrencyRatesService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }


    //Execution of commands passed as arguments
    private static void SetCommands(Dictionary<string, string> commands)
    {
        foreach (var command in commands)
        {
            switch (command.Key.ToLower())
            {
                case "-install":
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        LogHelper.WriteEvent("The service is successfully installed!", EventType.Info);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteEvent($"An error occurred while installing the service: {ex.Message}", EventType.Error);
                    }

                    break;
                case "-uninstall":
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        LogHelper.WriteEvent("The service is successfully uninstalled!", EventType.Info);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteEvent($"An error occurred while uninstalling the service: {ex.Message}", EventType.Error);
                    }
                    break;
                case "-start":
                    StartService("CurrencyRatesService");
                    break;
                case "-stop":
                    StopService("CurrencyRatesService");
                    break;
                case "-out":
                    if (command.Value != null)
                    {
                        CurrencyRatesService.UpdateParameter("Settings", "filePath", command.Value);
                    }
                    else
                    {
                        LogHelper.WriteEvent("Usage: -out <filePath>. There are no value.", EventType.Error);
                    }
                    break;
                case "-fmt":
                    if (command.Value != null)
                    {
                        CurrencyRatesService.UpdateParameter("Settings", "fileType", command.Value.ToLower());
                    }
                    else
                    {
                        LogHelper.WriteEvent("Usage: -fmt <fileType>. There are no value.", EventType.Error);
                    }
                    break;
                case "-interval":
                    if (command.Value != null)
                    {
                        CurrencyRatesService.UpdateParameter("Settings", "execInterval", command.Value);
                    }
                    else
                    {
                        LogHelper.WriteEvent("Usage: -interval <execInterval>. There are no value.", EventType.Error);
                    }
                    break;
                case "-currency":
                    if (command.Value != null)
                    {
                        CurrencyRatesService.UpdateParameter("Settings", "valcode", command.Value);
                    }
                    else
                    {
                        LogHelper.WriteEvent("Usage: -currency <valcode>. There are no value.", EventType.Error);
                    }
                    break;
                default:
                    Console.WriteLine("Invalid argument. Use -install, -uninstall, -start, -stop, -out, -interval or -fmt.");
                    break;
            }
        }
    }


    private static void StartService(string serviceName)
    {
        ServiceController service = new ServiceController(serviceName);
        try
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(10000);

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);

            LogHelper.WriteEvent("The service is running!", EventType.Info);
        }
        catch (Exception ex)
        {
            LogHelper.WriteEvent($"Error starting service: {ex.Message}", EventType.Error);
            Console.WriteLine();
        }
    }

    private static void StopService(string serviceName)
    {
        ServiceController service = new ServiceController(serviceName);
        try
        {
            TimeSpan timeout = TimeSpan.FromMilliseconds(10000);

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

            LogHelper.WriteEvent("The service is stopped!", EventType.Info);
        }
        catch (Exception ex)
        {
            LogHelper.WriteEvent($"Error stopping service: {ex.Message}", EventType.Error);
        }
    }
}
