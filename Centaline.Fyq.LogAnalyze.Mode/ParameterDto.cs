using Nest;
using System.Collections.Generic;
using System.IO;

namespace Centaline.Fyq.LogAnalyze
{
    [ElasticsearchType(Name = "parameterdto")]
    public class ParameterDto
    {
        [Nested]
        public List<KeyValueDto> GET { get; set; }
        [Nested]
        public List<KeyValueDto> POST { get; set; }

    }
        public class PostGetParameter
        {
            public Dictionary<string,string> GET { get; set; }
            public Dictionary<string,string> POST { get; set; }
        }

        [ElasticsearchType(Name = "KeyValueDto")]
    public class KeyValueDto
    {
        [Keyword]
        public string Key { get; set; }
        [Keyword]
        public string Value { get; set; }
    }
}