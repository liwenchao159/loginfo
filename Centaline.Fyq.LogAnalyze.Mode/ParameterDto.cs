using Nest;
using System.Collections.Generic;
using System.IO;

namespace Centaline.Fyq.LogAnalyze
{
    [ElasticsearchType(Name = "parameterdto")]
    public class ParameterDto
    {
        [Object]
        public Dictionary<string, object> GET { get; set; }
        [Object]
        public Dictionary<string, object> POST { get; set; }
    }
}