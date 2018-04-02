using Elasticsearch.Net;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Tester.Core
{
    public class ElasticSearchProxy
    {
        private ElasticSearchProxy(ElasticClient client)
        {
            this.Client = client;
        }              

        /// <summary>
        /// 获取直接访问es的客户端引用
        /// </summary>
        public ElasticClient Client { get; private set; }

        /// <summary>
        /// 创建ElasticSearch代理客户端
        /// </summary>
        /// <param name="uri">地址集合，如果有集群有多个地址，则指定多个，客户端实现负载与故障转移</param>
        /// <returns>返回代理对象</returns>
        public static ElasticSearchProxy CreateProxy(params string[] uri)
        {
            if (uri == null || uri.Length <= 0)
            {
                throw new ArgumentNullException("uris");
            }
            return new ElasticSearchProxy(CreateElasticClient(uri));
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="settings">索引配置参数</param>
        /// <returns>返回创建结果</returns>
        public async Task<ICreateIndexResponse> CreateIndexAsync(string indexName, Dictionary<string, object> settings)
        {
            return await Client.CreateIndexAsync(indexName, d =>
            {
                return d.Settings(s =>
                {
                    foreach (var kv in settings)
                    {
                        s.Setting(kv.Key, kv.Value);
                    }
                    return s;
                });
            });
        }

        /// <summary>
        /// 更新索引配置
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="settings">配置信息</param>
        /// <returns>返回更新结果</returns>
        public Task<IUpdateIndexSettingsResponse> UpdateIndex(string indexName, Dictionary<string, object> settings)
        {
            return Client.UpdateIndexSettingsAsync(indexName, d =>
            {
                return d.IndexSettings(s =>
                {
                    foreach (var kv in settings)
                    {
                        s.Setting(kv.Key, kv.Value);
                    }
                    return s;
                });
            });
        }

        /// <summary>
        /// 获取索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <returns>返回索引信息</returns>
        public async Task<IGetIndexResponse> GetIndexAsync(string indexName)
        {
            return await Client.GetIndexAsync(indexName);
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <returns>返回删除结果</returns>
        public async Task<IDeleteIndexResponse> DeleteIndexAsync(string indexName)
        {
            return await Client.DeleteIndexAsync(indexName);
        }

        /// <summary>
        /// 关闭索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <returns></returns>
        public Task<ICloseIndexResponse> CloseIndexAsync(string indexName)
        {
            return Client.CloseIndexAsync(indexName);
        }

        /// <summary>
        /// 打开索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <returns></returns>
        public Task<IOpenIndexResponse> OpenIndexAsync(string indexName)
        {
            return Client.OpenIndexAsync(indexName);
        }

        /// <summary>
        /// 在索引中创建一个映射
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="indexName">索引名称</param>
        /// <returns>返回添加结果</returns>
        public async Task<IPutMappingResponse> MappingAsync<T>(string indexName)
            where T : class
        {
            return await Client.MapAsync<T>(d =>
            {
                return d.Index(indexName).Type<T>().AutoMap();
            });
        }

        /// <summary>
        /// 在索引中添加或更新一个文档
        /// </summary>        
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="document">文档对象</param>
        /// <returns>返回添加结果</returns>
        public async Task<IIndexResponse> UpsertDocAsync<T>(string indexName, T document)
            where T : class
        {
            return await Client.IndexAsync(document, d => { return d.Index(indexName); });
        }

        /// <summary>
        /// 在索引中添加或更新多个文档
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="docs">文档列表</param>     
        public async Task<IBulkResponse> UpsertDocManyAsync<T>(string indexName, IEnumerable<T> docs)
            where T : class
        {
            return await Client.BulkAsync(d =>
            {
                return d.Index(indexName).IndexMany(docs);
            });
        }

        /// <summary>
        /// 通过Id删除文档
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="id">主键Id</param>
        /// <returns>返回删除结果</returns>
        public async Task<IDeleteResponse> DeleteDocAsync<T>(string indexName, string id)
            where T : class
        {
            return await Client.DeleteAsync<T>(id, d => { return d.Index(indexName); });
        }

        /// <summary>
        /// 批量删除文档
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="docs">文档列表</param>
        /// <returns>返回删除结果</returns>
        public async Task<IBulkResponse> DeleteDocManyAsync<T>(string indexName, IEnumerable<T> docs)
            where T : class
        {
            return await Client.BulkAsync(d =>
            {
                return d.Index(indexName).DeleteMany<T>(docs);
            });
        }

        /// <summary>
        /// 通过Id查找文档
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="id">主键Id</param>
        /// <returns>返回文档对象</returns>
        public async Task<T> GetDocAsync<T>(string indexName, string id)
            where T : class
        {
            var rest = await Client.GetAsync<T>(id, d => { return d.Index(indexName); });
            if (!rest.Found)
            {
                return null;
            }
            return rest.Source;
        }

        /// <summary>
        /// 搜索文档
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="indexName">索引名称</param>
        /// <param name="typeName">类型名称，默认从T类型的特性中读取</param>
        /// <param name="from">搜索开始序号</param>
        /// <param name="size">返回数据条数</param>
        /// <param name="query">搜索条件设置</param>
        /// <param name="selector">查询条件设置</param>
        /// <param name="suggest">suggest查询</param>
        /// <param name="filter">filter查询</param>
        /// <returns>返回搜索结果</returns>
        public Task<ISearchResponse<T>> SearchDocAsync<T>(string indexName, 
            string typeName = null, int from = 0, int? size = null, 
            Func<QueryContainerDescriptor<T>, QueryContainer> query = null, 
            Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null, 
            Func<SuggestContainerDescriptor<T>, IPromise<ISuggestContainer>> suggest = null,
            Func<QueryContainerDescriptor<T>, QueryContainer> filter = null)
            where T : class
        {
            return Client.SearchAsync<T>(d =>
            {
                var req = d.Index(indexName).From(from);
                if (!string.IsNullOrEmpty(typeName))
                {
                    req = req.Type(typeName);
                }
                if (size.HasValue)
                {
                    req.Size(size.Value);
                }
                if (query != null)
                {
                    req.Query(query);
                }
                if (sort != null)
                {
                    req.Sort(sort);
                }
                if (suggest != null)
                {
                    req.Suggest(suggest);
                }
                if (filter != null)
                {
                    req.PostFilter(filter);
                }
                return req;
            });
        }

        /// <summary>
        /// 检测分词结果
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="analyzer">分词器</param>
        /// <param name="content">需检测的文本</param>
        /// <returns>返回分词结果</returns>
        public Task<IAnalyzeResponse> AnalyzeAsync(string indexName, string analyzer, params string[] content)
        {
            return Client.AnalyzeAsync(d =>
            {
                return d.Index(indexName).Analyzer(analyzer).Text(content);
            });
        }

        /// <summary>
        /// 检测分词结果
        /// </summary>
        /// <param name="indexName">索引名称</param>
        /// <param name="field">字段</param>
        /// <param name="content">需检测的文本</param>
        /// <returns>返回分词结果</returns>
        public async Task<IAnalyzeResponse> AnalyzeAsync<T>(string indexName, Expression<Func<T, object>> field, params string[] content)
        {
            return await Client.AnalyzeAsync(d =>
            {
                return d.Index(indexName).Field<T>(field).Text(content);
            });
        }

        #region 私有成员

        private static ElasticClient CreateElasticClient(params string[] uri)
        {
            if (uri.Length > 1)
            {
                return new ElasticClient(
                    new ConnectionSettings(new StaticConnectionPool(
                        from item in uri select new Uri(item))));
            }
            return new ElasticClient(new ConnectionSettings(new Uri(uri[0])));
        }

        #endregion
    }
}
