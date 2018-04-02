using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using Nest;
using System.Linq;

namespace ElasticSearchTester.Tester
{
    [TestClass]
    public class AnalyzerTester : BaseTester
    {
        /// <summary>
        /// 测试分词器
        /// </summary>
        [TestMethod]
        public void TestAnalyzer()
        {
            // 带同义词（iphone8、iphone 8）
            var r1 = Proxy.AnalyzeAsync(IndexName, "ext_ik_max_word", "iphone8").Result;

            WriteTokens("ext_ik_max_word", r1.Tokens);

            var r2 = Proxy.AnalyzeAsync(IndexName, "ext_ik_smart", "iphone8").Result;

            WriteTokens("ext_ik_max_word", r2.Tokens);

            // 不带同义词
            var r3 = Proxy.AnalyzeAsync(IndexName, "ik_max_word", "叉烧包").Result;

            WriteTokens("ik_max_word", r3.Tokens);

            var r4 = Proxy.AnalyzeAsync(IndexName, "ik_smart", "叉烧包").Result;

            WriteTokens("ik_smart", r4.Tokens);
        }

        private void WriteTokens(string analyzer, IReadOnlyCollection<AnalyzeToken> tokens)
        {
            StringBuilder builder = new StringBuilder(analyzer);
            builder.Append(":");
            builder.Append(string.Join(",", tokens.Select(item => item.Token)));
            System.Diagnostics.Debug.WriteLine(builder.ToString());
        }
    }
}
