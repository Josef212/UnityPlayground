using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Response
{
    public string ErrorKey { get; private set; }
    public string ErrorValue { get; private set; }
    public bool IsError { get; private set; }
    // TODO: Will use a simple string to support the JSON error data for now
    public string AdditionalData { get; private set; }

    public Response(string errorData, string additionalData = null)
    {
        // TODO: Should recieve this data as JSON object and parse it 

        IsError = false;

        if(errorData != null)
        {
            ErrorKey = "ERROR";
            ErrorValue = errorData;
            IsError = true;
        }

        AdditionalData = additionalData;
    }
}
