using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OzekiDemo.Utility
{
    public class ApplicationLog
    {
        public static bool CreateEventLog()
        {
            bool iSuccess = false;
            try
            {
                if (EventLog.SourceExists("RelianceOzeki"))
                {
                    iSuccess = false;
                }
                else
                {
                    EventLog.CreateEventSource("RelianceOzeki", "RelianceOzekiLog");
                    EventLog iLog = GetEventLog();
                    iLog.MaximumKilobytes = 2048;
                    iLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 7);
                    iSuccess = true;
                }
            }
            catch (Exception ex)
            {
                iSuccess = false;
                WriteEventToApplication(ex.ToString());
            }

            return iSuccess;
        }

        private static EventLog GetEventLog()
        {
            EventLog myLog = new EventLog();
            myLog.Source = "RelianceOzeki";
            return (myLog);
        }

        public static void WriteErrorToLog(System.Exception Exception)
        {
            try
            {
                EventLog iLog = GetEventLog();
                iLog.WriteEntry(Exception.ToString(), EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                WriteEventToApplication(ex.ToString());
            }
        }

        public static void WriteErrorToLog(string ErrorMessage)
        {
            try
            {
                EventLog iLog = GetEventLog();
                iLog.WriteEntry(ErrorMessage, EventLogEntryType.Error);
            }
            catch (Exception ex)
            {
                WriteEventToApplication(ex.ToString());
            }
        }

        public static void WriteEventToLog(string Message)
        {
            try
            {
                EventLog iLog = GetEventLog();
                iLog.WriteEntry(Message, EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                WriteEventToApplication(ex.ToString());
            }
        }

        public static void WriteWarningToLog(System.Exception Exception)
        {
            try
            {
                EventLog iLog = GetEventLog();
                iLog.WriteEntry(Exception.ToString(), EventLogEntryType.Warning);
            }
            catch (Exception ex)
            {
                WriteEventToApplication(ex.ToString());
            }
        }

        public static void WriteWarningToLog(string ErrorMessage)
        {
            try
            {
                EventLog iLog = GetEventLog();
                iLog.WriteEntry(ErrorMessage, EventLogEntryType.Warning);
            }
            catch (Exception ex)
            {
                WriteEventToApplication(ex.ToString());
            }
        }

        public static void WriteEventToApplication(string Message)
        {
            try
            {
                WriteEventLogEntry(Message);
            }
            catch (Exception ex)
            {
                // MsgBox(ex.ToString);
            }
        }

        private static void WriteEventLogEntry(string message)
        {
            System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("RelianceOzeki"))
            {
                System.Diagnostics.EventLog.CreateEventSource("RelianceOzeki", "ApplicationLog");
            }

            eventLog.Source = "RelianceOzeki";
            int eventID = 8;
            eventLog.WriteEntry(message, System.Diagnostics.EventLogEntryType.Error, eventID);
            eventLog.Close();
        }
    }
}
