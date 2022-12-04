namespace StringFormatter.Core.Interfaces
{
    public interface IStringParser
    {
        string Parse(string template, object target);
    }
}
