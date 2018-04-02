using ElasticSearchTester.Tester.Source;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolGood.Words;

namespace ElasticSearchTester.Tester
{
    /// <summary>
    /// 测试自动完成
    /// 1、正常查询
    /// 2、如果1没出结果，将关键字转拼音查询（有对拼音的纠错）
    /// 3、如果2没出结果，调用纠错服务，纠错服务主要针对汉字的纠错
    /// </summary>
    [TestClass]
    public class CompletionTester : BaseTester
    {
        private string idxName = "test_complection";

        /// <summary>
        /// 初始化自动补全的索引
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
            var result2 = Proxy.MappingAsync<ComplectionDataInfo>(idxName).Result;

            // 生成索引
            Random rand = new Random();
            List<ComplectionDataInfo> dataSource = new List<ComplectionDataInfo>();
            using (FileStream fs = new FileStream("data.txt", FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var json = sr.ReadLine();
                    while (json != null)
                    {
                        var item = Newtonsoft.Json.JsonConvert.DeserializeObject<IndexDataInfo>(json);
                        if (!string.IsNullOrEmpty(item.title))
                        {
                            // 去掉空格，查询时将
                            var fTitle = item.title.Replace(" ", "");
                            var dataInfo = new ComplectionDataInfo()
                            {

                                Content = item.title,
                                Suggest = new ComplectionDataDetailInfo()
                                {
                                    input = new string[]
                                {
                                     item.title,
                                     WordsHelper.GetPinYin(item.title).ToLower(),
                                     WordsHelper.GetFirstPinYin(item.title).ToLower()
                                },
                                    weight = rand.Next(1, 100) // 随机权重
                                }
                            };
                            dataSource.Add(dataInfo);
                        }                        
                        json = sr.ReadLine();
                    }
                }
            }
            var r1 = Proxy.UpsertDocManyAsync(idxName, dataSource).Result;
            if (!r1.IsValid)
            {
                throw new Exception(r1.ToString());
            }            
        }

        /// <summary>
        /// 测试自动补全
        /// </summary>
        [TestMethod]
        public void Suggest()
        {           
            var r1 = Proxy.SearchDocAsync<ComplectionDataInfo>(idxName,
                suggest: (d) =>
                {
                    return d.Completion("products_completion",
                        (cs) =>
                        {
                            return cs
                                .Field(info => info.Suggest)
                                .Prefix("xiankegaoqing")
                                .Size(15)
                                .Fuzzy(fd =>
                                {
                                    // 允许纠错，不适用于中文
                                    return fd.Fuzziness(Fuzziness.Auto);
                                });
                        });
                }).Result;
        }
    }
}
