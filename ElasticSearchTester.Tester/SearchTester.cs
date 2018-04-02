using Elasticsearch.Net;
using ElasticSearchTester.Tester.Source;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Tester
{
    [TestClass]
    public class SearchTester : BaseTester
    {
        #region 简单查询

        /// <summary>
        /// match_all: 列出所有文档
        /// </summary>
        [TestMethod]
        public void match_all()
        {           
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return q.MatchAll();
            }).Result;
        }

        /// <summary>
        /// match: 按字段全文检索,如果在一个精确值的字段上使用它， 例如数字、日期、布尔或者一个 not_analyzed 字符串字段，那么它将会精确匹配给定的值
        /// </summary>
        [TestMethod]
        public void match()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return q.Match((qd) =>
                {
                    return qd.Field(f => f.title).Query("牛奶");
                });
            }).Result;
        }

        /// <summary>
        /// match: 按字段全文检索,并满足分词后的所有项
        /// </summary>
        [TestMethod]
        public void match2()
        {
            // 需要全部满足
            var r1 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
                query: (q) =>
                    q.Match(mq => mq.Field(info => info.title)
                        .Query("华为手机")
                        .MinimumShouldMatch(MinimumShouldMatch.Fixed(3))
                        // .Operator(Operator.And)
                    )).Result;

            // 至少满足N项
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
              query: (q) =>
                  q.Match(mq => mq.Field(info => info.title)
                      .Query("华为手机")
                      .MinimumShouldMatch(MinimumShouldMatch.Fixed(3))
                      .Boost(0)
                  )).Result;
        }

        /// <summary>
        /// multi_match: 按字段全文检索，可以指定多个字段，只需其中一个满足即可
        /// </summary>
        [TestMethod]
        public void multi_match()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return q.MultiMatch((qd) =>
                {
                    return qd.Fields(f => f.Fields
                        (
                            info=>info.title,
                            info => info.spuname,
                            info => info.sortname
                        )).Query("牛奶");
                });
            }).Result;             
        }

        /// <summary>
        /// range: 按范围查找
        /// </summary>
        [TestMethod]
        public void range()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return q.Range((qd) =>
                {
                    // 价格: 100 ~ 120
                    return qd.Field(info => info.skupricemin).GreaterThanOrEquals(100).LessThanOrEquals(120);
                });
            }).Result;
        }

        /// <summary>
        /// term: 对查询关键字不进行分词
        /// </summary>
        [TestMethod]
        public void term1()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return q.Term((qd) =>
                {
                    return qd.Field(info => info.sortname).Value("苹果");
                });
            }).Result;
        }

        /// <summary>
        /// term: 精确值查询
        /// </summary>
        [TestMethod]
        public void term2()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100,
                filter: (q) =>
                {
                    return q.Term((qd) =>
                    {
                        return qd.Field(info => info.skupricemin).Value(100.0);
                    });
                }).Result;
        }

        /// <summary>
        /// terms: 与term类似，可以指定多个值，只需满足其中一个
        /// </summary>
        [TestMethod]
        public void terms()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return q.Terms((qd) =>
                {
                    return qd.Field(info => info.sortname).Terms("苹果","手机");
                });
            }).Result;
        }

        /// <summary>
        /// exists: 判断某个字段是否有值
        /// </summary>
        [TestMethod]
        public void exists()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return q.Exists((qd) =>
                {
                    return qd.Field(info => info.keyword);
                });
            }).Result;
        }

        /// <summary>
        /// missing: 判断某个字段没有值
        /// </summary>
        [TestMethod]
        public void missing()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return !q.Exists((qd) =>
                {
                    return qd.Field(info => info.keyword);
                });
            }).Result;
        }

        #endregion

        #region 组合查询

        /// <summary>
        /// must: 文档 必须 匹配这些条件才能被包含进来
        /// </summary>
        [TestMethod]
        public void must()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return
                q.Match((qd) =>
                {
                    return qd.Field(info => info.title).Query("牛奶");
                }) &&
                q.Match((qd) =>
                 {
                     return qd.Field(info => info.title).Query("傻逼");
                 });
            }).Result;
        }

        /// <summary>
        /// must_not: 文档 必须不 匹配这些条件才能被包含进来
        /// </summary>
        [TestMethod]
        public void must_not()
        {
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return
                !(q.Match((qd) =>
                {
                    return qd.Field(info => info.title).Query("手机");
                }) ||
                q.Match((qd) =>
                {
                    return qd.Field(info => info.title).Query("牛奶");
                }));
            }).Result;
        }

        /// <summary>
        /// should: 如果满足这些语句中的任意语句，将增加 _score ，否则，无任何影响。它们主要用于修正每个文档的相关性得分
        ///         如果一个组合语句中只有should，则需要满足其中一个
        /// </summary>
        [TestMethod]
        public void should()
        {
            // 查询title包含'手机'，如果出现'华为'、'4G'，则增加分值
            var r1 = Proxy.Client.LowLevel.Search<SearchResponse<IndexDataInfo>>(IndexName, 
                PostData.Serializable(
                        new
                        {
                            query = new
                            {
                                @bool = new
                                {
                                    must = new
                                    {
                                        match = new { title = "手机" }
                                    },
                                    should = new object[] {
                                        new {
                                            match = new { title = "王八" }
                                        },
                                        new {
                                            match = new { title = "二狗" }
                                        }
                                    }
                                }
                            }
                        }
                    ));


            // 查询title包含'手机'，且title包含'华为'、'4g'其中一个
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100, query: (q) =>
            {
                return
                q.Match(qd=>qd.Field(info=>info.title).Query("手机")) &&
                (q.Match((qd) =>
                {
                    return qd.Field(info => info.title).Query("华为");
                }) ||
                q.Match((qd) =>
                {
                    return qd.Field(info => info.title).Query("4g");
                }));
            }).Result;
        }

        /// <summary>
        /// should:  
        ///          
        /// </summary>
        [TestMethod]
        public void should2()
        {
            // 查询价格不在100-200之间的，且名称包含'华为'或'手机'的产品
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
                filter: (q) =>
                {
                    return !q.Range(rd => rd.Field(info => info.skupricemin)
                        .GreaterThanOrEquals(100).LessThanOrEquals(200)) 
                        &&
                        (
                            q.Match(qd => qd.Field(info => info.title).Query("华为"))
                            ||
                            q.Match(qd => qd.Field(info => info.title).Query("手机"))
                        );
                }
            ).Result;
        }



        #endregion

        #region 排序

        /// <summary>
        /// sort_field: 按某个字段排序
        /// </summary>
        [TestMethod]
        public void sort_field()
        {
            // 按价格降序, filter:不计算分值
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName, from: 0, size: 100,
                filter: (q) => q.Match((qd) => qd.Field(info => info.title).Query("手机")),
                sort: (s) => s.Descending(info => info.marketpricemin)).Result;
        }

        #endregion

        #region 权重

        /// <summary>
        /// boost: 权重，按搜索词提升权重
        /// </summary>
        [TestMethod]
        public void Boost1()
        {
            var r1 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
                query:
                   q => q.Match(qm => qm.Field("title").Query("手机").Boost(2.0))
                        ||
                        q.Match(qm => qm.Field("title").Query("华为").Boost(1.0))
                        ).Result;
        }

        /// <summary>
        /// boost: 权重，按字段设置权重
        /// </summary>
        [TestMethod]
        public void Boost2()
        {
            var r1 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
                query:
                   q => q.Match(qm => qm.Field(info => info.title).Query("手机").Boost(2.0))
                        ||
                        q.Match(qm => qm.Field(info => info.spuname).Query("华为").Boost(1.0))
                        ).Result;
        }

        /// <summary>
        /// boost:权重，按字段设置权重，多字段搜索
        /// </summary>
        [TestMethod]
        public void Boost3()
        {
            var r1 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
                query:
                   q => q.MultiMatch
                   (
                       md => md.Fields(fd => fd
                            .Field(info => info.title, 2.0)
                            .Field(info => info.spuname, 1.0))
                            .Query("手机")
                   )
            ).Result;
        }

        #endregion

        #region 多字段查询

        /// <summary>
        /// should:多字段查询，评分规则
        /// </summary>
        [TestMethod]
        public void 多字段1()
        {
            // 评分规则: title + spuname
            var r1 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
                query:
                   q =>
                       q.Match(m => m.Field(info => info.title).Query("手机"))
                       ||
                       q.Match(m => m.Field(info => info.spuname).Query("手机"))
            ).Result;
        }

        /// <summary>
        /// should:多字段查询，评分规则
        /// </summary>
        [TestMethod]
        public void 多字段2()
        {
            // 评分规则: title 或 spuname，取分值最高的 + 其余字段 * 0.3
            var r1 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
                query:
                   q =>
                       q.MultiMatch(mq =>
                           mq.Fields(f => f
                               .Field(info => info.title)
                               .Field(info => info.spuname)
                            ).Query("手机")
                            .TieBreaker(0.3)
                )
            ).Result;

            // 评分规则: title + spuname
            var r2 = Proxy.SearchDocAsync<IndexDataInfo>(IndexName,
                query:
                   q =>
                       q.MultiMatch(mq =>
                           mq.Fields(f => f
                               .Field(info => info.title)
                               .Field(info => info.spuname)
                            ).Query("手机")
                            .Type(TextQueryType.MostFields)// 默认为 BestFields
                )
            ).Result;
        }

        #endregion
    }
}
