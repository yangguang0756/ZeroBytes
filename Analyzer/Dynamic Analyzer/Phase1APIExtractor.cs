using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analyzer
{
    static class APIHandler
    {
        static SpyManager spyManager = new SpyManager();
        public static void ExtractAPIUnits(ref Sample s,int durationInSeconds=30)
        {
            var apis=spyManager.InterceptAPIs(s.FilePath, durationInSeconds);
            s.APIs = apis.GroupBy(a => a.MD5).Select(ss=>ss.First()).ToList();
        }
    }
}
