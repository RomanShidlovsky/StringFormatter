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
            var result = Core.StringFormatter.Shared.Format("{Ints[3}!", user);
            
            Assert.That(result, Is.EqualTo("Vasya!"));
        }
    }

    public class User
    {
        public string Name { get; set; } = "Vasya";
        public int[] Ints = new int[3] {1, 2, 3};
    }
}