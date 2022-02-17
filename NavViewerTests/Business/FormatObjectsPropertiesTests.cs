using Microsoft.VisualStudio.TestTools.UnitTesting;
using NavCodeViewer.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavViewerTests.Business.Tests
{
    [TestClass()]
    public class FormatObjectsPropertiesTests
    {
        [TestMethod()]
        public void FormatObjectsPropertiesTest01()
        {
            var tomodified = @"  OBJECT-PROPERTIES
  {
    Date=15/02/20;
    Time=21:54:29;
    Modified=Yes;
    Version List=NAVW19.00;
  }";
            var expected = @"  OBJECT-PROPERTIES
  BEGIN
    Date=15/02/20;
    Time=21:54:29;
    Modified=Yes;
    Version List=NAVW19.00;
  END";
            var result = new TableMgt().FormatObjectsProperties(tomodified);
            Assert.AreEqual(expected, result);
            Console.WriteLine(tomodified);
        }


        [TestMethod()]
        public void FormatObjectsPropertiesTest02()
        {
            var tomodified = @"  OBJECT-PROPERTIES
  {
    Date=15/02/20;
    Time=21:54:29;
    Modified=Yes;
    Version List={NAVW19.00};
  }";
            var expected = @"  OBJECT-PROPERTIES
  BEGIN
    Date=15/02/20;
    Time=21:54:29;
    Modified=Yes;
    Version List={NAVW19.00};
  END";
            var result = new TableMgt().FormatObjectsProperties(tomodified);
            Assert.AreEqual(expected, result);
            Console.WriteLine(tomodified);
        }
    }
}
