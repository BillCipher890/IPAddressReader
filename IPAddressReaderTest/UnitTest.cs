using System.Net;
using IPAddressReader;

namespace IPAddressReaderTest
{
    public class UnitTest
    {
        [Fact]
        public void TestCreatingRootCommand()
        {
            void Filter(FileInfo fileLog, FileInfo fileOutput, DateTime timeStart, DateTime timeEnd, IPAddress? addressStart, int? addressMask) { }
            var rootCommand = IPAddressCommandBuilder.CreateRootCommand(Filter);
            Assert.NotNull(rootCommand);
        }

        [Fact]
        public void TestCreatingIPAddressFilter()
        {
            var filter = new IPAddressFilter();
            Assert.NotNull(filter);
        }

        [Fact]
        public void TestExecutingEmptyNotFullFilter()
        {
            var testData = new string[]
            {
                "--file-log",
                string.Empty,
                "--file-output",
                string.Empty,
                "--time-start",
                string.Empty,
                "--time-end",
                string.Empty
            };
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(testData);
        }

        [Fact]
        public void TestExecutingEmptyFullFilter()
        {
            var testData = new string[]
            {
                "--file-log",
                string.Empty,
                "--file-output",
                string.Empty,
                "--time-start",
                string.Empty,
                "--time-end",
                string.Empty,
                "--address-start",
                string.Empty,
                "--address-mask",
                string.Empty
            };
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(testData);
        }

        [Fact]
        public void TestExecutingFullFilter()
        {
            var testData = new string[]
            {
                "--file-log",
                "G:/source/testData/IPAddressReader/input.txt", 
                "--file-output", 
                "G:/source/testData/IPAddressReader/output.txt", 
                "--time-start", 
                "01.01.2001", 
                "--time-end", 
                "06.04.2200", 
                "--address-start",
                "255.255.255.255",
                "--address-mask",
                "16"
            };
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(testData);
        }

        [Fact]
        public void ExecutingWrondPathsFilter()
        {
            var testData = new string[]
            {
                "--file-log",
                "G1:/source/testData/IPAddressReader/input.txt21ds",
                "--file-output",
                "Gaa:/source/testData/IPAddressReader/output.txtdda",
                "--time-start",
                "01.01.2001",
                "--time-end",
                "06.04.2200"
            };
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(testData);
        }

        [Fact]
        public void ExecutingWrondDatesFilter()
        {
            var testData = new string[]
            {
                "--file-log",
                "G:/source/testData/IPAddressReader/input.txt",
                "--file-output",
                "G:/source/testData/IPAddressReader/output.txt",
                "--time-start",
                "00.00.0000",
                "--time-end",
                "125.44.9999999"
            };
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(testData);
        }

        [Fact]
        public void ExecutingWrondAddressStartFilter()
        {
            var testData = new string[]
            {
                "--file-log",
                "G:/source/testData/IPAddressReader/input.txt",
                "--file-output",
                "G:/source/testData/IPAddressReader/output.txt",
                "--time-start",
                "01.01.0001",
                "--time-end",
                "04.04.2024",
                "--address-start",
                "333.444.555.666"
            };
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(testData);
        }

        [Fact]
        public void ExecutingWrondAddressStartAndMaskFilter()
        {
            var testData = new string[]
            {
                "--file-log",
                "G:/source/testData/IPAddressReader/input.txt",
                "--file-output",
                "G:/source/testData/IPAddressReader/output.txt",
                "--time-start",
                "01.01.0001",
                "--time-end",
                "04.04.2024",
                "--address-start",
                "333.444.555.666",
                "--address-mask",
                "165"
            };
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(testData);
        }

        [Fact]
        public void ExecutingMaskWithoutAddressStartFilter()
        {
            var testData = new string[]
            {
                "--file-log",
                "G:/source/testData/IPAddressReader/input.txt",
                "--file-output",
                "G:/source/testData/IPAddressReader/output.txt",
                "--time-start",
                "01.01.0001",
                "--time-end",
                "04.04.2024",
                "--address-mask",
                "16"
            };
            var filter = new IPAddressFilter();
            filter.ExecuteFilter(testData);
        }
    }
}