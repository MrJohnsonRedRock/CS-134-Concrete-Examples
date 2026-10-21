using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

// LessonUpdater — the CIT134 > Get Latest Lesson menu item.
//
// Students should never have to resolve a merge conflict to get today's lesson.
// So before pulling, this sets aside ("stashes") every change in the project:
// uncommented lines, a changed Step number, a moved object. Nothing is deleted;
// git keeps the stash, and `git stash list` shows it. Then it pulls.
//
// Assets/Playground/ is gitignored, so students' own work there is never stashed
// and never touched by a pull.
//
// Students clone the project once themselves (see STUDENTS.md), then use this
// button at the start of every class.
//
// A project with a .labsync-skip file at its root (the instructor's authoring
// copy) is never stashed or pulled.

public static class LessonUpdater
{
    private const int TimeoutMs = 60000;

    // git stash makes a commit, and a student who never set up git has no name.
    private const string Identity = "-c user.name=\"CS134 Student\" -c user.email=student@localhost ";

    [MenuItem("CIT134/Get Latest Lesson")]
    private static void GetLatestLesson()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Get Latest Lesson", "Stop Play mode first, then try again.", "OK");
            return;
        }

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        if (File.Exists(Path.Combine(projectRoot, ".labsync-skip")))
        {
            EditorUtility.DisplayDialog("Get Latest Lesson",
                "This is the instructor copy (.labsync-skip), so nothing was stashed or pulled.", "OK");
            return;
        }

        bool proceed = EditorUtility.DisplayDialog(
            "Get Latest Lesson",
            "This sets aside every change you've made to the class project (anything outside "
            + "Assets/Playground), then downloads the latest lesson.\n\n"
            + "Your changes are stashed in git, not deleted.",
            "Get Latest Lesson",
            "Cancel");
        if (!proceed) { return; }

        // Unsaved scene edits live only in memory; save them so the stash can hold them.
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; }

        GitResult stash = RunGit(projectRoot,
            Identity + "stash push --include-untracked -m \"Get Latest Lesson " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\"");
        if (stash.gitMissing)
        {
            EditorUtility.DisplayDialog("Get Latest Lesson",
                "Unity can't find git on this computer. Show this to your instructor.", "OK");
            return;
        }
        if (stash.exitCode != 0)
        {
            ShowFailure("Setting your changes aside failed.", stash);
            return;
        }

        GitResult pull = RunGit(projectRoot, "pull --ff-only");
        if (pull.exitCode != 0)
        {
            ShowFailure("Downloading the lesson failed. Your changes are safe in the stash.", pull);
            return;
        }

        AssetDatabase.Refresh();

        bool stashedSomething = !stash.output.Contains("No local changes");
        string message = "Got the latest lesson.";
        if (pull.output.Contains("Already up to date"))
        {
            message = "You already have the latest lesson.";
        }
        if (stashedSomething)
        {
            message += "\n\nYour earlier changes were set aside (git stash list).";
        }

        Debug.Log("Get Latest Lesson:\n" + stash.output + "\n" + pull.output);
        EditorUtility.DisplayDialog("Get Latest Lesson", message, "OK");
    }

    private struct GitResult
    {
        public bool gitMissing;
        public int exitCode;
        public string output;
    }

    private static GitResult RunGit(string workingDirectory, string arguments)
    {
        GitResult result = new GitResult();

        ProcessStartInfo info = new ProcessStartInfo("git", arguments);
        info.WorkingDirectory = workingDirectory;
        info.UseShellExecute = false;
        info.CreateNoWindow = true;
        info.RedirectStandardOutput = true;
        info.RedirectStandardError = true;
        info.EnvironmentVariables["GIT_TERMINAL_PROMPT"] = "0";   // never hang on a password prompt

        output = new StringBuilder();
        try
        {
            using (Process process = new Process())
            {
                process.StartInfo = info;
                process.OutputDataReceived += CollectLine;
                process.ErrorDataReceived += CollectLine;   // git writes progress to stderr
                EditorUtility.DisplayProgressBar("Get Latest Lesson", "git " + arguments, 0.5f);
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (!process.WaitForExit(TimeoutMs))
                {
                    process.Kill();
                    result.exitCode = -1;
                    lock (output)
                    {
                        output.AppendLine("git took too long and was stopped. Check your internet connection.");
                    }
                }
                else
                {
                    process.WaitForExit();   // flush the async readers
                    result.exitCode = process.ExitCode;
                }
            }
        }
        catch (Win32Exception)
        {
            result.gitMissing = true;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        lock (output)
        {
            result.output = output.ToString();
        }
        return result;
    }

    // Both git output streams arrive on background threads, so lock while appending.
    private static StringBuilder output = new StringBuilder();

    private static void CollectLine(object sender, DataReceivedEventArgs e)
    {
        if (e.Data == null) { return; }
        lock (output)
        {
            output.AppendLine(e.Data);
        }
    }

    private static void ShowFailure(string headline, GitResult result)
    {
        Debug.LogWarning("Get Latest Lesson: " + headline + "\n" + result.output);
        EditorUtility.DisplayDialog(
            "Get Latest Lesson",
            headline + "\n\n" + result.output + "\nShow this to your instructor.",
            "OK");
    }
}
