
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public enum APIType
    {
        Simple,
        HandleCreation,
        HandleConsuming,
        HandleRelease
    }
    public enum APICategory
    {
        Files,
        Registry,
        Process,
        Internet,
        Simple,
        LibraryLoading,
        ToolHelp
    }
    public enum APIID
    {
        CreateFile,
        CopyFile,
        MoveFile,
        DeleteFile,
        FindFirstFile,
        CreateProcess,
        LoadLibrary,
        GetProcAddress,
        CreateRemoteThread,
        WriteProcessMemory,
        CreateToolhelp,
        Process32First,
        OpenProcess,
        SetWindowsHook,
        ShellExecute,
        RegCreateKey,
        RegOpenKey,
        RegSetValue,
        CreateService,
        SocketConnect,
        SocketBind,
        UrlDownloadToFile
    }


    [Serializable]
    public  class APIUnit 
    {
        public APIType Type { set; get; }
        public APIID ID { set; get; }
        public APICategory Category { set; get; }
        public IntPtr Handle { set; get; }
        public int ProcessID { set; get; }
        public string Function { set; get; }
        public ParameterBase Parameter { set; get; }
        public string Module { set; get; }
        public APIUnit(int pid, string function,APIType type, APICategory category,APIID id)
        {
            Function = function.Split('!')[1];
            ProcessID = pid;
            Category = category;
            Type = type;
            ID = id;
        }
        public override string ToString()
        {
            return String.Format("PID\t{0}\r\nModule\t{3}\r\nFunction\t{1}\r\nParameters\r\n{2}\r\n", ProcessID, Function, Parameter.Description,Module);
        }
        public string MD5
        {
             get
            {
                if (Parameter != null)
                    return Parameter.MD5();
                else
                    return "";
            }
        }
    }
}
