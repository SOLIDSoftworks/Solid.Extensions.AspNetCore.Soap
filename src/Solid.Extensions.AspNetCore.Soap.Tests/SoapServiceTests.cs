using Microsoft.Extensions.Logging;
using Solid.Extensions.AspNetCore.Soap.Tests.Host;
using Solid.Testing.AspNetCore.Extensions.XUnit;
using Solid.Testing.AspNetCore.Extensions.XUnit.Soap;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Solid.Extensions.AspNetCore.Soap.Tests
{
    public class SoapServiceTests : IClassFixture<SoapTestingServerFixture<Startup>>
    {
        private SoapTestingServerFixture<Startup> _fixture;

        public SoapServiceTests(SoapTestingServerFixture<Startup> fixture, ITestOutputHelper output)
        {
            fixture.SetOutput(output);
            _fixture = fixture;
            _fixture.UpdateConfiguration(builder => builder.SetLogLevel("Solid", LogLevel.Trace), clear: true);
        }

        [Theory]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Warning)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Critical)]
        public void ShouldDoNonDestructiveLogging(LogLevel level)
        {
            _fixture.UpdateConfiguration(builder => builder.SetDefaultLogLevel(level), clear: true);

            var expected = Guid.NewGuid().ToString();
            var channel = _fixture.CreateChannel<IEchoServiceContract>(path: "echo");
            // mostly just testing that this doesn't explode
            var value = channel.Echo(expected);

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public void ShouldEcho(string expected)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(path: "echo");
            var value = channel.Echo(expected);

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public async Task ShouldAsynchronouslyEcho(string expected)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(path: "echo");
            var value = await channel.AsynchronousEchoAsync(expected);

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public void ShouldOutEcho(string expected)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(path: "echo");
            channel.OutEcho(expected, out var value);

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public void ShouldWrappedEcho(string expected)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(path: "echo");
            var wrapper = new EchoWrapper { Value = expected };
            var response = channel.WrappedEcho(wrapper);

            Assert.Equal(expected, response.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public void ShouldWrappedAndOutEcho(string expected)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(path: "echo");
            var wrapper = new EchoWrapper { Value = expected };
            var response = channel.WrappedAndOutEcho(wrapper, out var value);

            Assert.Equal(expected, response.Value);
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("expected")]
        public void ShouldGetFaultException(string expected)
        {
            var channel = _fixture.CreateChannel<IFaultServiceContract>(path: "faults");
            var exception = null as Exception;
            try
            {
                channel.ThrowsException(expected);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsType<FaultException>(exception);
            var fault = exception as FaultException;
            Assert.NotEqual(expected, fault.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("expected")]
        public void ShouldGetFaultContract(string expected)
        {
            var channel = _fixture.CreateChannel<IFaultServiceContract>(path: "faults");
            var exception = null as Exception;
            try
            {
                channel.ThrowsContractFault(expected);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsType<FaultException<TestDetail>>(exception);
            var fault = exception as FaultException<TestDetail>;
            var detail = fault.Detail;
            Assert.Equal(expected, detail.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("expected")]
        public void ShouldGetFaultExceptionWithExceptionDetails(string expected)
        {
            var channel = _fixture.CreateChannel<IDetailedFaultServiceContract>(path: "detailedfaults");
            var exception = null as Exception;
            try
            {
                channel.ThrowsException(expected);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Assert.IsType<FaultException<ExceptionDetail>>(exception);
            var fault = exception as FaultException<ExceptionDetail>;
            var detail = fault.Detail;
            Assert.Equal(expected, fault.Message);
            Assert.Equal(expected, detail.Message);
        }
    }
}
