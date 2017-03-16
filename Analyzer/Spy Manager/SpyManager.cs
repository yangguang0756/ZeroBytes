using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nektra.Deviare2;
using System.Threading;
using System.Runtime.InteropServices;
using Model;
namespace Analyzer
{
    public class SpyManager : IDisposable
    {
        public ManualResetEvent InitializedEvent { set; get; }
        #region Fields
        List<int> lstIDs;
        ManualResetEvent shutdownEvent;
        AutoResetEvent processTerminated;
        Thread initializeThread;
        NktSpyMgr spyMgr;
        eNktHookFlags flags = 0;
        NktHooksEnum hookCollection;
        #endregion
        #region Public Functions
        public SpyManager()
        {           
            spyMgr = new NktSpyMgr();
            lstIDs = new List<int>();
            processTerminated = new AutoResetEvent(false);
            InitializedEvent = new ManualResetEvent(false);
            shutdownEvent = new ManualResetEvent(false);
            Init();
            InitializedEvent.WaitOne();
        }
        public List<APIUnit> InterceptAPIs(string path, int durationSeconds)
        {
            Begin();
            object o;
            NktProcess nktProcess = spyMgr.CreateProcess(path, true, out o);
            if (nktProcess != null)
            {
                lstIDs.Add(nktProcess.Id);
                HookManager.Modules.Add(nktProcess.Name.ToUpper());
                hookCollection.Attach(nktProcess, true);
                spyMgr.OnProcessStarted += spyMgr_OnProcessStarted;
                spyMgr.OnProcessTerminated += spyMgr_OnProcessTerminated;
                spyMgr.ResumeProcess(nktProcess, o);                
                EndAfter(durationSeconds);
                spyMgr.OnProcessStarted -= spyMgr_OnProcessStarted;
                spyMgr.OnProcessTerminated -= spyMgr_OnProcessTerminated;
            }
            return HookManager.Reports.ToList() ;
        }       
        #endregion
        #region Private Methods
        private void Begin()
        {
            HookManager.Reports.Clear();
            HookManager.Modules.Clear();
            lstIDs.Clear();
        }
        void EndAfter(int durationSeconds)
        {
            processTerminated.Reset();
            processTerminated.WaitOne(durationSeconds * 1000);
            lstIDs.ForEach(pid => spyMgr.TerminateProcess(pid, 0));
        }    
        void Init()
        {            
            initializeThread = new Thread(new ThreadStart(InitializeThreadFunction));
            initializeThread.Name = "Main API Thread";
            initializeThread.SetApartmentState(ApartmentState.MTA);
            initializeThread.IsBackground = true;
            initializeThread.Start();
        }
        void InitializeThreadFunction()
        {
            InitializeDeviare();
            WaitForShutdownRequest();
            ShutdownDeviare();
        }
        void InitializeDeviare()
        {
            spyMgr.LicenseKey = @"PGluZm8+PHByb2ROYW1lPmRldmlhcmU8L3Byb2ROYW1lPjx1c2VyTmFtZT5IaXNoYW0gR2FsYWw8
L3VzZXJOYW1lPjx1c2VyRU1haWw+SGlzaGFtLmdhbGFsQGZjaS5hdS5lZHUuZWc8L3VzZXJFTWFp
bD48bGljVHlwZT5lZHVjYXRpb25hbDwvbGljVHlwZT48bGljQ291bnQ+MTwvbGljQ291bnQ+PGV4
cERhdGU+MjAxNjAxMjY8L2V4cERhdGU+PGJ1eURhdGU+MjAxNTAxMjY8L2J1eURhdGU+PC9pbmZv
Pg==|a+PI/2JGEpdWe/AssUkIDODT4CXMUokcW2138BJoKXmBuAPmr/ecRV1Lo8Rp+OUJE2rL2np
qV7tx2xWFhyIIWajViZAOjj27/xT8zQRJsMBtE0jl610WxEpwWX7GM7LbQbxxkCPvaqIusopKCqF
x3yIbTcSKUN8WMWHsHtXU4wjL2N/2rOIjDRLu9Qpwk6QdxPDRpOCb5fSCb/cZWdPlznGO0Mpi4Ke
BiJiEni3Z/LGwlsNOhOP0w2ZCito2iO1llutAbYXAzyDG+qbc6+NmOIPBL9PAHz+KkyATlEW3MfL
7BjRSuCRGplwc+QRrNql4kKbDu3f1CXKURnNIUy/PFQ==";
            spyMgr.Initialize();
            hookCollection = spyMgr.CreateHooksCollection();
            flags |= eNktHookFlags.flgAutoHookChildProcess;
            flags |= eNktHookFlags.flgAutoHookActive;
            flags |= eNktHookFlags.flgOnlyPostCall;
            HookManager.Reports.Clear();
            foreach(string function in HookManager.Handlers.Keys)
            {
                NktHook hook = spyMgr.CreateHook(function, (int)(flags));
                hookCollection.Add(hook);
            }
            spyMgr.OnFunctionCalled += (h, p, c) => { HookManager.Handlers[h.FunctionName](h, p, c); };
            InitializedEvent.Set();
        }
        void spyMgr_OnProcessTerminated(NktProcess proc)
        {
            lstIDs.Remove(proc.Id);
            HookManager.Modules.Remove(proc.Name.ToUpper());
            if (lstIDs.Count == 0)
                processTerminated.Set();
        }
        void spyMgr_OnProcessStarted(NktProcess proc)
        {
            if (lstIDs.Exists(id => proc.ParentId == id))
            {
                lstIDs.Add(proc.Id);
                HookManager.Modules.Add(proc.Name.ToUpper());
            }
        }
        void WaitForShutdownRequest()
        {
            shutdownEvent.WaitOne();
        }
        void ShutdownDeviare()
        {
            try
            {
                Marshal.ReleaseComObject(spyMgr);
            }
            catch { }
        }
        void IDisposable.Dispose()
        {
            shutdownEvent.Set();
            initializeThread.Join();
        }
        #endregion
    }
}
