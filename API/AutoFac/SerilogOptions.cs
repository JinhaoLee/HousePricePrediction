using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Events;


namespace API.AutoFac
{
    public class SerilogOptions
    {
        public LogEventLevel RollingFileLogEventLevel { get; set; }
        public LogEventLevel ConsoleLogEventLevel { get; set; }
    }

}