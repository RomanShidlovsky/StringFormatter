using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringFormatter.Core.Interfaces
{
    public interface IStringParser
    {
        string Parse(string template, object target);
    }
}
