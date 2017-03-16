
using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Analyzer
{
    static class SemanticHandler
    {
        static Sample sample;
        static Dictionary<APIID, Func<SequenceUnit, List<SemanticUnit>>> Handle = new Dictionary<APIID, Func<SequenceUnit, List<SemanticUnit>>>() 
        {
        {APIID.CreateFile,OnCreateFile},
        {APIID.LoadLibrary,OnLoadLibrary},
        {APIID.CreateProcess,OnCreateProcess},
        {APIID.CreateToolhelp,OnCreateToolHelp},

        {APIID.RegCreateKey,OnRegCreateKey},
        {APIID.RegOpenKey,OnRegOpenKey},        
        {APIID.CreateService,OnCreateService},

        {APIID.CopyFile,OnCopyFile},
        {APIID.MoveFile,OnMoveFile},
        {APIID.DeleteFile,OnDeleteFile},
        {APIID.FindFirstFile,OnFindFirstFile},
        
        {APIID.ShellExecute,OnShellExecute},
        {APIID.SetWindowsHook,OnSetWindowsHook},
        {APIID.SocketBind,OnSocketBind},
        {APIID.SocketConnect,OnSocketConnect},
        {APIID.UrlDownloadToFile,OnUrlDownloadFile}
        };
        
        public static void ExtractSemanticUnits(ref Sample s)
        {
            sample=s;
            foreach(SequenceUnit seq in sample.SequenceUnits)
            {
                sample.SemanticUnits.AddRange(Handle[seq.API.ID](seq));
            }
            sample.SemanticUnits = sample.SemanticUnits.GroupBy(su => su.MD5).Select(ssu => ssu.First()).ToList();
        }

        #region Methods
        static List<SemanticUnit> OnUrlDownloadFile(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            InternetOperationSemanticUnit su = new InternetOperationSemanticUnit();
            var p = s.API.Parameter as UrlDownloadToFileParameter;
            su.URL = p.Url;
            su.FilePath = p.FilePath;
            su.Operation = "Download";
            try { su.FileDirectory = Path.GetDirectoryName(p.FilePath); }
            catch { }
            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnCreateFile(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            CreateFileSemanticUnit sU = new CreateFileSemanticUnit();
            var cParam = (CreateFileParameter)(((SequenceUnit)s).API.Parameter);

            sU.Access = cParam.Access;
            sU.Mode = cParam.Mode;
            try { sU.Directory = Path.GetDirectoryName(cParam.Path).ToUpper().Trim(); }
            catch { sU.Directory = cParam.Path; }

            try { sU.Extension = Path.GetExtension(cParam.Path).ToUpper().Trim(); }
            catch { }
            if (IsExecutable(cParam.Path))
                sU.Extension = "EXE";

            sem.Add(sU);
            return sem;
        }

        private static bool IsExecutable(string path)
        {
            Stream file;
            bool status = false;
            try
            {
                file = File.OpenRead(path);
                if (file.ReadByte() == 0x4d && file.ReadByte() == 0x5a)
                    status= true;
                file.Close();
                return status;
            }catch
            {                
                return false;
            }

        }
        static List<SemanticUnit> OnLoadLibrary(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            RuntimeLoadingSemanticUnit sU = new RuntimeLoadingSemanticUnit();
            SequenceUnit seq=(SequenceUnit)s;
            var cParam = (LoadLibraryParameter)seq.API.Parameter;

            foreach (APIUnit api in seq.Consumers)
                sem.Add(new RuntimeLoadingSemanticUnit() { Module = cParam.LibraryName.ToUpper().Trim(), Function = ((GetProcAddressParameter)api.Parameter).Proc });
            return sem;
        }
        static List<SemanticUnit> OnRegCreateKey(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;                        
            var cParam = (RegCreateKeyParameter)seq.API.Parameter;
            foreach (APIUnit api in seq.Consumers)
            {
                RegSetValueParameter sParam = (RegSetValueParameter)api.Parameter;
                string value = string.Format(@"{0:X2}\{1}\{2}", cParam.HKey.ToInt32(), cParam.SubKey, sParam.Value);
                sem.Add(new RegSetValueSemanticUnit() {Value=value.ToUpper().Trim(), Data=sParam.Data.ToUpper().Trim()});
            }
            return sem;
        }
        static List<SemanticUnit> OnRegOpenKey(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (RegOpenKeyParameter)seq.API.Parameter;
            foreach (APIUnit api in seq.Consumers)
            {
                RegSetValueParameter sParam = (RegSetValueParameter)api.Parameter;
                string value = string.Format(@"{0:X2}\{1}\{2}", cParam.HKey.ToInt32(), cParam.SubKey, sParam.Value);
                sem.Add(new RegSetValueSemanticUnit() { Value = value.ToUpper().Trim(), Data = sParam.Data.ToUpper().Trim() });
            }
            return sem;
        }
        static List<SemanticUnit> OnCreateProcess(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (CreateProcessParameter)seq.API.Parameter;
            CreateProcessSemanticUnit su = new CreateProcessSemanticUnit();
            su.ProcessName = cParam.ProcessName.ToUpper().Trim();
            foreach (APIUnit api in seq.Consumers)
            {
                var remote = api.Parameter as CreateRemoteThreadParameter;
                if (remote != null)
                {
                    su.Injected = true;
                    sem.Add(new CodeInjectionSemanticUnit());
                }
            }
            List<string> pathes=sample.APIs.Select(ss=> {var f=(APIUnit)ss; if(f!=null && f.Parameter is CreateFileParameter) return ((CreateFileParameter)f.Parameter).Path; else return "";}).ToList();
            if(pathes!=null)
                su.Dropped = pathes.Contains(su.ProcessName, StringComparer.OrdinalIgnoreCase);
            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnCopyFile(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (CopyFileParameter)seq.API.Parameter;
            CopyFileSemanticUnit su = new CopyFileSemanticUnit();
            try { su.To = Path.GetDirectoryName(cParam.To).ToUpper().Trim(); }
            catch { su.To = cParam.To; }
            try { su.Extension = Path.GetExtension(cParam.To).ToUpper().Trim(); }
            catch { }
            if (IsExecutable(cParam.To))
                su.Extension = "EXE";

            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnMoveFile(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (MoveFileParameter)seq.API.Parameter;
            var su = new MoveFileSemanticUnit();
            try { su.To = Path.GetDirectoryName(cParam.To).ToUpper().Trim(); }
            catch { su.To = cParam.To; }
            try { su.Extension = Path.GetExtension(cParam.To).ToUpper().Trim(); }
            catch { }
            if (IsExecutable(cParam.To))
                su.Extension = "EXE";

            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnDeleteFile(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (DeleteFileParameter)seq.API.Parameter;
            var su = new DeleteFileSemanticUnit();
            try { su.File = Path.GetDirectoryName(cParam.File).ToUpper().Trim(); }
            catch { su.File = cParam.File; }
            try { su.Extension = Path.GetExtension(cParam.File).ToUpper().Trim(); }
            catch { }
            if (IsExecutable(cParam.File))
                su.Extension = "EXE";

            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnFindFirstFile(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (FindFirstFileParameter)seq.API.Parameter;
            var su = new FindFirstFileSemanticUnit();
            su.Pattern = cParam.FileName.ToUpper().Trim();

            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnCreateService(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (CreateServiceParameter)seq.API.Parameter;
            var su = new CreateServiceSemanticUnit();
            su.Name=cParam.Name;
            su.Mode=cParam.Mode;/// ?Drop service
            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnShellExecute(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (ShellExecuteParameter)seq.API.Parameter;
            var su = new ShellExecuteSemanticUnit();
            su.Name=Path.Combine(cParam.Directory,cParam.Name);
            su.Parameters=cParam.Parameters;
            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnSetWindowsHook(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (SetWindowsHookParameter)seq.API.Parameter;
            var su = new SetWindowsHookSemanticUnit();
            su.HookMode = cParam.HookType;
            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnCreateToolHelp(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (CreateToolhelp32Snapshot)seq.API.Parameter;
            var su = new CreateToolHelpSemanticUnit();
            su.Flags=cParam.Flags;
            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnProcess32First(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (Process32FirstParameter)seq.API.Parameter;
            var su = new Process32FirstSemanticUnit();
            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnSocketConnect(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
           var cParam = (ConnectionParameter)seq.API.Parameter;
            var su = new SocketConnectSemanticUnit();
            su.Port=cParam.Port;
            su.IP=cParam.IP.Trim();
            sem.Add(su);
            return sem;
        }
        static List<SemanticUnit> OnSocketBind(SequenceUnit s)
        {
            List<SemanticUnit> sem = new List<SemanticUnit>();
            SequenceUnit seq = (SequenceUnit)s;
            var cParam = (ConnectionParameter)seq.API.Parameter;
            var su = new SocketBindSemanticUnit();
            su.Port=cParam.Port;

            sem.Add(su);
            return sem;
        }
        #endregion
    }
}
