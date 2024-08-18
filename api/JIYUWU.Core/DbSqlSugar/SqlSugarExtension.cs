﻿using JIYUWU.Core.Common;
using JIYUWU.Core.Extension;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JIYUWU.Core.DbSqlSugar
{
    public static class SqlSugarExtension
    {

        public static int Add<T>(this MyDbContext dbContext, T table, bool saveChange = false) where T : class, new()
        {
            dbContext.SqlSugarClient.Insertable(table).AddQueue();
            if (saveChange)
            {
                return dbContext.SqlSugarClient.SaveQueues();
            }
            return 1;
        }

        public static int Add<T>(this ISqlSugarClient sqlSugarClient, T table, bool saveChange = false) where T : class, new()
        {
            sqlSugarClient.Insertable(table).AddQueue();
            if (saveChange)
            {
                return sqlSugarClient.SaveQueues();
            }
            return 1;
        }

        public static async Task<int> AddAsync<T>(this MyDbContext dbContext, T list, bool saveChange = false) where T : class, new()
        {
            dbContext.SqlSugarClient.Insertable(list).AddQueue();
            if (saveChange)
            {
                return await dbContext.SqlSugarClient.SaveQueuesAsync();
            }
            return 1;
        }

        public static int AddRange<T>(this MyDbContext dbContext, List<T> list, bool saveChange = false) where T : class, new()
        {
            if (typeof(T).GetSugarSplitTable() != null)
            {
                dbContext.SqlSugarClient.Insertable(list).SplitTable().ExecuteCommand();
                return list.Count;
            }
            dbContext.SqlSugarClient.Insertable(list).AddQueue();
            if (saveChange)
            {
                return dbContext.SqlSugarClient.SaveQueues();
            }
            return list.Count;
        }

        public static async Task<int> AddRangeAsync<T>(this MyDbContext dbContext, List<T> list, bool saveChange = false) where T : class, new()
        {
            if (typeof(T).GetSugarSplitTable() != null)
            {
                await dbContext.SqlSugarClient.Insertable(list).SplitTable().ExecuteCommandAsync();
                return list.Count;
            }
            dbContext.SqlSugarClient.Insertable(list).AddQueue();
            if (saveChange)
            {
                return await dbContext.SqlSugarClient.SaveQueuesAsync();
            }
            return list.Count;
        }
        public static int Update<TSource>(this MyDbContext dbContext, TSource entity, bool saveChanges = false) where TSource : class, new()
        {
            return UpdateRange<TSource>(dbContext, new List<TSource>() { entity }, new string[] { }, saveChanges);
        }
        public static int Update<TSource>(this MyDbContext dbContext, TSource entity, Expression<Func<TSource, object>> updateMainFields, bool saveChanges = false) where TSource : class, new()
        {
            return UpdateRange<TSource>(dbContext, new List<TSource>() { entity }, updateMainFields.GetExpressionProperty(), saveChanges);
        }

        public static int Update<TSource>(this MyDbContext dbContext, TSource entity, string[] properties, bool saveChanges = false) where TSource : class, new()
        {
            return UpdateRange<TSource>(dbContext, new List<TSource>() { entity }, properties, saveChanges);
        }
        public static int UpdateRange<TSource>(this MyDbContext dbContext, IEnumerable<TSource> entities, bool saveChanges = false) where TSource : class, new()
        {
            return UpdateRange<TSource>(dbContext, entities, new string[] { }, saveChanges);
        }
        public static int UpdateRange<TSource>(this MyDbContext dbContext, IEnumerable<TSource> entities, Expression<Func<TSource, object>> updateMainFields, bool saveChanges = false) where TSource : class, new()
        {
            return UpdateRange<TSource>(dbContext, entities, updateMainFields.GetExpressionProperty(), saveChanges);
        }
        public static int UpdateRange<TSource>(this MyDbContext dbContext, IEnumerable<TSource> entities, string[] properties, bool saveChanges = false) where TSource : class, new()
        {
            return dbContext.SqlSugarClient.UpdateRange<TSource>(entities, properties, saveChanges);
        }
        public static int Update<TSource>(this ISqlSugarClient sqlSugarClient, TSource entity, string[] properties, bool saveChanges = false) where TSource : class, new()
        {
            return sqlSugarClient.UpdateRange<TSource>(new List<TSource>() { entity }, properties, saveChanges);
        }

        //public static int Update<TSource>(this SqlSugarScope sqlSugarScope, TSource entity, string[] properties, bool saveChanges = false) where TSource : class, new()
        //{
        //    return sqlSugarScope.UpdateRange<TSource>(new List<TSource>() { entity }, properties, saveChanges);
        //}

        public static int UpdateRange<TSource>(this ISqlSugarClient sqlSugarClient, IEnumerable<TSource> entities, string[] properties, bool saveChanges = false) where TSource : class, new()
        {
            if (entities.Count() == 0)
            {
                return 0;
            }
            if (properties != null && properties.Length > 0)
            {
                PropertyInfo[] entityProperty = typeof(TSource).GetProperties();
                string keyName = entityProperty.GetKeyName();
                if (properties.Contains(keyName))
                {
                    properties = properties.Where(x => x != keyName).ToArray();
                }
                properties = properties.Where(x => entityProperty.Select(s => s.Name).Contains(x)).ToArray();
            }
            bool splitTable = typeof(TSource).GetSugarSplitTable() != null;
            IUpdateable<TSource> updateable = null;
            if (properties == null || properties.Length == 0)
            {
                updateable = sqlSugarClient.Updateable<TSource>(entities.ToList());//.AddQueue();
            }
            else
            {
                updateable = sqlSugarClient.Updateable<TSource>(entities.ToList()).UpdateColumns(properties);//.AddQueue();
            }
            if (splitTable)
            {
                updateable.SplitTable().ExecuteCommand();
                return entities.Count();
            }
            updateable.AddQueue();
            if (!saveChanges)
            {
                return 0;
            }
            return sqlSugarClient.SaveQueues();
        }


        public static Task<T> FirstOrDefaultAsync<T>(this ISugarQueryable<T> queryable)
        {
            return queryable.FirstAsync();
        }

        public static T FirstOrDefault<T>(this ISugarQueryable<T> queryable)
        {
            return queryable.First();
        }

        public static ISugarQueryable<T> Include<T, TProperty>(this ISugarQueryable<T> queryable, Expression<Func<T, TProperty>> incluedProperty)
        {
            return queryable.Includes(incluedProperty);
        }

        public static T First<T>(this ISugarQueryable<T> queryable)
        {
            return queryable.First();
        }

        public static ISugarQueryable<T> ThenByDescending<T>(this ISugarQueryable<T> queryable, Expression<Func<T, object>> expression)
        {
            return queryable.OrderByDescending(expression);
        }


        public static int SaveChanges(this ISqlSugarClient sqlSugarClient)
        {
            return sqlSugarClient.SaveQueues();
        }

        public static async Task<int> SaveChangesAsync(this ISqlSugarClient sqlSugarClient)
        {
            return await sqlSugarClient.SaveQueuesAsync();
        }


        public static ISugarQueryable<TEntity> Set<TEntity>(this ISqlSugarClient sqlSugarClient, bool filterDeleted = false) where TEntity : class
        {
            var query = sqlSugarClient.Queryable<TEntity>();
            if (filterDeleted && !string.IsNullOrEmpty(AppSetting.LogicDelField))
            {
                if (typeof(TEntity).GetProperty(AppSetting.LogicDelField) != null)
                {
                    var expression = AppSetting.LogicDelField.CreateExpression<TEntity>((int)DelStatus.正常, LinqExpressionType.Equal);
                    return query.Where(expression);
                }
            }
            return query;
        }

        public static List<T> QueryList<T>(this ISqlSugarClient sqlSugarClient, string sql, object parameters)
        {
            return sqlSugarClient.Ado.SqlQuery<T>(sql, parameters);
        }
        public static object ExecuteScalar(this ISqlSugarClient sqlSugarClient, string sql, object parameters)
        {
            return sqlSugarClient.Ado.GetScalar(sql, parameters);
        }
        public static int ExcuteNonQuery(this ISqlSugarClient sqlSugarClient, string sql, object parameters)
        {
            return sqlSugarClient.Ado.ExecuteCommand(sql, parameters);
        }
        public static ISqlSugarClient SetTimout(this ISqlSugarClient sqlSugarClient, int time)
        {
            // sqlSugarClient.Ado.CommandTimeOut = time;
            return sqlSugarClient;
        }


    }
}
