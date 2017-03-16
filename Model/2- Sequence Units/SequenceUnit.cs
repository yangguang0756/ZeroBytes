using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{

    [Serializable]
    public class SequenceUnit
    {
        public APICategory Category { set; get; }
        public APIUnit API { set; get; }
        public List<APIUnit> Consumers { set; get; }
        public SequenceUnit(APICategory cat,APIUnit api)
        {
            Category = cat;
            API = api;
            Consumers = new List<APIUnit>();
        }
        public SequenceUnit(APICategory cat,APIUnit api, List<APIUnit> consumers)
        {
            Category = cat;
            API = api;
            Consumers = new List<APIUnit>();
            if(consumers!=null && consumers.Count>0)
                Consumers.AddRange(consumers);
        }
    }
}
