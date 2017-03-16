using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class CloseHandleParameter : ParameterBase
    {
        public IntPtr Handle { get; private set; }

       
    }
    [Serializable]
    public class CreateFileParameter : ParameterBase
    {
        public string Path { get; set; }
        public uint Access { get; set; }
        public uint Mode { get; set; }
        public IntPtr Handle { get; set; }


       
    }
    [Serializable]
    public class ReadWriteFileParameter : ParameterBase
    {
        public IntPtr Handle { get; set; }
        public int Return { get; set; }

   
    }
    [Serializable]
    public class CopyFileParameter : ParameterBase
    {
        public string From { get; set; }
        public string To { get; set; }

        
    }
    [Serializable]
    public class MoveFileParameter : ParameterBase
    {
        public string From { get; set; }
        public string To { get; set; }

       
    }
    [Serializable]
    public class DeleteFileParameter : ParameterBase
    {
        public string File { get; set; }

       
    }
    [Serializable]
    public class FindFirstFileParameter:ParameterBase
    {
        public string FileName { set; get; }

       
    }
    [Serializable]
    public class LoadLibraryParameter : ParameterBase
    {
        public string LibraryName { set; get; }
        public IntPtr Handle { get; set; }

       
    }
    [Serializable]
    public class GetProcAddressParameter : ParameterBase
    {
        public IntPtr Handle { get; set; }
        public string Proc { set; get; }

       
    }
    [Serializable]
    public class CreateProcessParameter : ParameterBase
    {
        public string ProcessName { set; get; }
        public string Parameters { set; get; }
        public IntPtr Handle { set; get; }
        public long ID { set; get; }

      
    }
    [Serializable]
    public class CreateRemoteThreadParameter : ParameterBase
    {
        public IntPtr Handle { get; set; }

       
    }
    [Serializable]
    public class WriteProcessMemoryParameter : ParameterBase 
    {
        public IntPtr Handle { get; set; }

        
    }
    [Serializable]
    public class Process32FirstParameter : ParameterBase 
    {
        public IntPtr Handle { get; set; }

       
    }

    [Serializable]
    public class CreateToolhelp32Snapshot:ParameterBase
    {
        public uint Flags { set; get; }
        public IntPtr Handle { get; set; }

        
    }
}
