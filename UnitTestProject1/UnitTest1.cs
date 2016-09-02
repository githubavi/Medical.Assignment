using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPF.Assignment;
using System.Xml.Linq;
using System.Threading;
using Moq;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1 : IView
    {
        static DailyMedModel model;
        
        [TestCleanup]
        public void TestFinished()
        {
        }

        [ClassCleanup]
        public static void CCleanup()
        {
        }

        [ClassInitialize]
        public static void CCInitialize(TestContext ctx)
        {
            model = new DailyMedModel();
        }

        [TestInitialize]
        public void TestInit()
        {
        }
        
        [TestMethod]
        public void TestMethod1()
        {
            DailyMedViewModel vm = new DailyMedViewModel(this);

            try
            {
                vm.Query = "";
                vm.Query = "go";
                vm.Query = "google";
                vm.Query = null;
            }
            catch (ValidationException)
            {
                //valid
            }
        }

        [TestMethod]
        public void TestMethod2()
        {   
            Mock<XDocument> mockHandler = new Mock<XDocument>();
            Mock<Stream> mockStream = new Mock<Stream>();

            try
            {
                var data = model.GetSuggestDataString(mockStream.Object);
                var data2 = model.GetSuggestDataXml(mockHandler.Object);

                mockHandler.Verify();

                Assert.IsNull(data, "Fail 1");
                Assert.IsNotNull(data2, "Fail 2");
            }catch(InvalidDataException)
            {
                //validation
            }
            
        }

        [TestMethod]
        public void Test_QueryData_ThrowsInvalidDataException()
        {
            var ex = ExceptionAssert.Throws<InvalidDataException>(() => model.GetQueryData(new Mock<Stream>().Object));
            Assert.AreEqual(ex.GetType(), typeof(InvalidDataException));
        }

        public void SetCursor()
        {
            throw new NotImplementedException();
        }

        public void SetSuggestionBox()
        {
            throw new NotImplementedException();
        }
    }

    public static class ExceptionAssert
    {
        public static T Throws<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                return ex;
            }

            Assert.Fail("Expected exception of type {0}.", typeof(T));

            return null;
        }
    }
}
