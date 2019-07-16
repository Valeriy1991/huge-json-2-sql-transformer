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
        private readonly string _tableName;
        private readonly string _schema;

        public SqlBuilderDirector(ISqlBuilder sqlBuilder, string tableName, string schema)
        {
            _sqlBuilder = sqlBuilder;
            _tableName = tableName;
            _schema = schema;
            _stringSqlBuilder = new StringBuilder();
        }

        public Task<string> MakeAsync(string jsonContent)
        {
            if(string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentNullException(nameof(jsonContent));

            _stringSqlBuilder.Append(_sqlBuilder.CreateTable(_tableName, _schema));
            _stringSqlBuilder.Append("\n");
            _stringSqlBuilder.Append(_sqlBuilder.CreateManyInserts());

            return Task.FromResult(_stringSqlBuilder.ToString());
        }
    }
}