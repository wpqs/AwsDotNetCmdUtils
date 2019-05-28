using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AwsDotNetS3LargeFileXferCmd
{
    public class CmdLineParamsProc : CmdLineParams
    {
        public static readonly string OpParam = "--operation";
        public static readonly string InputFileParam = "--inputfile";
        public static readonly string OutputFileParam = "--outputfile";
        public static readonly string BucketNameParam = "--bucketname";
        public static readonly string BucketRegionParam = "--bucketregion";
        public static readonly string HelpParam = "--help";

        public static readonly string OpValueUpload = "upload";
        public static readonly string OpValueDownload = "download";

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
            InputFile,
            OutputFile,
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

        protected override bool ProcParam(string param)
        {
            bool rc = false;

            switch (GetParamType(param))
            {
                case Param.Help:
                {
                    SetErrorMsg(GetParamHelp((int) Param.Help));
                }
                break;
                case Param.Op:
                {
                    var value = GetParamValue(param).ToLower();
                    if (value == OpValueDownload)
                    {
                        Op = OpMode.Download;
                        rc = true;
                    }
                    else if (value == OpValueUpload)
                    {
                        Op = OpMode.Upload;
                        rc = true;
                    }
                    else
                        SetErrorMsg($"invalid value for parameter {value}{Environment.NewLine}{GetParamHelp((int)Param.Op)}");
                }
                break;
                case Param.BucketRegion:
                {
                    BucketRegion = GetParamValue(param);
                    if (string.IsNullOrEmpty(BucketRegion))
                        SetErrorMsg($"parameter value is null or empty {Environment.NewLine}{GetParamHelp((int)Param.BucketRegion)}");
                    else
                        rc = true;
                }
                    break;
                case Param.BucketName:
                {
                    BucketName = GetParamValue(param);
                    if (string.IsNullOrEmpty(BucketName))
                        SetErrorMsg($"parameter value is null or empty {Environment.NewLine}{GetParamHelp((int) Param.BucketName)}");
                    else
                        rc = true;
                }
                break;
                case Param.InputFile:
                {
                    InputFile = GetParamValue(param);
                    if (string.IsNullOrEmpty(InputFile))
                        SetErrorMsg($"parameter value is null or empty {Environment.NewLine}{GetParamHelp((int)Param.InputFile)}");
                    else
                        rc = true;
                }
                break;
                case Param.OutputFile:
                {
                    OutputFile = GetParamValue(param);
                    if (string.IsNullOrEmpty(OutputFile))
                        SetErrorMsg($"parameter value is null or empty {Environment.NewLine}{GetParamHelp((int)Param.OutputFile)}");
                    else
                        rc = true;
                }
                break;
                default: //case Param.Unknown:
                {
                    SetErrorMsg($"Unknown parameter {param}{Environment.NewLine}{GetParamHelp()}");
                }
                break;
            }

            return rc;
        }
        protected override void ValidateParams()
        {
            IsError = false;

            if (string.IsNullOrEmpty(BucketRegion))
                SetErrorMsg($"missing {BucketRegionParam}{Environment.NewLine}{GetParamHelp((int)Param.BucketRegion)}");
            else
            {
                if (string.IsNullOrEmpty(BucketName))
                    SetErrorMsg($"missing {BucketNameParam}{Environment.NewLine}{GetParamHelp((int)Param.BucketName)}");
                else
                {
                    if (Op == OpMode.Download)
                    {
                        if (string.IsNullOrEmpty(OutputFile))
                            SetErrorMsg($"missing {OutputFileParam}{Environment.NewLine}{GetParamHelp((int) Param.OutputFile)}");
                    }
                    else if (Op == OpMode.Upload)
                    {
                        if (string.IsNullOrEmpty(InputFile))
                            SetErrorMsg($"missing {InputFileParam}{Environment.NewLine}{GetParamHelp((int) Param.InputFile)}");
                    }
                    else
                    {
                        SetErrorMsg($"missing {OpParam}{Environment.NewLine}{GetParamHelp((int) Param.Op)}");
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
                rc = "hint expected params: ";
                rc += $"{OpParam} {OpValueUpload} | {OpValueDownload} ";
                rc += $"{BucketRegionParam} us-east-1 ";
                rc += $"{BucketNameParam} name ";
                rc += $"{InputFileParam} 'drive:path\\filename' ";
                rc += " | ";
                rc += $"{OutputFileParam} 'drive:path\\filename' ";
            }
            else if (help == Param.Op)
            {
                rc = "hint expected param: ";
                rc += $"{OpParam} {OpValueUpload} | {OpValueDownload} ";
            }
            else if (help == Param.BucketRegion)
            {
                rc = "hint expected param: ";
                rc += $"{BucketRegionParam} us-east-1";
            }
            else if (help == Param.BucketName)
            {
                rc = "hint expected param: ";
                rc += $"{BucketNameParam} name";
            }
            else if (help == Param.InputFile)
            {
                rc = "hint expected param: ";
                rc += $"{InputFileParam} 'drive:path\\filename' ";
            }
            else if (help == Param.OutputFile)
            {
                rc = "hint expected param: ";
                rc += $"{OutputFileParam} 'drive:path\\filename' ";
            }
            else
            {
                rc = "hint param is unknown";
            }
            return rc;
        }

        protected override int GetArgsForParam(string arg)
        {
            int rc = 0;

            switch (GetParamType(arg))
            {
                case Param.Help:
                    rc = 0;
                break;
                default:
                    rc = 1;
                break;
            }
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
            else if (param == InputFileParam)
                rc = Param.InputFile;
            else if (param == OutputFileParam)
                rc = Param.OutputFile;
            else if (param == HelpParam)
                rc = Param.Help;
            else
                rc = Param.Unknown;

            return rc;
        }
    }
}
