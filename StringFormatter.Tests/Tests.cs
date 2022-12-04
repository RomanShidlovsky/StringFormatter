using StringFormatter.Core;

namespace StringFormatter.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            
            var user = new User();
            var result = Core.StringFormatter.Shared.Format("{Name[[0]]}!", user);
            
            Assert.That(result, Is.EqualTo("{FIELD_VALUE!"));
            
        }
    }

    public class User
    {
        public string Name { get; set; } = "Vasya";
    }
}