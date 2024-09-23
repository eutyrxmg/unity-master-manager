using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class Util {

    public static bool ExecuteCommand(string command, string arguments, string workingDirectory = null) {
        workingDirectory ??= Application.dataPath;

        ProcessStartInfo processStartInfo = new ProcessStartInfo {
            FileName = command,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory,
        };

        bool isSuccess = true;
        StringBuilder outputData = new();
        using var proc = new Process();
        using var cancelToken = new CancellationTokenSource();
        proc.EnableRaisingEvents = true;
        proc.StartInfo = processStartInfo;
        proc.OutputDataReceived += (sender, ev) => {
            if (ev.Data != null) {
                outputData.AppendLine(ev.Data);
            }
        };
        proc.ErrorDataReceived += (sender, ev) => {
            if (ev.Data != null) {
                outputData.AppendLine(ev.Data);
                isSuccess = false;
            }
        };
        proc.Exited += (sender, ev) => {
            // ReSharper disable once AccessToDisposedClosure
            cancelToken.Cancel();
        };
        proc.Start();
        proc.BeginErrorReadLine();
        proc.BeginOutputReadLine();
        cancelToken.Token.WaitHandle.WaitOne();

        if (outputData.Length != 0) {
            if (isSuccess) {
                Debug.Log(outputData);
            } else {
                Debug.LogError(outputData);
            }
        }
        Debug.Log($"process end. command={command} {arguments}");
        return isSuccess;
    }

    public static void SaveResource(string path, byte[] data) {
        if (!Directory.Exists(Const.AppResourceFolder)) {
            Directory.CreateDirectory(Const.AppResourceFolder);
        }
        File.WriteAllBytes(path, data);
        AssetDatabase.Refresh();
    }
}