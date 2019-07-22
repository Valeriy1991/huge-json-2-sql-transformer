using System.Collections.Generic;
using Bogus;

namespace j2s.Tests.Unit.Fake
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
""phone"": ""{faker.Person.Phone}""
}}"
                .Replace("\r\n", " ");
        }

        public static string WithNestedObject(this string jsonItem)
        {
            var faker = new Faker();
            var email1 = faker.Person.Email;
            var email2 = faker.Person.Email;
            return $@"{jsonItem.TrimEnd('}')}, 
""emails"": [{{ ""item"": ""{email1}"" }}, {{ ""item"": ""{email2}"" }}]
}}"
                .Replace("\r\n", " ");
        }
    }
}