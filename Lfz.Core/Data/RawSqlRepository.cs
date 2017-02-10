/*======================================================================
 * 
 *        Copyright (C)  1996-2012  杭州品茗物联网平台事业部    
 *        All rights reserved
 *
 *        Filename : RawSqlRepository
 *        DESCRIPTION : 数据访问访问层基类
 *
 *        Created By 林芳崽 at  2013-01-09 11:15:01
 *        http://www.pinming.cn/
 *
 *======================================================================*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Lfz.Collections;
using Lfz.Logging;
using Lfz.Utitlies;

namespace Lfz.Data
{
    /// <summary>
    /// 数据访问访问层基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RawSqlRepository<T> : IRawSqlRepository<T> where T : class
    {
        /// <summary>
        /// 日志提供接口
        /// </summary>
        public ILogger Logger { get; set; }


        public readonly string ConnectionString;
         

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionStr"></param>
        protected RawSqlRepository(string connectionStr)
        {
            ConnectionString = connectionStr;
            Logger = LoggerFactory.GetLog();
        }

        /// <summary>
        /// 初始化添加更新参数
        /// </summary> 
        /// <param name="model"></param>
        /// <param name="isUpdate"></param>
        public abstract SqlParameter[] GetParameters(T model, bool isUpdate);

        /// <summary>
        /// 表名
        /// </summary>
        public abstract string TableName { get; }

        /// <summary>
        /// 主键名称
        /// </summary>
        public abstract string PrimaryKeyName { get; }

        /// <summary>
        /// Update 语句 UPDATE $TableName$ ..
        /// </summary>
        public abstract string UpdateSqlClause { get; }

        /// <summary>
        /// INSERT 语句 INSERT $TableName$(,,)VALUES(,,)
        /// </summary>
        public abstract string CreateSqlClause { get; }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract T FormatModel(DataRow reader);

        /// <summary>
        /// 分页数据获取
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="fields"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IPageOfItems<T> GetPaged(string whereClause, string fields, string orderBy, int pageIndex, int pageSize)
        {
            whereClause = whereClause.Replace("'", "''");
            var pageOfItems = new PageOfItems<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            //var sqlClause =
            //    string.Format("EXEC [dbo].[up_GetPaged] '{0}','{1}' ,'{2}','{3}','{4}',{5},{6}", 
            //    TableName, fields, whereClause, orderBy, PrimaryKeyName, pageIndex, pageSize);


            var parameters = new[] {
                new SqlParameter("@TableName", SqlDbType.NVarChar, 255),
                new SqlParameter("@Fields", SqlDbType.NVarChar, 255),
                new SqlParameter("@WhereClause", SqlDbType.NVarChar, 2000),
                new SqlParameter("@OrderBy", SqlDbType.NVarChar, 255),
                new SqlParameter("@PrimaryKey", SqlDbType.NVarChar, 255),
                new SqlParameter("@PageIndex", SqlDbType.Int, 4),
                new SqlParameter("@PageSize", SqlDbType.Int, 4),
            };
            parameters[0].Value = TableName;
            parameters[1].Value = string.IsNullOrEmpty(fields) ? "*" : fields;
            parameters[2].Value = whereClause??string.Empty;
            parameters[3].Value = orderBy ?? string.Empty;
            parameters[4].Value = PrimaryKeyName;
            parameters[5].Value = pageIndex;
            parameters[6].Value = pageSize;
            using (var reader = DbHelperSql.RunProcedure(ConnectionString, "up_GetPaged", parameters))
            {
                var list = FormatModel(reader, pageSize).ToReadOnlyCollection();
                if (reader.Tables.Count >= 2 && reader.Tables[1].Rows.Count > 0)
                {
                    var row = reader.Tables[1].Rows[0];
                    if (row["TotalRowCount"] == DBNull.Value) pageOfItems.TotalItemCount = 0; else pageOfItems.TotalItemCount = (Int32)row["TotalRowCount"];
                }
                pageOfItems.AddRange(list);
            }
            return pageOfItems;

            //using (var reader = ExecuteDataSet(sqlClause))
            //{
            //    var list = FormatModel(reader, pageSize).ToReadOnlyCollection();
            //    if (reader.Tables.Count >= 2 && reader.Tables[1].Rows.Count > 0)
            //    {
            //        var row = reader.Tables[1].Rows[0];
            //        if (row["TotalRowCount"] == DBNull.Value) pageOfItems.TotalItemCount = 0; else pageOfItems.TotalItemCount = (Int32)row["TotalRowCount"];
            //    }
            //    pageOfItems.AddRange(list);
            //}
            //return pageOfItems;
        }

        /// <summary>
        /// 根据Id获取实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetModel(int id)
        {
            var list = GetList(string.Format("{0}='{1}'", PrimaryKeyName, id), PrimaryKeyName, 1);
            return list.FirstOrDefault();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlClause"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object GetSingle(string sqlClause, params SqlParameter[] parameters)
        {
            return DbHelperSql.GetSingle(ConnectionString, sqlClause, parameters);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public T GetFirstOrDefault(string whereClause = "")
        {
            return GetList(whereClause, string.Empty, 1).FirstOrDefault();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="whereClause"></param> 
        /// <param name="orderBy"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<T> GetList(string whereClause, string orderBy, int count)
        {
            var topClause = "";
            if (!string.IsNullOrEmpty(whereClause)) whereClause = " WHERE " + whereClause;
            if (!string.IsNullOrEmpty(orderBy)) orderBy = " ORDER BY " + orderBy;
            if (count > 0) topClause = string.Format(" TOP {0} ", count);
            var sqlClause = string.Format("SELECT {1} * FROM {0} {2} {3}", TableName, topClause, whereClause, orderBy);
            using (var reader = ExecuteDataSet(sqlClause))
            {
                return FormatModel(reader, count);
            }
        }

        /// <summary>
        /// 记录条数获取
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public int Count(string whereClause)
        {
            if (!string.IsNullOrEmpty(whereClause)) whereClause = string.Format(" WHERE {0}", whereClause);
            var sqlClase = string.Format("SELECT COUNT(*) FROM {0} {1}", TableName, whereClause);
            return TypeParse.StrToInt(DbHelperSql.GetSingle(ConnectionString, sqlClase));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public bool Exists(string whereClause)
        {
            return Exists(whereClause, null);
        }

        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public bool Exists(string whereClause, SqlTransaction transaction)
        {
            if (!string.IsNullOrEmpty(whereClause)) whereClause = string.Format("WHERE {0}", whereClause);
            var sqlClase = string.Format("IF EXISTS(SELECT 1 FROM {0} {1}) SELECT 1 ELSE SELECT 0", TableName, whereClause);
            return TypeParse.StrToInt(DbHelperSql.GetSingle(ConnectionString, sqlClase)) == 1;
        }

        /// <summary>
        /// 表: 的添加方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="model">ArmlenoperateModel 实例</param> 
        /// <returns>返回结果：0(添加错误)，>0(添加成功)</returns>
        public virtual int Create(T model)
        {
            return Create(model, null);
        }

        /// <summary>
        /// 表: 的添加方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="model">ArmlenoperateModel 实例</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <returns>返回结果：0(添加错误)，>0(添加成功)</returns>
        public int Create(T model, SqlTransaction transaction)
        {

            var sqlClause = string.Format(CreateSqlClause, TableName);
            var parameters = GetParameters(model, false);
            return
                 DbHelperSql.ExecuteSql(ConnectionString, sqlClause, parameters);
        }

        /// <summary>
        /// 表:T的修改方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="model">T 实例</param> 
        /// <returns>返回结果：0(修改错误)，>0(修改成功)</returns>
        public virtual int Update(T model)
        {
            return Update(model, null);
        }

        public virtual int Update(int id, string fieldname, object fieldValue)
        {
            var sqlClause = string.Format("UPDATE SET {2}='{3}' {0} WHERE {1} =@Id", TableName, PrimaryKeyName, fieldname, fieldValue);
            var parameters = new[] { new SqlParameter("@Id", SqlDbType.Int, 4) };
            parameters[0].Value = id;
            return
                DbHelperSql.ExecuteSql(ConnectionString, sqlClause, parameters);
        }

        /// <summary>
        /// 表:T 的修改方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="model">T 实例</param>
        /// <param name="transaction">数据库事务对象</param>
        /// <returns>返回结果：0(修改错误)，>0(修改成功)</returns>
        public int Update(T model, SqlTransaction transaction)
        {
            var sqlClause = string.Format(UpdateSqlClause, TableName);
            var parameters = GetParameters(model, true);
            return
                DbHelperSql.ExecuteSql(ConnectionString, sqlClause, parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual int Delete(int id)
        {
            return Delete(id, null);
        }

        /// <summary>
        /// 表:d_ArmLenOperate的删除方法｜｜表描述：臂长操作
        /// </summary>
        /// <param name="id">主键ID</param> 
        /// <param name="transaction">数据库事务对象</param>
        /// <returns>返回结果：0(删除错误)，>0(删除成功)</returns>
        public int Delete(int id, SqlTransaction transaction)
        {
            var sqlClause = string.Format("DELETE FROM {0} WHERE {1} =@Id", TableName, PrimaryKeyName);
            var parameters = new[] { new SqlParameter("@Id", SqlDbType.Int, 4) };
            parameters[0].Value = id;
            return
                DbHelperSql.ExecuteSql(ConnectionString, sqlClause, parameters);
        }


        /// <summary>
        /// 根据黑匣子编号删除记录
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <param name="transaction"></param>
        /// <param name="fieldname"></param>
        public void DeleteByField<TElement>(string fieldname, T fieldValue, SqlTransaction transaction)
        {
            var sqlClause = string.Format("DELETE FROM {0} WHERE {1} =@{1}", TableName, fieldname);
            SqlDbType sqlDbType = SqlDbType.BigInt;
            int size = 0;
            if (typeof(TElement) == typeof(int))
            {
                sqlDbType = SqlDbType.Int;
                size = 4;
            }
            else if (typeof(TElement) == typeof(Int64))
            {
                sqlDbType = SqlDbType.BigInt;
                size = 8;
            }
            else if (typeof(TElement) == typeof(Guid))
            {
                sqlDbType = SqlDbType.UniqueIdentifier;
                size = 16;
            }
            else if (typeof(TElement) == typeof(DateTime))
            {
                sqlDbType = SqlDbType.DateTime;
                size = 16;
            }
            else if (typeof(TElement) == typeof(string))
            {
                sqlDbType = SqlDbType.NVarChar;
                size = (fieldValue.ToString()).Length;
            }
            else return;

            var parameters = new[] { new SqlParameter(string.Format("@{0}", fieldValue), sqlDbType, size) };
            parameters[0].Value = fieldValue;
            DbHelperSql.ExecuteSql(ConnectionString, sqlClause, parameters);
        }

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
        public object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                    object val = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if (val == DBNull.Value) return null;
                    return val;
                }
            }
        }

        public void Execute(string sqlClause)
        {
            DbHelperSql.ExecuteSql(this.ConnectionString, sqlClause);
        }

        public DataSet ExecuteDataSet(string sqlClause, params SqlParameter[] commandParameters)
        {
            return DbHelperSql.Query(this.ConnectionString, sqlClause, commandParameters);
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        protected void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlType"></param>
        /// <returns></returns>
        public SqlDbType GetFieldType(string sqlType)
        {
            SqlDbType type = SqlDbType.NVarChar;
            var names = Enum.GetNames(typeof(SqlDbType));
            var currentName = names.FirstOrDefault(x => x.Equals(sqlType, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(currentName))
            {
                type = (SqlDbType)Enum.Parse(typeof(SqlDbType), currentName);
            }
            return type;
        }

        public IEnumerable<T> FormatModel(DataSet reader, int count)
        {
            List<T> result = new List<T>();
            if (reader != null && reader.Tables.Count > 0 && reader.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < reader.Tables[0].Rows.Count; i++)
                {
                    var value = FormatModel(reader.Tables[0].Rows[i]);
                    result.Add(value);
                }
            }
            return result;
        }

    }
}