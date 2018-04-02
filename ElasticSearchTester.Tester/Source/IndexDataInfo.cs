using Nest;
using System;

namespace ElasticSearchTester.Tester.Source
{
    /// <summary>
    /// 商品搜索返回结果
    /// </summary>
    [ElasticsearchType(IdProperty = "spuid", Name = "info")]
    public class IndexDataInfo
    {
        /// <summary>
        /// SPUID
        /// </summary>              
        [Number(Index = true)]
        public long spuid
        {
            get; set;
        }

        /// <summary>
        /// 产品名称(spu名)
        /// </summary>    
        [Text(Index = true, Analyzer = "ik_max_word", SearchAnalyzer = "ext_ik_smart")]
        public string spuname
        {
            get; set;
        }

        /// <summary>
        /// 产品类型
        /// </summary>            
        [Number(Index = false)]
        public int prodtype
        {
            get; set;
        }

        /// <summary>
        /// 品牌id
        /// </summary>             
        [Number(Index = false)]
        public long brandid
        {
            get; set;
        }

        /// <summary>
        /// 品牌名称
        /// </summary>       
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ik_smart")]
        public string brandname { get; set; }

        /// <summary>
        /// 品牌英文名称
        /// </summary>      
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ik_smart")]
        public string brandsname { get; set; }


        /// <summary>
        /// 产品分类id,关联产品分类表prodsort
        /// </summary>             
        [Number(Index = false)]
        public long sortid
        {
            get; set;
        }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ext_ik_smart")]
        public string sortname { get; set; }

        /// <summary>
        /// 是否允许无理由退货:0否,1是
        /// </summary>          
        [Number(Index = false)]
        public int allowreturn
        {
            get; set;
        }

        /// <summary>
        /// 上架时间
        /// </summary>        
        [Date(Index = true)]
        public DateTime shelftime
        {
            get; set;
        }

        /// <summary>
        /// 最低sku价格
        /// </summary>           
        [Number(Index = true)]
        public decimal skupricemin
        {
            get; set;
        }

        /// <summary>
        /// 最低市场价格
        /// </summary>             
        [Number(Index = false)]
        public decimal marketpricemin
        {
            get; set;
        }

        /// <summary>
        /// 最大预估店主收益
        /// </summary>          
        [Number(Index = true)]
        public decimal maxstoreincome
        {
            get; set;
        }

        /// <summary>
        /// 商品标题
        /// </summary>      
        [Text(Index = true, Analyzer = "ik_max_word", SearchAnalyzer = "ext_ik_smart")]
        public string title
        {
            get; set;
        }

        /// <summary>
        /// 附加标题
        /// </summary>       
        [Text(Index = true, Analyzer = "ik_max_word", SearchAnalyzer = "ext_ik_smart")]
        public string attachtitle
        {
            get; set;
        }

        /// <summary>
        /// 标题前缀
        /// </summary>      
        [Text(Index = false)]
        public string prefix
        {
            get; set;
        }

        /// <summary>
        ///  SPU大图 横幅大图
        /// </summary>      
        [Text(Index = false)]
        public string SPUlargerPic
        {
            get; set;
        }

        /// <summary>
        ///  SPU白底图 列表商品图
        /// </summary>     
        [Text(Index = false)]
        public string SPUwhitePic
        {
            get; set;
        }

        /// <summary>
        /// 库存量
        /// </summary>              
        [Number(Index = false)]
        public long skustockqty
        {
            get; set;
        }

        /// <summary>
        ///  状态:8：已上架
        /// </summary>             
        [Number(Index = false)]
        public int status
        {
            get; set;
        }

        /// <summary>
        /// 搜索关键词
        /// </summary>            
        [Text(Index = true, Analyzer = "ik_smart", SearchAnalyzer = "ext_ik_smart")]
        public string keyword
        {
            get; set;
        }

        /// <summary>
        /// 提取所有参与搜索的成员
        /// </summary>
        [Text(Index = true, Analyzer = "ik_max_word", SearchAnalyzer = "ext_ik_smart")]
        public string all { get; set; }

        /// <summary>
        /// 删除标识,0:未删除；1:已删除（备用）
        /// </summary>     
        [Number(Index = true)]
        public int delflag
        {
            get; set;
        }

        /// <summary>
        /// 允许销售区域，在“SALES_AREA”字段中填充，如果没有，则填充0（全部可售）
        /// </summary>
        [Number(Index = true)]
        public int[] salesareas { get; set; }

        /// <summary>
        /// 不允许销售区域，在“NO_SALES_AREA”字段中填充，如果没有，则填充0（全部可售）
        /// </summary>
        [Number(Index = true)]
        public int[] nosalesareas { get; set; }

        /// <summary>
        /// 销量
        /// </summary>
        [Number(Index = true)]
        public int sale_quantity { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
        [Number(Index = true)]
        public int browse_num { get; set; }
    }
}
