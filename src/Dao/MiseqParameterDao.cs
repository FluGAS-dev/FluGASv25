using FluGASv25.Models.Properties;
using FluGASv25.Utils;
using System.Data;
using System.Linq;

namespace FluGASv25.Dao
{
    public static class MiseqParameterDao
    {
        private const string TableName = "MiseqParameters";

        // 1-insert.
        public static long InsertPatameter(Parameters p)
        {
            var withoutClm = new string[] { "id" };  // Insert で ID は AutoInclimentだから指定しない。
            long insertId = DbCommon.InsertTable(TableName, p, withoutClm);
            return insertId;
        }

        public static MiseqParameters[] GetParameters()
        {
            var allData = DbCommon.SelectTableAll(TableName, typeof(MiseqParameters));

            // 初期インストール時の 救済処置
            if (allData == null || allData.Count < 1)
            {
                var p = new MiseqParameters
                {
                    Name = ConstantValues.DefaultMiseqParameterName,
                    Note = "init",
                    IsFastQC = true,
                    IsAnalysisTop3 = true,
                    IsReferenceSelectBlastEvalue = true,
                    //ReferenceSelectBlastElement = Proc.Several.Blast.SelectLengthElm,
                    ReferenceSelectBlastElement = Proc.Several.Blast.SelectScoreElm, // 2020.04.06 Length から Score へ by 岡
                    IsLessThanNone1st = false,
                    IsLessThanNone2nd = true,
                    IsSampling = false,
                };
                InsertPatameter(p);
                var ps = DbCommon.SelectTableAll(TableName, typeof(MiseqParameters));
                allData = ps;
            }
            var list = allData.Select(s => s).Cast<MiseqParameters>().ToArray();

            // 必ず1件は在るはず（初期データベースに入れている）
            return (MiseqParameters[])list;
        }


        public static MiseqParameters GetParameters(string parameterName)
        {
            var allParam = GetParameters();
            var lambda = allParam.Select(s => s).Cast<MiseqParameters>();

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
