using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace WPF.Assignment
{
    public class Util
    {
        public static string BaseUri
        {
            get
            {
                return ConfigurationManager.AppSettings["searchUri"];
            }
        }

        public static string NLMSearchUri
        {
            get
            {
                return ConfigurationManager.AppSettings["NLMSearchUri"];
            }
        }

        public static string NLMLookUpBaseUri
        {
            get
            {
                return ConfigurationManager.AppSettings["NLMLookUpBaseUri"];
            }
        }

        public static string SuggestUri
        {
            get
            {
                return ConfigurationManager.AppSettings["suggestUri"];
            }
        }

        public static string NLMSuggestUri
        {
            get
            {
                return ConfigurationManager.AppSettings["NLMSuggestUri"];
            }
        }

        public static bool IsValid(string data)
        {
            if (Regex.IsMatch(data, @"^[a-zA-Z0-9\s.\?\,\'\;\:\!\-\&]+$"))
                return true;
            else
                return false;
        }

        internal static void LogMessage(Exception ex)
        {
            EventLog.WriteEntry("Application", ex.ToString());
        }
    }

    [Serializable]
    public class ValidationException : ApplicationException
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
    }
}
