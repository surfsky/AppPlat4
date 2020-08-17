using Microsoft.VisualStudio.TestTools.UnitTesting;
using MarkdownSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownSharp.Tests
{
    [TestClass()]
    public class MarkdownTests
    {
        [TestMethod()]
        public void TransformTest()
        {
            var markdown = @"
```cs
public void Main()
{
  Console.WriteLine(""Github Style Code blocks\"");
}
```
";
            var md = new MarkdownSharp.Markdown();
            var txt = md.Transform(markdown);
            // 样式怎么办？
        }
    }
}