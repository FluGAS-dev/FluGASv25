using FluGASv25.Models.Properties;
using System.Linq;

namespace FluGASv25.Dao
{
    class ReferenceDao
    {
        private const string TableName = "REFERENCE";

        // 1-insert.
        public static long InsertReferenceTable(Reference s)
        {
            var withoutClm = new string[] { "id" };  // Sample登録で ID は AutoInclimentだから指定しない。
            long insertId = DbCommon.InsertTable(TableName, s, withoutClm);
            return insertId;
        }

        public static Reference[] GetParameters()
        {

            var allData = DbCommon.SelectTableAll(
                                                        TableName,
                                                        typeof(Reference));
            // 初期インストール時の 救済処置
            if (allData == null || allData.Count < 1)
            {
                var p = new Reference
                {
                    ID = 0
                };
                InsertReferenceTable(p);
                var ps = DbCommon.SelectTableAll(TableName, typeof(Reference));
                allData = ps;
            }

            var list = allData.Select(s => s)
                                        .Cast<Reference>()
                                        .OrderBy(s => s.ID)
                                        .ToArray();
            // 初期インストール時の 救済処置
            if (allData == null || ! allData.Any())
            {
                var p = new Reference
                {
                    ID = 1,
                };
                InsertReferenceTable(p);
                var ps = DbCommon.SelectTableAll(TableName, typeof(Reference));
                allData = ps;
            }

            // 必ず1件は在るはず（初期データベースに入れている）
            return list ;
        }


        public static Reference GetDefalutParameter()
        {
            // 必ず1件は在るはず（初期データベースに入れている）
            var list = GetParameters();
            return (Reference)list.First();
        }

        public static Reference GetLastParameter()
        {
            // 必ず1件は在るはず（初期データベースに入れている）
            var list = GetParameters();
            return (Reference)list.Last();
        }

    }
}
