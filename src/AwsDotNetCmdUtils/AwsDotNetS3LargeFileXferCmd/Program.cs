using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace AwsDotNetS3LargeFileXferCmd
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            int rc = -1;

            Console.WriteLine($"AwsDotNetS3LargeFileXferCmd {GetVersion()}");

            DateTime tim = DateTime.UtcNow;
            var cmdLineParams = new CmdLineParamsProc(args);
            if (cmdLineParams.IsError)
                Console.WriteLine(cmdLineParams.GetErrorMsg());
            else
            {
                string errMsg = null;
                if (cmdLineParams.Op == CmdLineParamsProc.OpMode.Upload)
                {
                    errMsg = await FileTransferUpload(cmdLineParams);
                }
                else if (cmdLineParams.Op == CmdLineParamsProc.OpMode.Download)
                {
                    errMsg = await FileTransferDownload(cmdLineParams);
                }
                else
                {
                    errMsg = $"Operation {cmdLineParams.Op} not supported";
                }

                if (errMsg != null)
                    Console.WriteLine(errMsg);
                else
                    rc = 0;
            }
            TimeSpan elapsed = DateTime.UtcNow - tim;
            if (rc == 0)
                 Console.WriteLine("{0}",GetJobDetails(cmdLineParams, elapsed));

            Console.WriteLine((rc == 0) ? "ends" : "abends");
            return rc;
        }

        private static string GetVersion()
        {
            string rc = "v";

            rc += typeof(Program)?.GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            return rc;
        }

        private static object GetJobDetails(CmdLineParamsProc cmdLineParams, TimeSpan elapsed)
        {
            string rc = "";

            double lengthMB = 0.0;
            if (cmdLineParams.Op == CmdLineParamsProc.OpMode.Download)
            {
                lengthMB = ((double)new System.IO.FileInfo(cmdLineParams.OutputFile).Length) / 1048576;
                rc = $"download of {Path.GetFileName(cmdLineParams.OutputFile)} {string.Format("({0:0.##} MB) ", lengthMB)}";
                rc += string.Format("completed in {0} hour {1} minutes {2}.{3} seconds ", elapsed.Hours, elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
                if (elapsed.TotalSeconds > 0)
                    rc += string.Format("- transfer speed {0:0.##} MBPS", (lengthMB * 8) / elapsed.TotalSeconds);
            }
            else if (cmdLineParams.Op == CmdLineParamsProc.OpMode.Upload)
            {
                lengthMB = ((double)new System.IO.FileInfo(cmdLineParams.InputFile).Length) / 1048576;
                rc = $"upload of {Path.GetFileName(cmdLineParams.InputFile)}  {string.Format("({0:0.##} MB) ", lengthMB)}";
                rc += string.Format("completed in {0} hour {1} minutes {2}.{3} seconds ", elapsed.Hours, elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
                if (elapsed.TotalSeconds > 0)
                    rc += string.Format("- transfer speed {0:0.##} MBPS", (lengthMB * 8) / elapsed.TotalSeconds);
            }
            else
            {
                rc = "details of operation not yet supported";
            }
            return rc;
        }

        private static async Task<string> FileTransferDownload(CmdLineParamsProc cmdLineParams)
        {
            string rc = null;

            try
            {
                var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(cmdLineParams.BucketRegion));

                var fileTransferUtility = new TransferUtility(s3Client);

                var fileTransferRequest = new TransferUtilityDownloadRequest
                {
                    FilePath = cmdLineParams.OutputFile,
                    BucketName = cmdLineParams.BucketName,
                    Key = Path.GetFileName(cmdLineParams.OutputFile)
                };

                await fileTransferUtility.DownloadAsync(fileTransferRequest);
            }
            catch (Exception e)
            {
                rc = e.Message;
            }
            return rc;
        }

        private static async Task<string> FileTransferUpload(CmdLineParamsProc cmdLineParams)
        {
            string rc = null;

            try
            {
                if (File.Exists(cmdLineParams.InputFile) == false)
                    rc = $"input file {cmdLineParams.InputFile} does not exist";
                else
                {
                    var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(cmdLineParams.BucketRegion));

                    var fileTransferUtility = new TransferUtility(s3Client);

                    await fileTransferUtility.UploadAsync(cmdLineParams.InputFile, cmdLineParams.BucketName);
                }
            }
            catch (Exception e)
            {
                rc = e.Message;
            }
            return rc;
        }
    }
}
