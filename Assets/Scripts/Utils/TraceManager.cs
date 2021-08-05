using UnityEngine;

public enum TraceChannel
{
    Main = 0,
    Fine = 1
}

public enum TraceType
{
    error = 0,
    warning = 1,
    info = 2
}

public static class TraceManager
{
    private static int[] enableChannel = new int[2] { 1, 1 };

    public static void WriteTrace(TraceChannel channel, TraceType type, string text)
    {
        WriteTrace(channel, type, text, new object[0]);
    }

    public static void WriteTrace(TraceChannel channel, TraceType type, string text, object[] args)
    {
        if (type != TraceType.error && enableChannel[(int)channel] != 1) return;

        if (type == TraceType.error)
        {
            Debug.LogErrorFormat(text, args);
        }
        else if (type == TraceType.warning)
        {
            Debug.LogWarningFormat(text, args);
        }
        else if (type == TraceType.info)
        {
            Debug.Log(text);
        }
    }
}
