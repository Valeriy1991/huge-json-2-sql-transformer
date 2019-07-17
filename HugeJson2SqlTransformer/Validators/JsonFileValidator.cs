using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ether.Outcomes;
using HugeJson2SqlTransformer.Validators.Abstract;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace HugeJson2SqlTransformer.Validators
{
    public class JsonFileValidator : IJsonFileValidator
    {
        public Task<IOutcome> ValidateAsync(string jsonSchema, string jsonContent)
        {
            IOutcome result;
            IList<string> messages = new List<string>();
            using (var reader = new JsonTextReader(new StringReader(jsonContent)))
            {
                var validatingReader = new JSchemaValidatingReader(reader) {Schema = JSchema.Parse(jsonSchema)};
                validatingReader.ValidationEventHandler += (o, a) => messages.Add(a.Message);

                try
                {
                    while (reader.Read())
                    {
                    }
                }
                catch (JsonReaderException ex)
                {
                    messages.Add(ex.Message);
                }
            }

            if (messages.Any())
            {
                var errorsBuilder = new StringBuilder("JSON is invalid because:\n");

                var messagesCount = messages.Count;
                for (int i = 0; i < messagesCount; i++)
                {
                    errorsBuilder.Append($"{i + 1}. {messages[i]}");
                }

                result = Outcomes.Failure().WithMessage(errorsBuilder.ToString());
                return Task.FromResult(result);
            }

            result = Outcomes.Success();
            return Task.FromResult(result);
        }
    }
}