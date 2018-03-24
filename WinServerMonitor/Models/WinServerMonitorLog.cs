using Orm.Son.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinServerMonitor.Models
{
    public class WinServerMonitorLog
    {
        [Key,Description("编号GUID")]
        public string Id { get; set; }

        [Description("CPU使用率")]
        public double ProcessorPercent { get; set; }

        [Description("内存使用率")]
        public double MemoryPercent { get; set; }

        [Description("IIS实例名")]
        public string InstanceName { get; set; }

        [Description("IIS实例当前连接数")]
        public double InstanceConnectionCnt { get; set; }

        [Description("IIS实例最大连接数")]
        public double InstanceMaxConnectionCnt { get; set; }

        [Description("添加时间")]
        public DateTime AddTime { get; set; }

    }
}
