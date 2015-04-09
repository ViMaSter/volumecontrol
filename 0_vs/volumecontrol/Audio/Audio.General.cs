using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio
{
    class Manager
    {
        public static Dictionary<AudioDevice, IList<AudioProcess>> devicesAndProcesses = new Dictionary<AudioDevice, IList<AudioProcess>>();
    }
}
