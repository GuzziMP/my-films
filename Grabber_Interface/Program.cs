using System;
using System.Windows.Forms;

namespace Grabber_Interface
{
  //using NLog;
  //using NLog.Config;
  //using NLog.Targets;

  static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // InitLogger();
            Application.Run(new GrabConfig(args));
        }

//        private static void InitLogger()
//        {
//          LoggingConfiguration config = LogManager.Configuration ?? new LoggingConfiguration();
//          string LogFileName = "MyFilmsGrabberInterface.log";
//          string OldLogFileName = "MyFilmsGrabberInterface-old.log";

//          try
//          {
//            FileInfo logFile = new FileInfo(Config.GetFile(Config.Dir.Log, LogFileName));
//            if (logFile.Exists)
//            {
//              if (File.Exists(Config.GetFile(Config.Dir.Log, OldLogFileName)))
//                File.Delete(Config.GetFile(Config.Dir.Log, OldLogFileName));

//              logFile.CopyTo(Config.GetFile(Config.Dir.Log, OldLogFileName));
//              logFile.Delete();
//            }
//          }
//          catch (Exception) { }

//          FileTarget fileTarget = new FileTarget();
//          fileTarget.FileName = Config.GetFile(Config.Dir.Log, LogFileName);
//          fileTarget.Layout = "${date:format=dd-MMM-yyyy HH\\:mm\\:ss,fff} " + "${level:fixedLength=true:padding=5} " +
//                              "[${logger:fixedLength=true:padding=20:shortName=true}]: ${message} " +
//                              "${exception:format=tostring}";
//          //"${qpc}";
//          //${qpc:normalize=Boolean:difference=Boolean:alignDecimalPoint=Boolean:precision=Integer:seconds=Boolean}

//          config.AddTarget("file", fileTarget);

//          // Get current Log Level from MediaPortal 
//          NLog.LogLevel logLevel;
//          MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"));

//          switch ((Level)xmlreader.GetValueAsInt("general", "loglevel", 0))
//          {
//            case Level.Error:
//              logLevel = LogLevel.Error;
//              break;
//            case Level.Warning:
//              logLevel = LogLevel.Warn;
//              break;
//            case Level.Information:
//              logLevel = LogLevel.Info;
//              break;
//            case Level.Debug:
//            default:
//              logLevel = LogLevel.Debug;
//              break;
//          }

//#if DEBUG
//          logLevel = LogLevel.Debug;
//#endif

//          LoggingRule rule = new LoggingRule("*", logLevel, fileTarget);
//          config.LoggingRules.Add(rule);

//          LogManager.Configuration = config;
//        }
    }
}