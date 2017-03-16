using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nektra.Deviare2;
using System.Runtime.InteropServices;
using Model;
namespace Analyzer
{
    static class HookManager
    {
        public static Dictionary<string, Action<NktHook, NktProcess, NktHookCallInfo>> Handlers = new Dictionary<string, Action<NktHook, NktProcess, NktHookCallInfo>>()
        {

            {"kernel32.dll!CreateFileA",OnCreateFile},{"kernel32.dll!CreateFileW",OnCreateFile},
            {"Kernel32.dll!CopyFileA",OnCopyFile},{ "Kernel32.dll!CopyFileW",OnCopyFile },{"Kernel32.dll!CopyFileExW",OnCopyFile },{"Kernel32.dll!CopyFileExA",OnCopyFile},
            {"Kernel32.dll!MoveFileA",OnMoveFile},{ "Kernel32.dll!MoveFileExA",OnMoveFile },{"Kernel32.dll!MoveFileW",OnMoveFile },{"Kernel32.dll!MoveFileExW",OnMoveFile},
            {"Kernel32.dll!DeleteFileA",OnDeleteFile},{ "Kernel32.dll!DeleteFileExA",OnDeleteFile },{"Kernel32.dll!DeleteFileW",OnDeleteFile },{"Kernel32.dll!DeleteFileExW",OnDeleteFile},
            {"Kernel32.dll!FindFirstFileA",OnFindFirstFile},{ "Kernel32.dll!FindFirstFileExA",OnFindFirstFile },{"Kernel32.dll!FindFirstFileW",OnFindFirstFile },{"Kernel32.dll!FindFirstFileExW",OnFindFirstFile},
            {"Kernel32.dll!LoadLibraryA",OnLoadLibrary},{ "Kernel32.dll!LoadLibraryExA",OnLoadLibrary },{"Kernel32.dll!LoadLibraryW",OnLoadLibrary },{"Kernel32.dll!LoadLibraryExW",OnLoadLibrary},
            {"Kernel32.dll!GetProcAddress",OnGetProcAddress},
            {"kernel32.dll!CreateProcessA",OnCreateProcess},{"kernel32.dll!CreateProcessW",OnCreateProcess},
            {"Kernel32.dll!CreateRemoteThread",OnCreateRemoteThread},{"Kernel32.dll!CreateRemoteThreadEx",OnCreateRemoteThread},
            {"Kernel32.dll!WriteProcessMemory",OnWriteProcessMemory},
            {"Kernel32.dll!Process32First",OnProcess32First},
            {"Kernel32.dll!CreateToolhelp32Snapshot",OnCreateToolhelp32Snapshot},

            {"Advapi32.dll!RegCreateKeyExA",OnRegCreateKey},{ "Advapi32.dll!RegCreateKeyExW",OnRegCreateKey },{"Advapi32.dll!RegCreateKeyA",OnRegCreateKey },{"Advapi32.dll!RegCreateKeyW",OnRegCreateKey},
            {"Advapi32.dll!RegOpenKeyExA",OnRegOpenKey},{ "Advapi32.dll!RegOpenKeyExW",OnRegOpenKey },{"Advapi32.dll!RegOpenKeyA",OnRegOpenKey },{"Advapi32.dll!RegOpenKeyW",OnRegOpenKey},
            {"Advapi32.dll!RegSetValueExA",OnRegSetValue},{ "Advapi32.dll!RegSetValueExW",OnRegSetValue },{"Advapi32.dll!RegSetValueA",OnRegSetValue },{"Advapi32.dll!RegSetValueW",OnRegSetValue},
            {"Advapi32.dll!CreateServiceA",OnCreateService},{ "Advapi32.dll!CreateServiceW",OnCreateService },

            {"Shell32.dll!ExecuteShellA",OnShellExecute},{"Shell32.dll!ExecuteShellW",OnShellExecute},{"Shell32.dll!ExecuteShellExA",OnShellExecute},{"Shell32.dll!ExecuteShellExW",OnShellExecute},

            {"User32.dll!SetWindowsHookExA",OnSetWindowsHook},{"User32.dll!SetWindowsHookExW",OnSetWindowsHook},

            {"ws2_32.dll!bind",OnSocket},{"ws2_32.dll!connect",OnSocket},
            {"UrlMon.dll!URLDownloadToFileA",OnUrlDownloadToFile},{"UrlMon.dll!URLDownloadToFileW",OnUrlDownloadToFile}
        };
        public static Queue<APIUnit> Reports = new Queue<APIUnit>();
        public static List<string> Modules = new List<string>();

        #region Methods
        static APIUnit Base(APIType type, APICategory cat,APIID id,NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            if (callInfo.StackTrace().Module(0) == null)
                return null;
            string module = callInfo.StackTrace().Module(0).Name.ToUpper();
            if (!Modules.Contains(module))
                return null;

            APIUnit report = new APIUnit(process.Id, hook.FunctionName,type,cat,id);
            report.Module = module;
            return report;
        }
        #region Kernel
        static void OnCreateFile(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            var report = Base(APIType.HandleCreation,APICategory.Files,APIID.CreateFile, hook, process, callInfo);
            if(report==null)
                return;
            var param = new CreateFileParameter();
            param.Path = callInfo.Params().GetAt(0).ReadString();
            param.Access = callInfo.Params().GetAt(1).ULongVal;
            param.Mode = callInfo.Params().GetAt(4).ULongVal;
            param.Handle = callInfo.Result().SizeTVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnLoadLibrary(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            
            var report = Base(APIType.HandleCreation, APICategory.LibraryLoading, APIID.LoadLibrary, hook, process, callInfo);
            if (report == null)
                return;
            var param = new LoadLibraryParameter();
            if (callInfo.Params().GetAt(0).IsNullPointer)
                param.LibraryName = "N/A";
            else
            {
                param.LibraryName = callInfo.Params().GetAt(0).ReadString();

            }
            param.Handle = callInfo.Result().SizeTVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnCreateProcess(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {

            var report = Base(APIType.HandleCreation, APICategory.Process, APIID.CreateProcess, hook, process, callInfo);
            if(report==null)
                return;
            var param = new CreateProcessParameter();
            param.ProcessName = callInfo.Params().GetAt(0).IsNullPointer ? "" : callInfo.Params().GetAt(1).ReadString();
            param.Parameters = callInfo.Params().GetAt(1).IsNullPointer ? "" : callInfo.Params().GetAt(1).ReadString();
            param.Handle = callInfo.Params().GetAt(9).Evaluate().Fields().GetAt(0).SizeTVal;
            param.ID = callInfo.Params().GetAt(9).Evaluate().Fields().GetAt(2).ULongVal;
            try
            {
                if (param.ProcessName == "")
                {
                    var pro = System.Diagnostics.Process.GetProcessById((int)param.ID);
                    param.ProcessName = pro.ProcessName;
                }
            }
            catch { }
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnCreateToolhelp32Snapshot(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            var report = Base(APIType.HandleCreation, APICategory.ToolHelp, APIID.CreateToolhelp, hook, process, callInfo);
            if(report==null)
                return;
            report.ID = APIID.CreateToolhelp;
            var param = new CreateToolhelp32Snapshot();
            param.Flags = callInfo.Params().GetAt(0).ULongVal;
            param.Handle = callInfo.Result().SizeTVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnCopyFile(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            APIUnit report = Base(APIType.Simple, APICategory.Simple, APIID.CopyFile,hook, process, callInfo);
            if(report==null)
                return;
            var param = new CopyFileParameter();
            param.From = callInfo.Params().GetAt(0).ReadString();
            param.To = callInfo.Params().GetAt(1).ReadString();
            report.Parameter = param;
            Reports.Enqueue(report);           

        }
        static void OnMoveFile(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            APIUnit report = Base(APIType.Simple, APICategory.Simple, APIID.MoveFile, hook, process, callInfo);
            if (report == null)
                return;
            var param = new MoveFileParameter();
            param.From = callInfo.Params().GetAt(0).ReadString();
            param.To = callInfo.Params().GetAt(1).ReadString();
            report.Parameter = param;
            Reports.Enqueue(report);

        }
        static void OnDeleteFile(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            APIUnit report = Base(APIType.Simple, APICategory.Simple,APIID.DeleteFile, hook, process, callInfo);
            if (report == null)
                return;
            var param = new DeleteFileParameter();
            param.File = callInfo.Params().GetAt(0).ReadString();
            report.Parameter = param;
            Reports.Enqueue(report);

        }
        static void OnFindFirstFile(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {

            APIUnit report = Base(APIType.Simple, APICategory.Simple,APIID.FindFirstFile, hook, process, callInfo);
            if(report==null)
                return;
            var param = new FindFirstFileParameter();
            param.FileName = callInfo.Params().GetAt(0).ReadString();
            report.Parameter = param;
            Reports.Enqueue(report);

        } 
        static void OnGetProcAddress(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            var report = Base(APIType.HandleConsuming, APICategory.LibraryLoading,APIID.GetProcAddress, hook, process, callInfo);
            if (report == null)
                return;
            var param = new GetProcAddressParameter();
            param.Handle = callInfo.Params().GetAt(0).SizeTVal;
            param.Proc = callInfo.Params().GetAt(1).ReadString();
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnCreateRemoteThread(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {

            var report = Base(APIType.HandleConsuming,APICategory.Process,APIID.CreateRemoteThread, hook, process, callInfo);
            if (report == null)
                return;
            var param = new CreateRemoteThreadParameter();
            param.Handle = callInfo.Params().GetAt(0).SizeTVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnWriteProcessMemory(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            var report = Base(APIType.HandleConsuming,APICategory.Process,APIID.WriteProcessMemory, hook, process, callInfo);
            if (report == null)
                return;
            var param = new WriteProcessMemoryParameter();
            param.Handle = callInfo.Params().GetAt(0).SizeTVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnProcess32First(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            var report = Base(APIType.HandleConsuming,APICategory.ToolHelp,APIID.Process32First, hook, process, callInfo);
            if (report == null)
                return;
            report.ID = APIID.Process32First;
            var param = new Process32FirstParameter();
            param.Handle = callInfo.Params().GetAt(0).SizeTVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        
        #endregion
        #region AdvAPI
        static void OnRegCreateKey(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            var report = Base(APIType.HandleCreation,APICategory.Registry,APIID.RegCreateKey, hook, process, callInfo);
            if (report == null)
                return;
            var param = new RegCreateKeyParameter();
            param.HKey = callInfo.Params().GetAt(0).SizeTVal;
            param.SubKey = callInfo.Params().GetAt(1).ReadString();
            param.Handle = hook.FunctionName.ToUpper().Contains("EX") ? callInfo.Params().GetAt(7).Evaluate().SizeTVal : callInfo.Params().GetAt(2).Evaluate().SizeTVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnRegOpenKey(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            var report = Base(APIType.HandleCreation,APICategory.Registry,APIID.RegOpenKey, hook, process, callInfo);
            if (report == null)
                return;
            var param = new RegOpenKeyParameter();
            param.HKey = callInfo.Params().GetAt(0).SizeTVal;
            param.SubKey = callInfo.Params().GetAt(1).ReadString();
            param.Handle = hook.FunctionName.ToUpper().Contains("EX") ? callInfo.Params().GetAt(4).Evaluate().SizeTVal : callInfo.Params().GetAt(2).Evaluate().SizeTVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnRegSetValue(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            var report = Base(APIType.HandleConsuming,APICategory.Registry,APIID.RegSetValue, hook, process, callInfo);
            if (report == null)
                return;
            var param = new RegSetValueParameter();
            param.Handle = callInfo.Params().GetAt(0).SizeTVal;
            param.Value = callInfo.Params().GetAt(1).IsNullPointer ? "Default" : callInfo.Params().GetAt(1).ReadString();
            string data = "";
            uint type, len;
            INktParam pData;
            if (hook.FunctionName.Contains("Ex"))
            {
                type = callInfo.Params().GetAt(3).ULongVal;
                len = callInfo.Params().GetAt(5).ULongVal;
                pData = callInfo.Params().GetAt(4);
            }
            else
            {
                type = callInfo.Params().GetAt(2).ULongVal;
                len = callInfo.Params().GetAt(4).ULongVal;
                pData = callInfo.Params().GetAt(3);
            }
            if (!pData.IsNullPointer)
            {
                switch (type)
                {
                    case 1:
                    case 2:
                    case 7:
                        byte[] buf = new byte[len];
                        GCHandle h = GCHandle.Alloc(buf, GCHandleType.Pinned);
                        IntPtr p = h.AddrOfPinnedObject();
                        INktProcessMemory mem = pData.Memory();
                        mem.ReadMem(p, pData.PointerVal, (IntPtr)len);
                        h.Free();
                        if (hook.FunctionName.Contains("W"))
                            data = Encoding.Unicode.GetString(buf, 0, (int)len);
                        else
                            data = Encoding.ASCII.GetString(buf, 0, (int)len);
                        break;
                    case 3:
                    case 4:
                        data = pData.Evaluate().ULongVal.ToString();
                        break;
                }

            }
            data = data.Replace('\0', ' ');
            data = data.Trim();
            param.Type = type;
            param.Data = data;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        static void OnCreateService(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            APIUnit report = Base(APIType.Simple, APICategory.Simple,APIID.CreateService, hook, process, callInfo);
            if (report == null)
                return;
            var param = new CreateServiceParameter();
            param.Name=callInfo.Params().GetAt(1).IsNullPointer?"N/A":callInfo.Params().GetAt(1).ReadString();
            param.Path= callInfo.Params().GetAt(7).IsNullPointer?"N/A":callInfo.Params().GetAt(7).ReadString();
            param.Mode=callInfo.Params().GetAt(5).ULongVal;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        #endregion
        #region Shell32
        static void OnShellExecute(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            APIUnit report = Base(APIType.Simple, APICategory.Simple,APIID.ShellExecute, hook, process, callInfo);
            if (report == null)
                return;
            report.ID = APIID.ShellExecute;
            var param = new ShellExecuteParameter();
            if (hook.FunctionName.Contains("teEx"))
            {
                INktParam p = callInfo.Params().GetAt(0).Evaluate();
                param.Name = p.Fields().GetAt(4).IsNullPointer ? "" : p.Fields().GetAt(4).ReadString();
                param.Parameters = p.Fields().GetAt(5).IsNullPointer ? "" : p.Fields().GetAt(5).ReadString();
                param.Directory = p.Fields().GetAt(6).IsNullPointer ? "" : p.Fields().GetAt(6).ReadString();
            }
            else
            {
                param.Name = callInfo.Params().GetAt(2).IsNullPointer ? "" : callInfo.Params().GetAt(2).ReadString();
                param.Parameters = callInfo.Params().GetAt(3).IsNullPointer ? "" : callInfo.Params().GetAt(3).ReadString();
                param.Directory = callInfo.Params().GetAt(4).IsNullPointer ? "" : callInfo.Params().GetAt(4).ReadString();
            }
            report.Parameter = param;
            Reports.Enqueue(report);          

        }
        #endregion
        #region User32
        static void OnSetWindowsHook(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            APIUnit report = Base(APIType.Simple, APICategory.Simple,APIID.SetWindowsHook, hook, process, callInfo);
            if (report == null)
                return;
            report.ID = APIID.SetWindowsHook;
            var param = new SetWindowsHookParameter();
            param.HookType = callInfo.Params().GetAt(0).LongVal;
            report.Parameter = param;
            Reports.Enqueue(report);           

        }
        #endregion
        #region WS2_32
        static void OnSocket(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            APIUnit report = Base(APIType.Simple, APICategory.Simple,APIID.SocketConnect, hook, process, callInfo);
            if (report == null)
                return;
            var param = new ConnectionParameter();            
            int len = callInfo.Params().GetAt(2).LongVal;
            byte[] buf = new byte[len];
            GCHandle h = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr p = h.AddrOfPinnedObject();
            var add = callInfo.Params().GetAt(1);
            INktProcessMemory mem = add.Memory();
            mem.ReadMem(p, add.PointerVal, (IntPtr)len);
            h.Free();
            report.ID = hook.FunctionName.Contains("bind") ? APIID.SocketBind : APIID.SocketConnect;
            param.Port = (ushort)(buf[2] * 256 + buf[3]);
            param.IP = String.Format("{0}.{1}.{2}.{3}", buf[4].ToString("D3"), buf[5].ToString("D3"), buf[6].ToString("D3"), buf[7].ToString("D3"));
            param.Server = hook.FunctionName.Contains("bind") ? true : false;
            report.ID = param.Server ? APIID.SocketBind : APIID.SocketConnect;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        #endregion
        #region UrlMon
        static void OnUrlDownloadToFile(NktHook hook, NktProcess process, NktHookCallInfo callInfo)
        {
            APIUnit report = Base(APIType.Simple, APICategory.Simple, APIID.SocketConnect, hook, process, callInfo);
            if (report == null)
                return;
            var param = new UrlDownloadToFileParameter();
            param.Url = callInfo.Params().GetAt(1).ReadString();
            param.FilePath = callInfo.Params().GetAt(2).ReadString();
            report.ID = APIID.UrlDownloadToFile;
            report.Parameter = param;
            Reports.Enqueue(report);
        }
        #endregion
        #endregion
    }
}
