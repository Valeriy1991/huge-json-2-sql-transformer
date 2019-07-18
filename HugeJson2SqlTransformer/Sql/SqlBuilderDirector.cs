using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HugeJson2SqlTransformer.Sql.Abstract;
using HugeJson2SqlTransformer.Sql.TableDefinition;

[assembly:InternalsVisibleTo("HugeJson2SqlTransformer.Tests")]
namespace HugeJson2SqlTransformer.Sql
{
    public class SqlBuilderDirector : ISqlBuilderDirector
    {
        internal ISqlBuilder SqlBuilder { get; private set; }
        internal StringBuilder StringSqlBuilder { get; private set; }
        private readonly string _tableName;
        private readonly string _schema;

        public SqlBuilderDirector(string tableName, string schema)
        {
            _tableName = tableName;
            _schema = schema;
            StringSqlBuilder = new StringBuilder();
        }

        public Task ChangeBuilder(ISqlBuilder sqlBuilder)
        {
            SqlBuilder = sqlBuilder;
            StringSqlBuilder = new StringBuilder();
            return Task.CompletedTask;
        }

        public Task<string> MakeAsync(string jsonContent)
        {
            if(string.IsNullOrWhiteSpace(jsonContent))
                throw new ArgumentNullException(nameof(jsonContent));

            StringSqlBuilder.Append(SqlBuilder.CreateTable(_tableName, _schema));
            StringSqlBuilder.Append("\n");
            StringSqlBuilder.Append(SqlBuilder.CreateInsert(_tableName, _schema, jsonContent));

            return Task.FromResult(StringSqlBuilder.ToString());
        }
    }
}