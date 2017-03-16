using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.IO;

namespace Analyzer
{
    public static class DynamicAnalyzer
    {               
        public static void Analyze(ref Sample s,int durationInSeconds=300)
        {
            APIHandler.ExtractAPIUnits(ref s, durationInSeconds);
            SequenceHandler.ExtractSequenceUnits(ref s);
            SemanticHandler.ExtractSemanticUnits(ref s);
        }
    }
}
