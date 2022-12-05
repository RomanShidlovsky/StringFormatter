using StringFormatter.Core;

namespace StringFormatter.Tests
{
    public class Tests
    {
        private class TestClass
        {
            public string Name;
            public int Age { get; set; }
            public string[] Cities { get; set; } = { "Minsk", "Vitebsk", "Senno" };

            private string _privateField = "Get it!";
            public TestClass(string name)
            {
                Name = name;
                Age = 18;
            }
        }

        private TestClass _testClass = new TestClass("Vasya");
        
        [Test]
        public void FieldAndPropertyAccessTest()
        {
            var result = Core.StringFormatter.Shared.Format(
                "Name: {Name}, Age: {Age}", _testClass);

            Assert.That(result, Is.EqualTo("Name: Vasya, Age: 18"));
        }

        [Test]
        public void UnclosedBraceTest() 
        {
            Assert.Throws<Exception>(() => Core.StringFormatter.Shared.Format(
                "Name: {Name, Age: {Age}", _testClass),
                "Didn't throw an exception on unclosed brace");
        }

        [Test]
        public void EscapedBracesTest()
        {
            var result = Core.StringFormatter.Shared.Format(
                "Escaped brace characters : '{{{{', {{{Age}}}, '}}'", _testClass);

            Assert.That(result, Is.EqualTo("Escaped brace characters : '{{', {18}, '}'"));
        }

        [Test]
        public void InvalidIdentifierTest()
        {
            Assert.Throws<Exception>(() => Core.StringFormatter.Shared.Format(
                "Invalid name: {1Name}", _testClass),
                "Didn't throw an exception on invalid character in identifier");
        }

        [Test]
        public void PrivateFieldOrPropertyTest()
        {
            Assert.Throws<Exception>(() => Core.StringFormatter.Shared.Format(
                "Private field: {_privateField}", _testClass),
                "Did't throw an exception on private field/property access");
        }

        [Test]
        public void CollectionAccessByIndexTest()
        {
            var result = Core.StringFormatter.Shared.Format(
                "Cities: {Cities[0]}, {Cities[1]}, {Cities[2]}", _testClass);

            Assert.That(result, Is.EqualTo("Cities: Minsk, Vitebsk, Senno"));
        }

        [Test]
        public void UnclosedSquareBraceTest()
        {
            Assert.Throws<Exception>(() => Core.StringFormatter.Shared.Format(
                "City: {Cities[1}", _testClass),
                "Didn't throw an exception on unclosed square brace");
        }

        [Test]
        public void InvalidIndexTest()
        {
            Assert.Throws<Exception>(() => Core.StringFormatter.Shared.Format(
                "City: {Cities[0a]}", _testClass),
                "Didn't throw an exception on invalid character in index");
        }

        [Test]
        public void MultipleAccessTest()
        {
            var result = Core.StringFormatter.Shared.Format(
                "{Name}, {Name}", _testClass);

            Assert.That(result, Is.EqualTo("Vasya, Vasya"));
        }
    }
}