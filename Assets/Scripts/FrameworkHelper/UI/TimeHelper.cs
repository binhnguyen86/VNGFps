using System;
using System.Collections;
using System.Collections.Generic;

public class TimeHelper {

    public static DateTime dt1970 = new DateTime(1970, 1, 1);

    public static double ConvertTime(long timeStamp, string type = "second")
    {
        double result = 0;
        TimeSpan time = TimeSpan.FromMilliseconds(timeStamp);
        switch (type)
        {
            case "minute":
                result = time.TotalMinutes;
                break;
            case "hour":
                result = time.TotalHours;
                break;
            case "day":
                result = time.TotalDays;
                break;
            default:
                result = time.TotalSeconds;
                break;
        }
        return result;
    }

    public static string ConverSecondToText(int second)
    {

        int hour = second / 3600;
        int minutes = (second % 3600) / 60;
        int seconds = (second % 3600) % 60;
        string hourStr = "00";
        string minStr = "00";
        string secondStr = "00";
        if (hour < 10) hourStr = "0" + hour;
        else hourStr = hour.ToString();
        if (minutes < 10) minStr = "0" + minutes;
        else minStr = minutes.ToString();
        if (seconds < 10) secondStr = "0" + seconds;
        else secondStr = seconds.ToString();
        if (hour > 0)
            return hourStr + " : " + minStr + " : " + secondStr;
        else
            return minStr + " : " + secondStr;
    }

}
