using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using BilibiliSpider.Entity.Database;
using BilibiliSpider.Entity.Database;
using ServiceStack.OrmLite;

namespace BilibiliSpider.DB
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class DBSet
    {
        // Database='mysql';Data Source=localhost;password=1qazZAQ!;User ID=root;Port=3306;SslMode=None;Allow User Variables=True;AllowPublicKeyRetrieval=True
        private static string sqlConnectFormatter = "Database='{0}';Data Source={1};password={3};User ID={2};Port=3306;SslMode=None;Allow User Variables=True;AllowPublicKeyRetrieval=True";

        private static OrmLiteConnectionFactory factory =
            new OrmLiteConnectionFactory("", MySqlDialect.Provider, false);

        static DBSet()
        {
            OrmLiteConfig.DialectProvider = MySqlDialect.Provider;
        }

        /// <summary>
        /// 获得数据库连接
        /// </summary>
        public static IDbConnection GetCon(SqliteDBName nameType)
        {
            var databaseName = nameType.ToString();

            if (!OrmLiteConnectionFactory.NamedConnections.ContainsKey(databaseName))
            {
                var connStr = string.Format(sqlConnectFormatter, databaseName, "192.168.1.60", "root", "");
                factory.RegisterConnection(databaseName, connStr, MySqlDialect.Provider);
            }

            return factory.Open(databaseName);
        }

        public static void Init()
        {
            using (var db = GetCon(SqliteDBName.Bilibili))
            {
                db.CreateTable<AV>();
                db.CreateTable<UP>();
                db.CreateTable<Tag>();
                // db.CreateTable<ImageDetect>();
                db.CreateTable<ImageTag>();
            }

            //using (var db = GetCon(SqliteDBName.History))
            //{
            //    db.CreateTable<AVHistory>();
            //}
        }

        /// <summary>
        /// 用来保存数据sqlite的主名字
        /// </summary>
        public enum SqliteDBName
        {
            /// <summary>
            /// b站主数据名字
            /// 注意，不要随意修改
            /// </summary>
            Bilibili,

            /// <summary>
            /// 用来放采集历史的
            /// </summary>
            History,
        }
    }
}
