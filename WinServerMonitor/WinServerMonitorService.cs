using Orm.Son.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinServerMonitor.Models;
using System.Configuration;

namespace WinServerMonitor
{
    public static class WinServerMonitorService
    {
        static bool WRITE_TO_DB;
        static string IIS_INSTANCE;
        static WinServerMonitorService()
        {
            SonFact.Init("conn");
            WRITE_TO_DB = ConfigurationManager.AppSettings["write_to_db"] == "true" ? true : false;
            IIS_INSTANCE = ConfigurationManager.AppSettings["iis_instance"] ?? "_Total";
        }

        public static void Start()
        {
            var isOk = SonFact.Cur.CreateTable<WinServerMonitorLog>();

            var list = new List<PerformanceCounter> {
                 new PerformanceCounter("Processor","% Processor Time","_Total"),
                 new PerformanceCounter("Memory","% Committed Bytes In Use"),
                 new PerformanceCounter("Web Service","Current Connections",IIS_INSTANCE),
                 new PerformanceCounter("Web Service","Maximum Connections",IIS_INSTANCE),
            };

            Runner(list);
        }

        public static void Runner(List<PerformanceCounter> counters)
        {
            while (true)
            {
                var log = new WinServerMonitorLog();
                foreach (var c in counters)
                {
                    var result = c.NextValue();
                    if (c.CategoryName == "Processor" && c.CounterName == "% Processor Time" && c.InstanceName == "_Total")
                        log.ProcessorPercent = result;
                    else if (c.CategoryName == "Memory" && c.CounterName == "% Committed Bytes In Use")
                        log.MemoryPercent = result;
                    else if (c.CategoryName == "Web Service" && c.CounterName == "Current Connections" && c.InstanceName == IIS_INSTANCE)
                        log.InstanceConnectionCnt = result;
                    else if (c.CategoryName == "Web Service" && c.CounterName == "Maximum Connections" && c.InstanceName == IIS_INSTANCE)
                    {
                        log.InstanceName = c.InstanceName;
                        log.InstanceMaxConnectionCnt = result;
                    }
                }

                log.ProcessorPercent = Math.Round(log.ProcessorPercent, 2);
                log.MemoryPercent = Math.Round(log.MemoryPercent, 2);

                Console.WriteLine(string.Format("CPU:{0}%\t Memory:{1}%\t Iis-cur-conn:{2}\t Iis-max-conn:{3}", log.ProcessorPercent, log.MemoryPercent, log.InstanceConnectionCnt, log.InstanceMaxConnectionCnt));
                if(WRITE_TO_DB)  AddLog(log);
                Thread.Sleep(1000);
            }
        }

        public static void AddLog(WinServerMonitorLog log)
        {
            log.Id = Guid.NewGuid().ToString("N");
            log.AddTime = DateTime.Now;
            SonFact.Cur.Insert(log);
        }

    }
}
