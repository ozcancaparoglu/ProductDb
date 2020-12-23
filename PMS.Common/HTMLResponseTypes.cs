using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Common
{
    public class HTMLResponseTypes
    {
        public static string Response(int statusCode)
        {
            ResponseTypes enumDisplayStatus = (ResponseTypes)statusCode;
            string stringValue = enumDisplayStatus.ToString();
            if (statusCode.ToString().Substring(0, 1) == "2")
                return "Successful!";
            else
                return statusCode + " - " + stringValue.Replace("-", "");
        }

        public enum ResponseTypes
        {
            Ok = 200,
            Created = 201,
            Accepted = 202,
            Partial_Information = 203,
            No_Response = 204,
            Bad_Request = 400,
            Unauthorized = 401,
            Payment_Required = 402,
            Forbidden = 403,
            Not_Found = 404,
            Unprocessable_Object_Result = 422,
            Internal_Error = 500,
            Not_Implemented = 501,
            Service_Temporarily_Overloaded = 502,
            Gateway_timeout = 503,
            Moved = 301,
            Found = 302,
            Method = 303,
            Not_Modified = 304
        }
    }
}
