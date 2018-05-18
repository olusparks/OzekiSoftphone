using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SharedProject
{
    public static class RouteToDB
    {
        private static string connectionString = string.Empty;

        public static string ConnectionToOzekiDB()
        {
            connectionString = GetAppPath(@"\OzekiDB.sdf");
            //ApplicationLog.WriteEventToLog("Path to Ozeki.sdf: " + connectionString);
            return connectionString;
            // dbContext = new ZoiperdbContext(connectionString);
        }

        public static string ConnectionToZoiperDB()
        {
            connectionString = GetAppPath(@"\Zoiperdb.sdf");
            //ApplicationLog.WriteEventToLog("Path to Zoiperdb.sdf: " + connectionString);
            return connectionString;
            // dbContext = new ZoiperdbContext(connectionString);
        }

        private static string GetAppPath(string imagePath)
        {
            string Dir2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path2 = Dir2 + imagePath;
            return path2;
        }
    }
}
