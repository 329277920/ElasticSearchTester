using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Tester.Source
{
    /// <summary>
    /// 自动补全明细实体
    /// </summary>
    public class ComplectionDataDetailInfo
    {
        /// <summary>
        /// 设置该项的所有可能值，比如中文、英文、拼音、拼音首字母、繁体等。
        /// </summary>
        [Keyword]
        public string[] input { get; set; }

        /// <summary>
        /// 设置该项的权重，在列表中的位置
        /// </summary>
        public int weight { get; set; }
    }
}
