#region	Vesion Info
//======================================================================
//Copyright(C) FreshMan.All right reserved.
//命名空间：CodeBuilder.Framework.PhysicalDataModel
//文件名称：ReferenceModel
//创 建 人：FreshMan
//创建日期：2017/9/4 21:25:20
//用    途：记录类的用途
//======================================================================
#endregion

// ReSharper disable once CheckNamespace
namespace CodeBuilder.PhysicalDataModel
{
    /// <summary>
    /// Referenced model
    /// </summary>
    public class ReferencedModel
    {
        /// <summary>
        /// ForeignKey
        /// </summary>
        public string ForeignKey { get; set; }

        /// <summary>
        /// Table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// table name key cell
        /// </summary>
        public string ForeignKeyCell { get; set; }

        /// <summary>
        /// ReferencedTableName
        /// </summary>
        public string ReferencedTableName { get; set; }

        /// <summary>
        /// ReferencedCell
        /// </summary>
        public string ReferencedCell { get; set; }
    }
}
