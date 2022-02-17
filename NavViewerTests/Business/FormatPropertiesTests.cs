using Microsoft.VisualStudio.TestTools.UnitTesting;
using NavCodeViewer.Business;
using NavViewerTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavViewerTests.Business.Tests
{
    [TestClass()]
    public class FormatPropertiesTests
    {
        [TestMethod()]
        public void FormatPropertiesTest()
        {
            var tomodified = Strings.String2;
            var result = new TableMgt().FormatObjectsProperties(tomodified);
            //Assert.AreEqual(expected, result);
            Console.WriteLine(result);
        }
    }
}