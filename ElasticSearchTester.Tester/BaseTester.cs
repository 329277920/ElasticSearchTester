using ElasticSearchTester.Tester.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticSearchTester.Tester
{
    public abstract class BaseTester
    {
        protected virtual ElasticSearchProxy Proxy
        {
            get { return ElasticSearchProxy.CreateProxy("http://192.168.2.65:9200"); }
        }

        /// <summary>
        /// 索引名称，只能小写
        /// </summary>
        protected virtual string IndexName { get { return "testindex"; } }

        protected virtual string TypeName { get { return "testtype"; } }
        
    }
}
