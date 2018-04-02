using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Tester.Source
{
    /// <summary>
    /// 纠错存储对象
    /// </summary>
    [ElasticsearchType(IdProperty = "content", Name = "info")]
    public class SuggestDataInfo
    {
        [Keyword]
        public string Content { get; set; }

        //[Keyword]
        //public string[] Suggest { get; set; }
    }
}
