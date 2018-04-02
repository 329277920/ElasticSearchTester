using ElasticSearchTester.Tester.Source;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace ElasticSearchTester.Tester
{
    /// <summary>
    /// 测试索引
    /// </summary>
    [TestClass]
    public class IndexTester : BaseTester
    {
        /// <summary>
        /// 创建索引
        /// </summary>
        [TestMethod]
        public void CreateIndex()
        {
            // 获取索引
            var r1 = Proxy.GetIndexAsync(IndexName).Result;
            if (r1.IsValid)
            {
                // 删除索引
                var r2 = Proxy.DeleteIndexAsync(IndexName).Result;
            }

            // 创建索引开始   ************************************************

            // 定义同义词
            var synonyms = new string[][]
            {
                new string[]{ "Iphone 8","iPhone8" }
            };

            // 定义索引全局参数
            var settings = new Dictionary<string, object>();
            settings.Add("index", new
            {
                number_of_replicas = 0,
                number_of_shards = 1,
            });

            // 配置分词器，与同义词
            settings.Add("analysis",
               new
               {
                   analyzer = new
                   {
                       ext_ik_smart = new
                       {
                           tokenizer = "ik_smart",
                           filter = new string[] { "synonym" }
                       },
                       ext_ik_max_word = new
                       {
                           tokenizer = "ik_max_word",
                           filter = new string[] { "synonym" }
                       }
                   },
                   filter = new
                   {
                       synonym = new
                       {
                           type = "synonym",
                           synonyms = synonyms.Select(item =>
                                        {
                                            return string.Join(", ", item);
                                        })
                       }
                   }
               }
            );
            var r3 = Proxy.CreateIndexAsync(IndexName, settings).Result;
            if (!r3.IsValid)
            {
                throw new Exception(r3.ToString());
            }

            // 创建索引结束   ************************************************

            // 创建类型映射
            var r4 = Proxy.MappingAsync<IndexDataInfo>(IndexName).Result;
            if (!r4.IsValid)
            {
                throw new Exception(r4.ToString());
            }
        }

        /// <summary>
        /// 更新索引配置
        /// </summary>
        [TestMethod]
        public void UpdateIndex()
        {
            // 关闭索引
            var r1 = Proxy.CloseIndexAsync(IndexName).Result;
            if (!r1.IsValid)
            {
                throw new Exception(r1.ToString());
            }

            // 配置分析器

            // 定义同义词
            var synonyms = new string[][]
            {
                new string[]{ "Iphone 8","iPhone8" },
                new string[]{ "Iphone x","iPhonex" }
            };

            var settings = new Dictionary<string, object>();
            settings.Add("analysis", new
            {
                analyzer = new
                {
                    ext_ik_smart = new
                    {
                        tokenizer = "ik_smart",
                        filter = new string[] { "synonym" }
                    },
                    ext_ik_max_word = new
                    {
                        tokenizer = "ik_max_word",
                        filter = new string[] { "synonym" }
                    }
                },
                filter = new
                {
                    synonym = new
                    {
                        type = "synonym",
                        // synonyms_path = "synonyms.txt"
                        synonyms = synonyms == null ? new string[] { }
                                        : synonyms.Select(item =>
                                        {
                                            return string.Join(", ", item);
                                        })
                    }
                }
            });
            var r2 = Proxy.UpdateIndex(IndexName, settings).Result;
            if (!r2.IsValid)
            {
                throw new Exception(r2.ToString());
            }

            // 打开索引
            var r3 = Proxy.OpenIndexAsync(IndexName).Result;
            if (!r3.IsValid)
            {
                throw new Exception(r3.ToString());
            }
        }

        /// <summary>
        /// 填充索引
        /// </summary>
        [TestMethod]
        public void FillIndex()
        {
            List<IndexDataInfo> dataSource = new List<IndexDataInfo>();
            using (FileStream fs = new FileStream("data.txt", FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    var json = sr.ReadLine();
                    while (json != null)
                    {
                        var item = Newtonsoft.Json.JsonConvert.DeserializeObject<IndexDataInfo>(json);
                        dataSource.Add(item);
                        json = sr.ReadLine();
                    }
                }
            }        
            var r1 = Proxy.UpsertDocManyAsync(IndexName, dataSource).Result;
            if (!r1.IsValid)
            {
                throw new Exception(r1.ToString());
            }
        }
    }
}
