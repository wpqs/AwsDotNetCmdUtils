using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AwsDotNetS3LargeFileXferCmd
{
    public class CmdLineParamsProc : CmdLineParams
    {
        public static readonly string HelpParam = "--help";
        public static readonly string OpParam = "--operation";
        public static readonly string OpValueUpload = "upload";
        public static readonly string OpValueDownload = "download";
        public static readonly string BucketNameParam = "--bucketname";
        public static readonly string BucketRegionParam = "--bucketregion";
        public static readonly string ThreadsParam = "--threads";
        public static readonly int    ThreadsValueDefault = 10;
        public static readonly int    ThreadsValueMin = 1;
        public static readonly int    ThreadsValueMax = 100;
        public static readonly string PartialSizeParam = "--partialsize";
        public static readonly long PartialSizeValueDefault = 16777216; //2**24
        public static readonly long PartialSizeValueMin = 4096;         //2**12
        public static readonly long PartialSizeValueMax = 4294967296;   //2**32

        public OpMode Op { private set; get; }
        public string InputFile { private set; get; }
        public string OutputFile { private set; get; }
        public string BucketName { private set; get; }
        public string BucketRegion { private set; get; }
        public int Threads { private set; get; }
        public long PartialSize { private set; get; }


        private enum Param
        {
            Help = 0,
            Op,
            BucketRegion,
            BucketName,
            Threads,
            PartialSize,
            Unknown
        }

        public enum OpMode
        {
            Upload = 0,
            Download,
            Unknown
        }

        public CmdLineParamsProc(string[] cmdLine = null) : base(cmdLine)
        {
        }

        protected override void SetDefaultValues()  //called from base class as values may be overwritten by values passed from cmdLine
        {
            IsError = false;
            Threads = ThreadsValueDefault;
            PartialSize = PartialSizeValueDefault;
        }

        protected override bool ProcParam(string paramLine)
        {
            bool rc = false;

            switch (GetParamType(paramLine))
            {
                case Param.Help:
                {
                    int argCnt = GetArgCount(paramLine);
                    if (argCnt != 0)
                        SetErrorMsg($"Error: Incorrect number of arguments for parameter, found {argCnt} should be 0.{Environment.NewLine}{GetParamHelp((int)Param.Help)}");
                    else
                        SetErrorMsg($"Help request:{Environment.NewLine}{GetParamHelp((int) Param.Help)}");
                }
                break;
                case Param.Op:
                {
                    int argCnt = GetArgCount(paramLine);
                    if (argCnt != 2)
                        SetErrorMsg($"Error: Incorrect number of arguments for parameter, found {argCnt} should be 2.{Environment.NewLine}{paramLine}{Environment.NewLine}{GetParamHelp((int)Param.Op)}");
                    else
                    {
                        var value = GetArgValue(paramLine, 1)?.ToLower() ?? "[not found]";
                        if (value == OpValueDownload)
                        {
                            OutputFile = GetArgValue(paramLine, 2);
                            if (string.IsNullOrEmpty(OutputFile))
                                SetErrorMsg($"Error: Second argument value in {paramLine} is empty or missing {Environment.NewLine}{GetParamHelp((int)Param.Op)}");
                            else
                            {
                                Op = OpMode.Download;
                                rc = true;
                            }
                        }
                        else if (value == OpValueUpload)
                        {
                            InputFile = GetArgValue(paramLine, 2);
                            if (string.IsNullOrEmpty(InputFile))
                                SetErrorMsg($"Error: Second argument value in {paramLine} is empty or missing {Environment.NewLine}{GetParamHelp((int)Param.Op)}");
                            else
                            {
                                Op = OpMode.Upload;
                                rc = true;
                            }
                        }
                        else
                            SetErrorMsg($"Error: First argument value in {paramLine} is invalid; {value}{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                    }
                }
                break;
                case Param.BucketRegion:
                {
                    int argCnt = GetArgCount(paramLine);
                    if (argCnt != 1)
                        SetErrorMsg($"Error: Incorrect number of arguments in parameter {paramLine}, found {argCnt} should be 1 {Environment.NewLine}{GetParamHelp((int)Param.BucketRegion)}");
                    else
                    {
                        BucketRegion = GetArgValue(paramLine, 1);
                        if (string.IsNullOrEmpty(BucketRegion))
                            SetErrorMsg($"Error: First argument value in {paramLine} is null or empty {Environment.NewLine}{GetParamHelp((int) Param.BucketRegion)}");
                        else
                            rc = true;
                    }
                }
                    break;
                case Param.BucketName:
                {
                    int argCnt = GetArgCount(paramLine);
                    if (argCnt != 1)
                        SetErrorMsg($"Error: Incorrect number of arguments in {paramLine}, found {argCnt} should be 1 {Environment.NewLine}{GetParamHelp((int)Param.BucketName)}");
                    else
                    {
                        BucketName = GetArgValue(paramLine, 1);
                        if (string.IsNullOrEmpty(BucketName))
                            SetErrorMsg($"Error: First argument value in {paramLine} is null or empty {Environment.NewLine}{GetParamHelp((int) Param.BucketName)}");
                        else
                            rc = true;
                    }
                }
                break;
                case Param.Threads:
                {
                    int argCnt = GetArgCount(paramLine);
                    if (argCnt != 1)
                        SetErrorMsg($"Error: Incorrect number of arguments for parameter {paramLine}, found {argCnt} should be 1 {Environment.NewLine}{GetParamHelp((int)Param.Threads)}");
                    else
                    {
                        var threadsCount = GetArgValue(paramLine, 1);
                        if (string.IsNullOrEmpty(threadsCount))
                            SetErrorMsg($"Error: Argument value in {paramLine} is null or empty {Environment.NewLine}{GetParamHelp((int)Param.Threads)}");
                        else
                        {
                            int threadCnt = 0;
                            if (Int32.TryParse(threadsCount, out threadCnt) == false)
                                SetErrorMsg($"Error: Parameter value {threadsCount} is not a valid number {Environment.NewLine}{GetParamHelp((int)Param.Threads)}");
                            else
                            {
                                Threads = threadCnt;
                                rc = true;
                            }
                        }
                    }
                }
                break;
                case Param.PartialSize:
                {
                    int argCnt = GetArgCount(paramLine);
                    if (argCnt != 1)
                        SetErrorMsg($"Error: Incorrect number of arguments for parameter {paramLine}, found {argCnt} should be 1 {Environment.NewLine}{GetParamHelp((int)Param.PartialSize)}");
                    else
                    {
                        var partialSize = GetArgValue(paramLine, 1);
                        if (string.IsNullOrEmpty(partialSize))
                            SetErrorMsg($"Error: Argument value in {paramLine} is null or empty {Environment.NewLine}{GetParamHelp((int)Param.PartialSize)}");
                        else
                        {
                            long size = 0L;
                            if (long.TryParse(partialSize, out size) == false)
                                SetErrorMsg($"Error: Parameter value {partialSize} is not a valid number {Environment.NewLine}{GetParamHelp((int)Param.PartialSize)}");
                            else
                            {
                                PartialSize = size;
                                rc = true;
                            }
                        }
                    }
                }
                break;
                default: //case Param.Unknown:
                {
                    SetErrorMsg($"Error: Unknown parameter {paramLine}{Environment.NewLine}{GetParamHelp()}");
                }
                break;
            }
            return rc;
        }

        protected override void ValidateParams()
        {
            IsError = false;

            if ((Threads < ThreadsValueMin) || (Threads > ThreadsValueMax))
                SetErrorMsg($"Error: {ThreadsParam} value {Threads.ToString()} is invalid {Environment.NewLine}{GetParamHelp((int)Param.Threads)}");
            else
            {
                if ((PartialSize < PartialSizeValueMin) || (PartialSize > PartialSizeValueMax))
                    SetErrorMsg($"Error: {PartialSizeParam} value {PartialSize.ToString()} is invalid {Environment.NewLine}{GetParamHelp((int)Param.PartialSize)}");
                else
                {
                    if (string.IsNullOrEmpty(BucketRegion))
                        SetErrorMsg(
                            $"Error: Missing {BucketRegionParam}{Environment.NewLine}{GetParamHelp((int) Param.BucketRegion)}");
                    else
                    {
                        if (string.IsNullOrEmpty(BucketName))
                            SetErrorMsg(
                                $"Error: Missing {BucketNameParam}{Environment.NewLine}{GetParamHelp((int) Param.BucketName)}");
                        else
                        {

                            if (Op == OpMode.Download)
                            {
                                if (string.IsNullOrEmpty(OutputFile))
                                    SetErrorMsg(
                                        $"Error: Filename argument missing{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                            }
                            else if (Op == OpMode.Upload)
                            {
                                if (string.IsNullOrEmpty(InputFile))
                                    SetErrorMsg(
                                        $"Error: Filename argument missing{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                            }
                            else
                            {
                                SetErrorMsg(
                                    $"Error: Missing {OpParam}{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                            }
                        }
                    }
                }
            }
        }
        protected override string GetParamHelp(int paramId = 0)
        {
            string rc = null;

            Param help = (Param)paramId;
            if (help == Param.Help)
            {
                rc = $"{Environment.NewLine}Hint: retry using program's expected parameters and their arguments which are:";
                rc += Environment.NewLine;
                rc += $"{BucketRegionParam} us-east-1 ";
                rc += $"{BucketNameParam} name ";
                rc += $"{OpParam} [{OpValueUpload} 'drive:path\\filename'";
                rc += $" | {OpValueDownload} 'drive:path\\filename']";
                rc += Environment.NewLine;
                rc += $"({ThreadsParam} {ThreadsValueDefault.ToString()} <min {ThreadsValueMin} max {ThreadsValueMax}>) ";
                rc += $"({PartialSizeParam} {PartialSizeValueDefault.ToString()} <min {PartialSizeValueMin} max {PartialSizeValueMax}> )";
                rc += GetHelpNotes();

            }
            else if (help == Param.Op)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected arguments for the parameter.{Environment.NewLine}";
                rc += $"{OpParam} [{OpValueUpload} 'drive:path\\filename' | {OpValueDownload} 'drive:path\\filename']";
                rc += GetHelpNotes();
            }
            else if (help == Param.BucketRegion)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected argument for the parameter.{Environment.NewLine}";
                rc += $"{BucketRegionParam} us-east-1";
                rc += GetHelpNotes();
            }
            else if (help == Param.BucketName)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected argument for the parameter.{Environment.NewLine}";
                rc += $"{BucketNameParam} name";
                rc += GetHelpNotes();
            }
            else if (help == Param.Threads)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected argument for the parameter.{Environment.NewLine}";
                rc += $"{ThreadsParam} {ThreadsValueDefault} <min {ThreadsValueMin} max {ThreadsValueMax}>";
                rc += GetHelpNotes();
            }
            else if (help == Param.PartialSize)
            {
                rc = $"{Environment.NewLine}Hint: retry using expected argument for the parameter.{Environment.NewLine}";
                rc += $"{PartialSizeParam} {PartialSizeValueDefault} <min {PartialSizeValueMin} max {PartialSizeValueMax}>";
                rc += GetHelpNotes();
            }
            else
            {
                rc = $"{Environment.NewLine}Program error: please report this problem";
            }
            return rc;
        }

        private string GetHelpNotes()
        {
            string rc = "";

            rc += Environment.NewLine;
            rc += Environment.NewLine;
            rc += "Notes:";
            rc += Environment.NewLine;
            rc += "   --a 1 2 3 means parameter 'a' with arguments 1, 2 and 3.";
            rc += Environment.NewLine;
            rc += "   [--a ... | --b ...] means enter either parameter a or b. --a [1 ... | 2 ...] means enter either argument 1 or 2.";
            rc += Environment.NewLine;
            rc += "   (--c ...) means parameter c is optional. --c 1 (2) means argument 2 is optional";
            rc += Environment.NewLine;
            rc += "   --d 5 <min 0 max 10> means argument for parameter d is a number in range 0-10 with a default value of 5";
            rc += Environment.NewLine;

            return rc;
        }

        private Param GetParamType(string param)
        {
            Param rc = Param.Unknown;

            int offset = param.IndexOf(' ');
            param = (offset >= 0) ? param.Substring(0, offset)?.ToLower() : param.ToLower();

            if (param == OpParam)
                rc = Param.Op;
            else if (param == BucketRegionParam)
                rc = Param.BucketRegion;
            else if (param == BucketNameParam)
                rc = Param.BucketName;
            else if (param == ThreadsParam)
                rc = Param.Threads;
            else if (param == PartialSizeParam)
                rc = Param.PartialSize;
            else if (param == HelpParam)
                rc = Param.Help;
            else
                rc = Param.Unknown;

            return rc;
        }
    }
}
