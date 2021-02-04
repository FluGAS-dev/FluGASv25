using FluGASv25.Models.Properties;
using FluGASv25.Utils;
using System.Data;
using System.Linq;

namespace FluGASv25.Dao
{
    public static class MinionParameterDao
    {
        private const string TableName = "MinionParameters";

        // 1-insert.
        public static long InsertPatameter(Parameters p)
        {
            var withoutClm = new string[] { "id" };  // Insert で ID は AutoInclimentだから指定しない。
            long insertId = DbCommon.InsertTable(TableName, p, withoutClm);
            return insertId;
        }

        public static MinionParameters[] GetParameters()
        {
            var allData = DbCommon.SelectTableAll(TableName, typeof(MinionParameters));

            // 初期インストール時の 救済処置
            if(allData == null || allData.Count < 1)
            {
                var p = new MinionParameters {
                    Name = ConstantValues.DefaultMinionParameterName,
                    Note = "init",
                    IsAnalysisTop3 = true,
                    IsReferenceSelectBlastEvalue = true,
                    // ReferenceSelectBlastElement = Proc.Several.Blast.SelectLengthElm,
                    ReferenceSelectBlastElement = Proc.Several.Blast.SelectScoreElm, // 2020.04.06 Length から Score へ by 岡
                    IsMinimap2 = true,
                    IsLessThanNone1st = false,
                    IsLessThanNone2nd = true,
                };
                InsertPatameter(p);
                var ps = DbCommon.SelectTableAll(TableName,typeof(MinionParameters));
                allData = ps;
            }
            var list = allData.Select(s => s).Cast<MinionParameters>().ToArray();

            // 必ず1件は在るはず（初期データベースに入れている）
            return (MinionParameters[])list;
        }


        public static MinionParameters GetParameters(string parameterName, int sequencerId)
        {
            var allParam = GetParameters();
            var lambda = allParam.Select(s => s).Cast<MinionParameters>();

            // 予防措置
            var isNameExist = lambda.Where(p => p.Name == parameterName);
            if (!isNameExist.Any())
                return lambda.Where(p => p.Id == 0).First();  // 初期のDefaultパラメータ。

            var oneParam = lambda.Where(p => p.Name == parameterName)  // 同じ名前で一番日付の新しいもの
                                                .OrderByDescending(p => p.CreateDate)
                                                .First();
            return oneParam;
        }

    }
}
