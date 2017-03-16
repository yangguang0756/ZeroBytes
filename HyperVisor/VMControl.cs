using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vestris.VMWareLib;
namespace HyperVisor
{
    class VMControl
    {
        public static string VMPath { set; get; }
        internal static void PowerOnWithReset()
        {
            using (VMWareVirtualHost vHost = new VMWareVirtualHost())
            {
                vHost.ConnectToVMWareWorkstation();
                using (var vMachine = vHost.Open(VMPath))
                {
                    var snap = vMachine.Snapshots.FindSnapshotByName("Ready");
                    snap.RevertToSnapshot( 120);
                    if (!vMachine.IsRunning)
                    {
                        vMachine.PowerOn();
                        vMachine.WaitForToolsInGuest();
                    }                    
                }
            }
        }        
    }
}
