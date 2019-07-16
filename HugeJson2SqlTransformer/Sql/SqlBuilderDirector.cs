using System;
using System.Text;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Sql.Abstract;

namespace HugeJson2SqlTransformer.Sql
{
    public class SqlBuilderDirector : ISqlBuilderDirector
    {
        private readonly ISqlBuilder _sqlBuilder;
        private readonly StringBuilder _stringSqlBuilder;

        public SqlBuilderDirector(ISqlBuilder sqlBuilder)
        {
            _sqlBuilder = sqlBuilder;
            _stringSqlBuilder = new StringBuilder();
        }

        public Task<string> MakeAsync(string jsonContent)
        {
            if(string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentNullException(nameof(jsonContent));

            _stringSqlBuilder.Append(_sqlBuilder.CreateTable());
            _stringSqlBuilder.Append("\n");
            _stringSqlBuilder.Append(_sqlBuilder.CreateManyInserts());

            return Task.FromResult(_stringSqlBuilder.ToString());
        }
    }
}