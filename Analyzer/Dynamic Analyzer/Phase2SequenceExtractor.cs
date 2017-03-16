using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
namespace Analyzer
{
    static class SequenceHandler
    {
        public static void ExtractSequenceUnits(ref Sample s)
        {
            var simpleAPIs = s.APIs.Where(api => api.Type == APIType.Simple);
            var complexAPIs = s.APIs.Where(api => api.Type != APIType.Simple);

            s.SequenceUnits.AddRange(simpleAPIs.Select(sApi => new SequenceUnit(sApi.Category, sApi)).ToList());

            var creators = complexAPIs.Where(api => api.Type == APIType.HandleCreation);
            foreach (var c in creators)
            {
                var consumers = complexAPIs.Where(api => api.Handle == c.Handle && api.Category == c.Category && (api.Type == APIType.HandleConsuming || api.Type == APIType.HandleRelease));
                s.SequenceUnits.Add(new SequenceUnit(c.Category, c, consumers.ToList()));
            }            
        }
    }
}
