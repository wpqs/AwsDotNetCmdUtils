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


        public OpMode Op { private set; get; }
        public string InputFile { private set; get; }
        public string OutputFile { private set; get; }
        public string BucketName { private set; get; }
        public string BucketRegion { private set; get; }

        private enum Param
        {
            Help = 0,
            Op,
            BucketRegion,
            BucketName,
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
                                SetErrorMsg($"Error: Parameter value is null or empty {Environment.NewLine}{GetParamHelp((int)Param.Op)}");
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
                                SetErrorMsg($"Error: Parameter value is null or empty {Environment.NewLine}{GetParamHelp((int)Param.Op)}");
                            else
                            {
                                Op = OpMode.Upload;
                                rc = true;
                            }
                        }
                        else
                            SetErrorMsg($"Error: Invalid argument {value} found in {paramLine}{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                    }
                }
                break;
                case Param.BucketRegion:
                {
                    int argCnt = GetArgCount(paramLine);
                    if (argCnt != 1)
                        SetErrorMsg($"Error: Incorrect number of arguments for parameter {paramLine}, found {argCnt} should be 1 {Environment.NewLine}{GetParamHelp((int)Param.BucketRegion)}");
                    else
                    {
                        BucketRegion = GetArgValue(paramLine, 1);
                        if (string.IsNullOrEmpty(BucketRegion))
                            SetErrorMsg($"Error: Parameter value is null or empty {Environment.NewLine}{GetParamHelp((int) Param.BucketRegion)}");
                        else
                            rc = true;
                    }
                }
                    break;
                case Param.BucketName:
                {
                    int argCnt = GetArgCount(paramLine);
                    if (argCnt != 1)
                        SetErrorMsg($"Error: Incorrect number of arguments for parameter {paramLine}, found {argCnt} should be 1 {Environment.NewLine}{GetParamHelp((int)Param.BucketName)}");
                    else
                    {
                        BucketName = GetArgValue(paramLine, 1);
                        if (string.IsNullOrEmpty(BucketName))
                            SetErrorMsg($"Error: Parameter value is null or empty {Environment.NewLine}{GetParamHelp((int) Param.BucketName)}");
                        else
                            rc = true;
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

            if (string.IsNullOrEmpty(BucketRegion))
                SetErrorMsg($"Error: Missing {BucketRegionParam}{Environment.NewLine}{GetParamHelp((int)Param.BucketRegion)}");
            else
            {
                if (string.IsNullOrEmpty(BucketName))
                    SetErrorMsg($"Error: Missing {BucketNameParam}{Environment.NewLine}{GetParamHelp((int)Param.BucketName)}");
                else
                {
                    if (Op == OpMode.Download)
                    {
                        if (string.IsNullOrEmpty(OutputFile))
                            SetErrorMsg($"Error: Filename argument missing{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                    }
                    else if (Op == OpMode.Upload)
                    {
                        if (string.IsNullOrEmpty(InputFile))
                            SetErrorMsg($"Error: Filename argument missing{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
                    }
                    else
                    {
                        SetErrorMsg($"Error: Missing {OpParam}{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
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
            else if (param == HelpParam)
                rc = Param.Help;
            else
                rc = Param.Unknown;

            return rc;
        }
    }
}
