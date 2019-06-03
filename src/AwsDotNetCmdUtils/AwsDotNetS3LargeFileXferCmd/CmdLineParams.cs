using System;
using System.Collections.Generic;
using System.Text;

namespace AwsDotNetS3LargeFileXferCmd
{
    public abstract class CmdLineParams
    {
        protected static readonly char quoteChar = '\'';
        protected static readonly char spaceChar = ' ';
        protected abstract void SetDefaultValues();
        protected abstract bool ParamProc(string paramLine);
        protected abstract string GetParamHelp(int paramId = 0);
        protected abstract void ValidateParams();

        public bool IsError { get; protected set; }

        private string _errMsg;
        public string GetErrorMsg()  { return _errMsg; }
        public void SetErrorMsg(string msg, bool overwrite=false)
        {
            if ((IsError == false) || (overwrite == true))
                _errMsg = msg;      //only set first error
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
                        if (ParamProc("--" + param.TrimEnd()) == false)
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
                bool firstQuote = false;
                char breakChar = spaceChar;
                int count = 1;
                int offset = 0;
                while ((offset = paramLine.IndexOf(breakChar, offset)) != -1)
                {
                    if ((count == argNumber) && (paramLine.Length > offset + 1))
                    {
                        var argument = paramLine.Substring(offset + 1).TrimStart();
                        if (argument.StartsWith(quoteChar) == false)
                        {
                            var end = argument.IndexOf(spaceChar);
                            if (end != -1) 
                                argument = argument.Substring(0, end);
                            rc = argument.TrimEnd();
                        }
                        else
                        {
                            var end = argument.IndexOf(quoteChar, 1);
                            if (end == -1)
                                SetErrorMsg($"Error: Invalid argument {argNumber} in \"{paramLine}\" - no closing quote character {Environment.NewLine}{GetParamHelp()}");
                            else
                            {
                                argument = argument.Substring(1, end-1);
                                if (argument.Length < 1)
                                    SetErrorMsg($"Error: Invalid argument {argNumber} in \"{paramLine}\" - nothing between the quotes {Environment.NewLine}{GetParamHelp()}");
                                else
                                    rc = argument.TrimEnd();
                            }
                        }
                        break;
                    }
                    while (paramLine[offset] == spaceChar) 
                        offset++;
                    if ((paramLine[offset] != quoteChar) || (offset + 1 >= paramLine.Length))
                    {
                        breakChar = spaceChar;
                        count++;
                    }
                    else
                    {
                        if (firstQuote == true)
                        {
                            breakChar = spaceChar;
                            count++;
                            firstQuote = false;
                        }
                        else
                        {
                            breakChar = quoteChar;
                            firstQuote = true;
                        }
                        offset++;
                    }
                }
            }
            return rc;
        }

        protected int GetArgCount(string paramLine)
        {
            int count = 0;
            while (GetArgValue(paramLine, count+1) != null)
                count++;
            return (IsError == false) ? count : -1;
        }

        protected static string GetHelpNotes()
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
            rc += "   (--c ...) means parameter c is optional. --c 1 (2) means argument 2 is optional.";
            rc += Environment.NewLine;
            rc += "   --d 5 <min 0 max 10> means argument for parameter d is a number in range 0-10 with a default value of 5.";
            rc += Environment.NewLine;
            rc += "   --e 1 '2 x' 3 means there are three arguments for parameter e; 1, '2 x' and 3. The second argument contains a space.";
            rc += Environment.NewLine;
            return rc;
        }
    }
}
