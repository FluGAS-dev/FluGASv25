using FluGASv25.Models.Properties;
using System.Collections.Generic;
using System.Linq;

namespace FluGASv25.Dao
{
    public class SampleDao
    {
        private const string TableName = "SAMPLE";

        // 1-insert.
        public static long InsertSample(Sample s)
        {
            var withoutClm = new string[] { "id" };  // Sample登録で ID は AutoInclimentだから指定しない。
            long insertId = DbCommon.InsertTable(TableName, s, withoutClm);
            return insertId;
        }

        public static Sample[] GetSamples()
        {
            var allData = DbCommon.SelectTableAll(
                                                        TableName,
                                                        typeof(Sample));

            var list = allData.Select(s => s)
                                        .Cast<Sample>()
                                        .Where(s => s.ISDELETE == "0")
                                        .ToArray();

            // 必ず1件は在るはず（初期データベースに入れている）
            return (Sample[])list;
        }

        // Sample のdelete は ISDELETE　を 0 以外にする。
        public static IEnumerable<long> DeleteSample(long[] deleteSampleIds)
        {
            var deleteIds = DbCommon.DeleteRecord(TableName, deleteSampleIds);
            return deleteIds;
        }

        public static long UpdateSample(Sample s)
        {
            var withoutClm = new string[] { "id" };  // Sample登録で ID は AutoInclimentだから指定しない。
            var updId = DbCommon.UpdateRecodeById(TableName, s, withoutClm);
            return 0;
        }

    }
}
