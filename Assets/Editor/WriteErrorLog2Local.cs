using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Text;

/// <summary>
/// 将错误日志写入工程本地文件中，便于Cursor的分析查看
/// </summary>
/// 
[InitializeOnLoad]
public class WriteErrorLog2Local
{
    private static string LOG_FILE_PATH
    {
        get
        {
            return Path.Combine(Application.dataPath.Replace("Assets", ""), "ErrorLog.txt");
        }
    }

    private static StringBuilder mCachedErrorLog = new StringBuilder();
    private static int mResidualWriteCheckInterval = 0;
    private const int RESIDUAL_WRITE_CHECK_INTERVAL = 100;

    static WriteErrorLog2Local()
    {
        ClearFile();

        Application.logMessageReceivedThreaded += HandleLog;
        EditorApplication.update += OnUpdate;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void ClearFile()
    {
        mCachedErrorLog.Clear();
        mResidualWriteCheckInterval = RESIDUAL_WRITE_CHECK_INTERVAL;

        if (File.Exists(LOG_FILE_PATH))
            File.WriteAllTextAsync(LOG_FILE_PATH, string.Empty);
        else
            File.Create(LOG_FILE_PATH);
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.ExitingPlayMode)
        {
            ClearFile();
        }
    }

    private static void OnUpdate()
    {
        if (--mResidualWriteCheckInterval <= 0)
        {
            mResidualWriteCheckInterval = RESIDUAL_WRITE_CHECK_INTERVAL;
            string content = mCachedErrorLog.ToString();
            mCachedErrorLog.Clear();
            if (string.IsNullOrEmpty(content))
                return;
            File.AppendAllTextAsync(LOG_FILE_PATH, content);
        }
    }

    private static void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Log || type == LogType.Warning)
            return;

        string errorLog = $"{condition}\n{stackTrace}\n";
        mCachedErrorLog.Append(errorLog);
    }
}