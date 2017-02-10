using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Lfz.Collections;
using Lfz.Logging;

namespace Lfz.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRawSqlRepository<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        T GetFirstOrDefault(string whereClause="");
        /// <summary>
        /// 日志提供接口
        /// </summary>
        ILogger Logger { get; set; }
            

        IEnumerable<T> FormatModel(DataSet reader, int count);

        T FormatModel(DataRow reader );
          
        /// <summary>
        /// 分页数据获取
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IPageOfItems<T> GetPaged(string whereClause, string fields, string orderBy, int pageIndex, int pageSize);

        /// <summary>
        /// 根据Id获取实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetModel(int id);

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereClause"></param> 
        /// <param name="orderBy"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<T> GetList(string whereClause, string orderBy, int count);

        /// <summary>
        /// 记录条数获取
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        int Count(string whereClause);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        bool Exists(string whereClause);

        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        bool Exists(string whereClause, SqlTransaction transaction);

        /// <summary>
        /// 表: 的添加方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="model">ArmlenoperateModel 实例</param> 
        /// <returns>返回结果：0(添加错误)，>0(添加成功)</returns>
        int Create(T model);

        /// <summary>
        /// 表: 的添加方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="model">ArmlenoperateModel 实例</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <returns>返回结果：0(添加错误)，>0(添加成功)</returns>
        int Create(T model, SqlTransaction transaction);

        /// <summary>
        /// 表:T的修改方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="model">T 实例</param> 
        /// <returns>返回结果：0(修改错误)，>0(修改成功)</returns>
        int Update(T model);

        /// <summary>
        /// 更新摸个字段值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fieldname"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        int Update(int id,string fieldname,object fieldValue);

        /// <summary>
        /// 表:T 的修改方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="model">T 实例</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <returns>返回结果：0(修改错误)，>0(修改成功)</returns>
        int Update(T model, SqlTransaction transaction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(int id);

        /// <summary>
        /// 表:d_ArmLenOperate的删除方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="id">主键ID</param> 
        /// <param name="transaction">数据库事务对象</param>
        /// <returns>返回结果：0(删除错误)，>0(删除成功)</returns>
        int Delete(int id, SqlTransaction transaction);

        /// <summary>
        /// 根据黑匣子编号删除记录
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <param name="transaction"></param>
        /// <param name="fieldname"></param>
        void DeleteByField<TElement>(string fieldname, T fieldValue, SqlTransaction transaction);

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        void Execute(string sqlClause);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string sqlClause, params SqlParameter[] commandParameters);
    }
}