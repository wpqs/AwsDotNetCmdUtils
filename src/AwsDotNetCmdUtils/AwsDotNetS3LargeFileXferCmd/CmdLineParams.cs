using System;
using System.Collections.Generic;
using System.Text;

namespace AwsDotNetS3LargeFileXferCmd
{
    public abstract class CmdLineParams
    {
        protected abstract bool ProcParam(string param);
        protected abstract int GetArgsForParam(string arg);
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
            IsError = false;
            if ((args == null) || (args.Length < 1))
                SetErrorMsg($"Command line has no parameters {Environment.NewLine}{GetParamHelp()}");
            else
            {
                var paramLine = "";
                var paramName = "";
                var argsForParam = 0;
                var argCount = 0;
                foreach (var arg in args)
                {
                    if (arg.IndexOf("--") == 0)
                    {
                        if (argCount < argsForParam)
                        {
                            SetErrorMsg($"Too few arguments for parameter {paramName} {Environment.NewLine}{GetParamHelp()}");
                            break;
                        }
                        else
                        {
                            argCount = 0;
                            paramName = arg;
                            paramLine = paramName;
                            argsForParam = GetArgsForParam(arg);
                        }
                    }
                    if (argCount == argsForParam)
                    {
                        if (argCount > 0)
                            paramLine += " " + arg;
                        if (ProcParam(paramLine) == false)
                            break;
                    }
                    else if (argCount > argsForParam)
                    {
                        SetErrorMsg($"Too many arguments for parameter {paramName} {Environment.NewLine}{GetParamHelp()}");
                        break;
                    }
                    else
                    {
                        if (argCount > 0)
                            paramLine += " " + arg;
                    }
                    argCount++;
                }
                if (IsError == false)
                    ValidateParams();
            }
        }

        protected string GetParamValue(string param)
        {
            string rc = "";
            if (param != null)
            {
                int offset = param.IndexOf(' ');
                if ((offset >= 0) && (param.Length > offset +1))
                {
                    rc = param.Substring(offset + 1);
                }
            }
            return rc;
        }
    }
}
