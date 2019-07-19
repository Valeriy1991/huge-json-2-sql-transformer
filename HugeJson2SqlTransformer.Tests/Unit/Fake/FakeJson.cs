using System.Collections.Generic;
using System.Text;
using Bogus;

namespace HugeJson2SqlTransformer.Tests.Unit.Fake
{
    public static class FakeJson
    {
        public static List<string> CreateMultiple(int count)
        {
            var list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                list.Add(Create());
            }

            return list;
        }

        public static string Create()
        {
            var faker = new Faker();
            return $@"{{
	""firstName"": ""{faker.Person.FirstName}"",
    ""lastName"": ""{faker.Person.LastName}"",
    ""isClient"": {faker.Random.Bool()},
    ""email"": ""{faker.Person.Email}""
}}";
        }
    }
}