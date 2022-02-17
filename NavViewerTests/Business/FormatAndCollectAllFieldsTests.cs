using Microsoft.VisualStudio.TestTools.UnitTesting;
using NavCodeViewer.Business;
using NavViewerTests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavViewerTests.Business.Tests
{
    [TestClass()]
    public class FormatAndCollectAllFieldsTests
    {
        [TestMethod()]
        public void FormatAndCollectFieldTest()
        {
            var tomodified = Strings.String3;
            NavCodeViewer.Domain.Field field = new NavCodeViewer.Domain.Field();
            //var result = new TableMgt().FormatAndCollectField(tomodified, field);
            //Assert.AreEqual(expected, result);
            Console.WriteLine(field.ToString());
            //Console.WriteLine(result);
        }
        [TestMethod()]
        public void FormatAndCollectAllFieldsTest01()
        {
            var tomodified = Strings.String4;
            //TableMgt table = new TableMgt();
            //var result = table.FormatFields(tomodified);
            //Assert.AreEqual(expected, result);
            //Console.WriteLine(field.ToString());
            //Console.WriteLine(result);
            //foreach (var it in table.FieldList)
            //{
            //    Console.WriteLine(it.ToString());
            //}
        }
        [TestMethod()]
        public void FormatAndCollectAllFieldsTest02()
        {
            var tomodified = Strings.String8;
            //TableMgt table = new TableMgt();
            //var result = table.FormatFields(tomodified);
            //Assert.AreEqual(expected, result);
            //Console.WriteLine(field.ToString());
            //Console.WriteLine(result);
            //PrivateObject @object = new PrivateObject(table);
            //var result = @object.Invoke("FormatAndCollectAllFields", tomodified);
            //foreach (var it in table.FieldList)
            //{
            //    Console.WriteLine(it.ToString());
            //}
        }
        [TestMethod()]
        public void FormatTable()
        {
            var tomodified = Strings.String9;
            //TableMgt table = new TableMgt();
            //var result = table.FormatTable(tomodified);
            //Console.WriteLine(result);
        }
        [TestMethod()]
        public void ReplaceQuotesOnCapionMLTest01()
        {
            //TableMgt table = new TableMgt();

            //var expected = @"    { 5790;   ;Shipping Time       ;DateFormula   ;AccessByPermission=TableData 5790=R;
            //                                       CaptionML=[ENU=Shipping Time;
            //                                                  FRA=D‚lai d exp‚dition] }";

            //string tomodified = @"    { 5790;   ;Shipping Time       ;DateFormula   ;AccessByPermission=TableData 5790=R;
            //                                       CaptionML=[ENU=Shipping Time;
            //                                                  FRA=D‚lai d'exp‚dition] }";
            //var result = table.ReplaceQuotesOnCapionML(tomodified);
            //Assert.AreEqual(expected, result);

            //tomodified = @"    { 5790;   ;Shipping Time       ;DateFormula   ;AccessByPermission=TableData 5790=R;
            //                                       CaptionML=[ENU=Shipping Time;
            //                                                  FRA=D‚lai d exp‚dition] }";
            //result = table.ReplaceQuotesOnCapionML(tomodified);
            //Assert.AreEqual(expected, result);

            //tomodified = @"BEGIN 9   ;   ;Phone No.           ;Text30        ;ExtendedDatatype=Phone No.;
            //                                       CaptionML=[ENU=']Phone No'.;
            //                                                  FRA=Nï¿½ tï¿½lï¿½phone] END";
            //result = table.ReplaceQuotesOnCapionML(tomodified);
            //Assert.AreEqual(expected, result);
        }
    }
}