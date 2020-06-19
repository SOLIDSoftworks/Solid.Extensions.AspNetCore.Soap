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
        [MemberData(nameof(EchoTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldEcho(EchoTestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            var value = channel.Echo(data.Value);

            Assert.Equal(data.Value, value);
        }

        [Theory]
        [MemberData(nameof(EchoTestData), DisableDiscoveryEnumeration = false)]
        public async Task ShouldAsynchronouslyEcho(EchoTestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            var value = await channel.AsynchronousEchoAsync(data.Value);

            Assert.Equal(data.Value, value);
        }

        [Theory]
        [MemberData(nameof(EchoTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldOutEcho(EchoTestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            channel.OutEcho(data.Value, out var value);

            Assert.Equal(data.Value, value);
        }

        [Theory]
        [MemberData(nameof(EchoTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldWrappedEcho(EchoTestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            var wrapper = new EchoWrapper { Value = data.Value };
            var response = channel.WrappedEcho(wrapper);

            Assert.Equal(data.Value, response.Value);
        }

        [Theory]
        [MemberData(nameof(EchoTestData), DisableDiscoveryEnumeration = false)]
        public void ShouldWrappedAndOutEcho(EchoTestData data)
        {
            var channel = _fixture.CreateChannel<IEchoServiceContract>(version: data.MessageVersion, path: data.Path);
            var wrapper = new EchoWrapper { Value = data.Value };
            var response = channel.WrappedAndOutEcho(wrapper, out var value);

            Assert.Equal(data.Value, response.Value);
            Assert.Equal(data.Value, value);
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

        public static TheoryData<EchoTestData> EchoTestData = new TheoryData<EchoTestData>
        {
            //new EchoTestData { Path = "echo1", Value = null, MessageVersion = MessageVersion.None },
            //new EchoTestData { Path = "echo1", Value = "", MessageVersion = MessageVersion.None },
            //new EchoTestData { Path = "echo1", Value = "expected", MessageVersion = MessageVersion.None },
            new EchoTestData { Path = "echo2", Value = null, MessageVersion = MessageVersion.Soap11 },
            new EchoTestData { Path = "echo2", Value = "", MessageVersion = MessageVersion.Soap11 },
            new EchoTestData { Path = "echo2", Value = "expected", MessageVersion = MessageVersion.Soap11 },
            new EchoTestData { Path = "echo3", Value = null, MessageVersion = MessageVersion.Soap11WSAddressingAugust2004 },
            new EchoTestData { Path = "echo3", Value = "", MessageVersion = MessageVersion.Soap11WSAddressingAugust2004 },
            new EchoTestData { Path = "echo3", Value = "expected", MessageVersion = MessageVersion.Soap11WSAddressingAugust2004 },
            new EchoTestData { Path = "echo4", Value = null, MessageVersion = MessageVersion.Soap12WSAddressing10 },
            new EchoTestData { Path = "echo4", Value = "", MessageVersion = MessageVersion.Soap12WSAddressing10 },
            new EchoTestData { Path = "echo4", Value = "expected", MessageVersion = MessageVersion.Soap12WSAddressing10 },
            new EchoTestData { Path = "echo5", Value = null, MessageVersion = MessageVersion.Soap12WSAddressingAugust2004 },
            new EchoTestData { Path = "echo5", Value = "", MessageVersion = MessageVersion.Soap12WSAddressingAugust2004 },
            new EchoTestData { Path = "echo5", Value = "expected", MessageVersion = MessageVersion.Soap12WSAddressingAugust2004 }
        };
    }

    public class EchoTestData
    {
        public string Value { get; set; }
        public MessageVersion MessageVersion { get; set; }
        public string Path { get; set; }
    }
}
