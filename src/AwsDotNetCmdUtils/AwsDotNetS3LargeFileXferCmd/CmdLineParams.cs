using System;
using System.Collections.Generic;
using System.Text;

namespace AwsDotNetS3LargeFileXferCmd
{
    public abstract class CmdLineParams
    {
        protected abstract void SetDefaultValues();
        protected abstract bool ProcParam(string paramLine);
        protected abstract string GetParamHelp(int paramId = 0);
        protected abstract void ValidateParams();

        public bool IsError { get; protected set; }

        private string _errMsg;
        public string GetErrorMsg()  { return _errMsg; }
        public void SetErrorMsg(string msg)
        {
            _errMsg = msg;
            IsError=true;
        }

        protected CmdLineParams(string[] args =null)
        {
            SetDefaultValues();

            if ((args == null) || (args.Length < 1))
                SetErrorMsg($"Error: Command line has no parameters {Environment.NewLine}{GetParamHelp()}");
            else
            {
                string cmdLine = "";
                foreach (var arg in args)
                    cmdLine += arg + " ";
                cmdLine = cmdLine.TrimEnd();

                var paramList = cmdLine.Split("--");
                foreach (var param in paramList)
                {
                    if (string.IsNullOrWhiteSpace(param) == false)
                    {
                        if (ProcParam("--" + param.TrimEnd()) == false)
                            break;
                    }
                }
                if (IsError == false)
                    ValidateParams();
            }
        }

        protected string GetArgValue(string paramLine, int argNumber=1)
        {
            string rc = null;
            if (paramLine != null)
            {
                int count = 0;
                int offset = 0;
                while ((offset = paramLine.IndexOf(' ', offset)) != -1)
                {
                    if ((++count == argNumber) && (paramLine.Length > offset + 1))
                    {
                        var argument = paramLine.Substring(offset + 1).TrimStart();
                        var end = argument.IndexOf(' ');
                        rc = end == -1 ? argument : argument.Substring(0, end);
                        break;
                    }
                    offset++;
                }
            }
            return rc;
        }

        protected int GetArgCount(string paramLine)
        {
            int rc = 0;
            int offset = 0;
            while ((offset = paramLine.IndexOf(' ', offset)) != -1)
            {
                rc++;
                offset++;
            }
            return rc;
        }
    }
}
