using System;
using System.Collections;
using System.Reflection;

public static class Utils
{
    public static uint sessionIDSize = 6;
    public static uint playerIDSize = 4;

    public static string[] SplitLoginInfo(string loginInfo)
    {
        string[] splitInfo = new string[2];
        try
        {
            int j = 0;
            for (int i = 0; i < loginInfo.Length; i++)
            {
                if (loginInfo[i] == '_')
                    j++;
                else
                    splitInfo[j] += loginInfo[i];
            }
        }
        catch(Exception e) { Console.WriteLine(e.Message); }
        return splitInfo;
    }

    public static string GenerateUID(Game.UID_TYPE type)
    {
        // Run of separate thread
        uint[] idSizes = { playerIDSize, sessionIDSize };
        string rndId = string.Empty;
        for (int i = 0; i < idSizes[(int)type]; i++)
        {
            rndId += new Random().Next(0, 9);
        }
        return rndId;
    }

    // Get class from data structure by property value
    public static T GetClassByPropertyValue<T>(T[] arr, string propertyIdent, object value)
    {
        foreach(T t in arr)
        {
            PropertyInfo? pInf = t.GetType().GetProperty(propertyIdent);
            object? pVal = pInf.GetValue(t, null);
            if (value.Equals(pVal))
            {
                return (T)t;
            }
        }
        return default(T);
    }
}
