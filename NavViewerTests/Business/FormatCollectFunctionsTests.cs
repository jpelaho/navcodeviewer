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
    public class FormatCollectFunctionsTests
    {
        [TestMethod()]
        public void FormatCollectFunctionsTests01()
        {
            //var result = new TableMgt().CollectProcedure(@"PROCEDURE GetLanguageID@2(LanguageCode@1000 : Code[10]) : Integer;");
            //Assert.AreEqual("GetLanguageID", result);
        }
        [TestMethod()]
        public void FormatCollectFunctionsTests02()
        {
            //var result = new Table().FormatCodeVAR(Strings.String5);
            //Console.WriteLine(result);
        }
        [TestMethod()]
        public void FormatCodeTests()
        {
            var result = new TableMgt().FormatCode(Strings.String7);
            Console.WriteLine(result);
        }
        
    }
}