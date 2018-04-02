using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Tester.Source
{
    /// <summary>
    /// 自动补全内容实体
    /// </summary>
    [ElasticsearchType(IdProperty = "content", Name = "info")]
    public class ComplectionDataInfo
    {
        // 忽略空格
        [Completion(PreserveSeparators = false)]
        public ComplectionDataDetailInfo Suggest { get; set; }

        [Keyword()]
        public string Content { get; set; }
    }
}
