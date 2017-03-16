using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    [Serializable]
    public class UrlDownloadToFileParameter : ParameterBase
    {
        public string Url { set; get; }
        public string FilePath { set; get; }

    }
}
