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

            Console.WriteLine($"AwsDotNetS3LargeFileXferCmd {GetVersion()}.{Environment.NewLine}Copyright 2019 Will Stott.{Environment.NewLine}Use subject to standard MIT License - see https://github.com/wpqs/AwsDotNetCmdUtils {Environment.NewLine}");

            DateTime tim = DateTime.UtcNow;
            var cmdLineParams = new CmdLineParamsApp(args);
            if (cmdLineParams.IsError)
                Console.WriteLine($"{cmdLineParams.GetErrorMsg()}");
            else
            {
                string errMsg = null;
                if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Upload)
                {
                    errMsg = await FileTransferUpload(cmdLineParams);
                }
                else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Download)
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

            Console.WriteLine((rc == 0) ? $"program ends : return code {rc}" : $"program abends: return code {rc}");
            return rc;
        }

        private static string GetVersion()
        {
            string rc = "v";

            rc += typeof(Program)?.GetTypeInfo()?.Assembly?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
            return rc;
        }

        private static object GetJobDetails(CmdLineParamsApp cmdLineParams, TimeSpan elapsed)
        {
            string rc = "";

            double lengthMB = 0.0;
            if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Download)
            {
                lengthMB = ((double)new System.IO.FileInfo(cmdLineParams.OutputFile).Length) / 1048576;
                rc = $"download of {Path.GetFileName(cmdLineParams.OutputFile)} {string.Format("({0:0.##} MB) ", lengthMB)}";
                rc += string.Format("completed in {0} hour {1} minutes {2}.{3} seconds ", elapsed.Hours, elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);
                if (elapsed.TotalSeconds > 0)
                    rc += string.Format("- transfer speed {0:0.##} MBPS", (lengthMB * 8) / elapsed.TotalSeconds);
            }
            else if (cmdLineParams.Op == CmdLineParamsApp.OpMode.Upload)
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
            rc += Environment.NewLine;
            return rc;
        }

        private static async Task<string> FileTransferDownload(CmdLineParamsApp cmdLineParams)
        {
            string rc = null;

            try
            {
                var folder = Path.GetDirectoryName(cmdLineParams.OutputFile);
                if (String.IsNullOrEmpty(folder) || (Directory.Exists(folder) == false))
                    rc = $"folder for output file {cmdLineParams.OutputFile} does not exist. Create folder and try again.";
                else
                {
                    if ((cmdLineParams.Overwrite == false) && (File.Exists(cmdLineParams.OutputFile)))
                        rc = $"output file {cmdLineParams.OutputFile} already exists. Add {CmdLineParamsApp.OpArgOverwrite} argument to command and try again ";
                    else
                    {
                        var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(cmdLineParams.BucketRegion));

                        var config = new TransferUtilityConfig
                        {
                            MinSizeBeforePartUpload = cmdLineParams.PartialSize,
                            ConcurrentServiceRequests = cmdLineParams.Threads
                        };
                        var fileTransferUtility = new TransferUtility(s3Client, config);

                        var fileTransferRequest = new TransferUtilityDownloadRequest
                        {
                            FilePath = cmdLineParams.OutputFile,
                            BucketName = cmdLineParams.BucketName,
                            Key = Path.GetFileName(cmdLineParams.OutputFile)
                        };

                        Console.Write($"downloading '{cmdLineParams.OutputFile}' from '{cmdLineParams.BucketName}'...");
                        await fileTransferUtility.DownloadAsync(fileTransferRequest);
                        Console.WriteLine("...done.");
                    }
                }
            }
            catch (Exception e)
            {
                rc = $"{Environment.NewLine}Error: Exception: {e.Message}{Environment.NewLine}";
            }
            return rc;
        }

        private static async Task<string> FileTransferUpload(CmdLineParamsApp cmdLineParams)
        {
            string rc = null;

            try
            {
                if (File.Exists(cmdLineParams.InputFile) == false)
                    rc = $"input file {cmdLineParams.InputFile} does not exist";
                else
                {
                    var s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(cmdLineParams.BucketRegion));

                    var config = new TransferUtilityConfig
                    {
                        MinSizeBeforePartUpload = cmdLineParams.PartialSize, 
                        ConcurrentServiceRequests = cmdLineParams.Threads 
                    };
                    var fileTransferUtility = new TransferUtility(s3Client, config);

                    Console.Write($"uploading '{cmdLineParams.InputFile}' to '{cmdLineParams.BucketName}'...");
                    await fileTransferUtility.UploadAsync(cmdLineParams.InputFile, cmdLineParams.BucketName);
                    Console.WriteLine("...done.");
                }
            }
            catch (Exception e)
            {
                rc = $"{Environment.NewLine}Error: Exception: {e.Message}{Environment.NewLine}";
            }
            return rc;
        }
    }
}
