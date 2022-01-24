using Microsoft.VisualStudio.TestTools.UnitTesting;
using Template.ProcessApis.Api1.Services;

namespace Template.ProcessApis.Api1.Test
{
    [TestClass]
    public class Service1Test
    {
        [TestMethod]
        public void TestGet()
        {
            Service1 service = new Service1();
            var result = service.Get(1);
            Assert.IsNotNull(result);
        }
    }
}