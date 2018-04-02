using ElasticSearchTester.Tester.Source;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.Words;

namespace ElasticSearchTester.Tester
{
    /// <summary>
    /// 测试纠错
    /// </summary>
    [TestClass]
    public class SuggestTester : BaseTester
    {
        private string idxName = "test_suggest";

        /// <summary>
        /// 建立索引
        /// </summary>
        [TestMethod]
        public void InitIndex()
        {
            var settings = new Dictionary<string, object>();

            settings.Add("index", new
            {
                number_of_replicas = 0,
                number_of_shards = 1,
            });

            var result0 = Proxy.DeleteIndexAsync(idxName).Result;

            // 创建索引                 
            var result1 = Proxy.CreateIndexAsync(idxName,
                settings).Result;

            // 创建映射
            var result2 = Proxy.MappingAsync<SuggestDataInfo>(idxName).Result;

            // 填充数据
            var items = new string[] {
                "手机","华为手机","小米","小米电脑","苹果手机"
            };
            var ds = new List<SuggestDataInfo>();
            foreach (var item in items)
            {               
                ds.Add(new SuggestDataInfo()
                {
                    Content = item
                });
            }
            var r1 = Proxy.UpsertDocManyAsync(idxName, ds).Result;
        }

        [TestMethod]
        public void Test()
        {
            var result2 = Proxy.SearchDocAsync<SuggestDataInfo>(idxName,
                suggest: (d) =>
                {
                    return d.Phrase("test_suggest",
                        (cs) =>
                        {
                            return cs
                                .Field(info => info.Content)
                                .Text("小密")
                                .Size(10);
                        });
                }).Result;
        }           
    }
}
