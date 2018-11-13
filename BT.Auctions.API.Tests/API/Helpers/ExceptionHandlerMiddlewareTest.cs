using BT.Auctions.API.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BT.Auctions.API.Tests.API.Helpers
{
    /// <summary>
    /// Used to test the exception handler attribute for global exceptions that aren't caught
    /// </summary>
    [TestClass]
    public class ExceptionHandlerMiddlewareTest
    {
        private Mock<ILogger<ExceptionHandlerMiddleware>> _exceptionLogger;

        [TestInitialize]
        public void Initialize()
        {
            _exceptionLogger = new Mock<ILogger<ExceptionHandlerMiddleware>>();

        }

        [TestMethod]
        public async Task Global_Exception_Handler_Throw_System_Exception()
        {
            //Setup

            var exceptionResponseMiddleware = new ExceptionHandlerMiddleware((innerHttpContext) => throw new Exception("Test Exception Thrown"), _exceptionLogger.Object);
            var context = new DefaultHttpContext();
            //Action
            await exceptionResponseMiddleware.Invoke(context);

            //Result
            Assert.AreEqual(500, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task Global_Exception_Handler_Throw_Unauthorised_Exception()
        {
            //Setup

            var exceptionResponseMiddleware = new ExceptionHandlerMiddleware((innerHttpContext) => throw new UnauthorizedAccessException(), _exceptionLogger.Object);
            var context = new DefaultHttpContext();
            //Action
            await exceptionResponseMiddleware.Invoke(context);

            //Result
            Assert.AreEqual(401, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task Global_Exception_Handler_Throw_Unknown_Method_Exception()
        {
            //Setup

            var exceptionResponseMiddleware = new ExceptionHandlerMiddleware((innerHttpContext) => throw new NotImplementedException(), _exceptionLogger.Object);
            var context = new DefaultHttpContext();
            //Action
            await exceptionResponseMiddleware.Invoke(context);

            //Result
            Assert.AreEqual(501, context.Response.StatusCode);
        }

        [TestMethod]
        public async Task Global_Exception_Handler_Throw_IOException_Exception()
        {
            //Setup

            var exceptionResponseMiddleware = new ExceptionHandlerMiddleware((innerHttpContext) => throw new Exception(), _exceptionLogger.Object);
            var context = new DefaultHttpContext();
            //Action
            await exceptionResponseMiddleware.Invoke(context);

            //Result
            Assert.AreEqual(500, context.Response.StatusCode);
        }
    }
}
