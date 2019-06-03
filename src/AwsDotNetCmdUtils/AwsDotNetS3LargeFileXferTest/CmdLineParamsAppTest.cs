using System;
using Xunit;
using AwsDotNetS3LargeFileXferCmd;

namespace AwsDotNetS3LargeFileXferTest
{
    public class CmdLineParamsAppTest
    {
        public static readonly string[] StdParamsUpload = { "--operation", "upload", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test" };
        public static readonly string[] StdParamsDownload = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test" };

        public static readonly string[] StdParamsDownloadOverwriteArg = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "overwrite", "--bucketregion", "us-west-1", "--bucketname", "test" };
        public static readonly string[] StdParamsDownloadOverwriteArgFileSpace = { "--operation", "download", "'c:\\users\\wills\\one dot\\largefile.bin'", "overwrite", "--bucketregion", "us-west-1", "--bucketname", "test" };
        public static readonly string[] StdParamsDownloadBadOverwriteArg = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "Xoverwrite", "--bucketregion", "us-west-1", "--bucketname", "test" };

        private readonly string[] StdParamsMissingOp = { "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsMissingOpMode = { "--operation", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsInvalidOpMode = { "--operation", "Xupload", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsDownloadMissingFilename = { "--operation", "download", "--bucketregion", "us-west-1", "--bucketname", "test" };
        private readonly string[] StdParamsUploadMissingFilename = { "--operation", "upload", "--bucketregion", "us-west-1", "--bucketname", "test" };

        private readonly string[] StdParamsMissingBucketRegion = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketname", "test" };
        private readonly string[] StdParamsBucketRegionMissingArg = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "--bucketname", "test" };
        private readonly string[] StdParamsMissingBucketName = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1" };
        private readonly string[] StdParamsBucketNameMissingArg = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname" };

        private readonly string[] StdParamsMinThreadsMinPartialSize = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--threads", "1", "--partialsize", "4096" };
        private readonly string[] StdParamsMaxThreadsMxPartialSize = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--threads", "100", "--partialsize", "4294967296" };

        private readonly string[] StdParamsExcessThreads = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--threads", "101" };
        private readonly string[] StdParamsTooFewThreads = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--threads", "0" };
        private readonly string[] StdParamsInvalidThreads = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--threads", "x" };
        private readonly string[] StdParamsMissingThreadsArg = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--threads" };

        private readonly string[] StdParamsExcessPartialSize = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--partialsize", "4294967297" };
        private readonly string[] StdParamsTooSmallPartialSize = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--partialsize", "4095" };
        private readonly string[] StdParamsInvalidPartialSize = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--partialsize", "y" };
        private readonly string[] StdParamsMissingPartialSizeArg = { "--operation", "download", "c:\\users\\wills\\largefile.bin", "--bucketregion", "us-west-1", "--bucketname", "test", "--partialsize" };


        [Fact]
        public void TestStdParamsUpload()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsUpload);

            Assert.False(cmdLine.IsError);
            Assert.Null(cmdLine.GetErrorMsg());

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Upload, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.InputFile);
            Assert.Null(cmdLine.OutputFile);
            Assert.Equal(CmdLineParamsApp.PartialSizeValueDefault, cmdLine.PartialSize);
            Assert.Equal(CmdLineParamsApp.ThreadsValueDefault, cmdLine.Threads);
        }

        [Fact]
        public void TestStdParamsDownload()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsDownload);

            Assert.False(cmdLine.IsError);
            Assert.Null(cmdLine.GetErrorMsg());

            Assert.Equal("us-west-1", cmdLine.BucketRegion);
            Assert.Equal("test", cmdLine.BucketName);
            Assert.Equal(CmdLineParamsApp.OpMode.Download, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.OutputFile);
            Assert.Null(cmdLine.InputFile);
            Assert.Equal(CmdLineParamsApp.PartialSizeValueDefault, cmdLine.PartialSize);
            Assert.Equal(CmdLineParamsApp.ThreadsValueDefault, cmdLine.Threads);
            Assert.False(cmdLine.Overwrite);
        }

        [Fact]
        public void TestStdParamsDownloadOverwriteArg()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsDownloadOverwriteArg);

            Assert.False(cmdLine.IsError);
            Assert.True(cmdLine.Overwrite);

            Assert.Equal(CmdLineParamsApp.OpMode.Download, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\largefile.bin", cmdLine.OutputFile);
        }

        [Fact]
        public void TestStdParamsDownloadOverwriteArgFileSpace()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsDownloadOverwriteArgFileSpace);

            Assert.False(cmdLine.IsError);
            Assert.True(cmdLine.Overwrite);

            Assert.Equal(CmdLineParamsApp.OpMode.Download, cmdLine.Op);
            Assert.Equal("c:\\users\\wills\\one dot\\largefile.bin", cmdLine.OutputFile);
        }

        [Fact]
        public void TestStdParamsDownloadBadOverwriteArg()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsDownloadBadOverwriteArg);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --operation download third argument is Xoverwrite not overwrite", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsMissingOp()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMissingOp);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: Missing --operation parameter", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsMissingOpMode()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMissingOpMode);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --operation has incorrect number of arguments; found 0 should be 2", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsInvalidOpMode()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsInvalidOpMode);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --operation first argument is invalid; xupload", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsDownloadMissingFilename()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsDownloadMissingFilename);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --operation has incorrect number of arguments; found 1 should be 2", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsUploadMissingFilename()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsUploadMissingFilename);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --operation has incorrect number of arguments; found 1 should be 2", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsMissingBucketRegion()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMissingBucketRegion);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: Missing --bucketregion parameter", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsBucketRegionMissingArg()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsBucketRegionMissingArg);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --bucketregion has incorrect number of arguments; found 0 should be 1", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsMissingBucketName()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMissingBucketName);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: Missing --bucketname parameter", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsBucketNameMissingArg()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsBucketNameMissingArg);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --bucketname has incorrect number of arguments; found 0 should be 1", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsMinThreadsMinPartialSize()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMinThreadsMinPartialSize);

            Assert.False(cmdLine.IsError);

            Assert.Equal(CmdLineParamsApp.PartialSizeValueMin, cmdLine.PartialSize);
            Assert.Equal(CmdLineParamsApp.ThreadsValueMin, cmdLine.Threads);
        }

        [Fact]
        public void TestStdParamsMaxThreadsMaxPartialSize()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMaxThreadsMxPartialSize);

            Assert.Equal(CmdLineParamsApp.PartialSizeValueMax, cmdLine.PartialSize);
            Assert.Equal(CmdLineParamsApp.ThreadsValueMax, cmdLine.Threads);

            Assert.False(cmdLine.IsError);
        }

        [Fact]
        public void TestStdParamsExcessThreadsExcess()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsExcessThreads);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: --threads value 101 is invalid", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsTooFewThreads()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsTooFewThreads);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: --threads value 0 is invalid", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsInvalidThreads()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsInvalidThreads);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --threads argument value x is not a valid number", cmdLine.GetErrorMsg());
        }


        [Fact]
        public void TestStdParamsMissingThreadsArg()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMissingThreadsArg);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --threads has incorrect number of arguments; found 0 should be 1", cmdLine.GetErrorMsg());
        }


        [Fact]
        public void TestStdParamsExcessPartialSize()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsExcessPartialSize);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: --partialsize value 4294967297 is invalid", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsTooSmallPartialSize()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsTooSmallPartialSize);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: --partialsize value 4095 is invalid", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsInvalidPartialSize()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsInvalidPartialSize);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --partialsize argument value y is not a valid number", cmdLine.GetErrorMsg());
        }

        [Fact]
        public void TestStdParamsMissingPartialSizeArg()
        {
            var cmdLine = new CmdLineParamsApp(StdParamsMissingPartialSizeArg);

            Assert.True(cmdLine.IsError);
            Assert.StartsWith("Error: parameter --partialsize has incorrect number of arguments; found 0 should be 1", cmdLine.GetErrorMsg());
        }
    }
}
