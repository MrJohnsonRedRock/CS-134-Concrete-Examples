using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

// DemoHUD — the shared on-screen readout every session demo writes to.
//
// From any demo script:
//     DemoHUD.SetTitle("Session 16 — static means no this");
//     DemoHUD.Show("Enemies alive", enemyCount);   // a row that updates in place
//     DemoHUD.Log("Constructor B ran");            // a line in the scrolling log
//
// Turn on "Show Memory Stats" in the Inspector for demos about allocation and GC.
// Those numbers come from Unity's profiler, so they only work in the Editor or in
// a Development Build. The HUD itself allocates a little every time it redraws;
// that cost is the same no matter which demo is running, so compare demos
// against each other, not against zero.
//
// Press H to hide or show the HUD.

[DefaultExecutionOrder(-1000)]   // wake up before any demo tries to write to us
public class DemoHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text hudText;
    [SerializeField] private bool showMemoryStats = false;
    [SerializeField] private int maxLogLines = 8;
    [SerializeField] private float redrawInterval = 0.25f;
    [SerializeField] private KeyCode toggleKey = KeyCode.H;

    // Session 16: a static field belongs to the type, not to any one HUD.
    // That is what lets a demo write DemoHUD.Show(...) without finding the HUD first.
    // It is also a GC root, so we clear it in OnDestroy; otherwise this HUD would
    // stay reachable after its scene unloads.
    private static DemoHUD instance;
    private static bool warnedMissing = false;

    private string title = "";
    private List<string> rowLabels = new List<string>();
    private Dictionary<string, string> rowValues = new Dictionary<string, string>();
    private Queue<string> logLines = new Queue<string>();
    private StringBuilder builder = new StringBuilder(1024);

    private bool needsRedraw = true;
    private float nextRedrawTime = 0f;

    // Memory stats
    private Recorder allocRecorder;
    private int allocationsThisSecond = 0;
    private int allocationsLastSecond = 0;
    private float secondStartTime = 0f;

    // ---------- Static API: what demos call ----------

    public static void SetTitle(string newTitle)
    {
        if (!CheckInstance()) { return; }
        instance.title = newTitle;
        instance.needsRedraw = true;
    }

    public static void Show(string label, string value)
    {
        if (!CheckInstance()) { return; }

        if (!instance.rowValues.ContainsKey(label))
        {
            instance.rowLabels.Add(label);   // remember the order rows first appeared
        }
        instance.rowValues[label] = value;
        instance.needsRedraw = true;
    }

    public static void Show(string label, int value)
    {
        Show(label, value.ToString());
    }

    public static void Show(string label, float value)
    {
        Show(label, value.ToString("0.00"));
    }

    public static void Show(string label, bool value)
    {
        Show(label, value ? "true" : "false");
    }

    public static void Log(string message)
    {
        Debug.Log(message);   // the Console gets a copy too
        if (!CheckInstance()) { return; }

        instance.logLines.Enqueue(message);
        while (instance.logLines.Count > instance.maxLogLines)
        {
            instance.logLines.Dequeue();
        }
        instance.needsRedraw = true;
    }

    public static void Clear()
    {
        if (!CheckInstance()) { return; }
        instance.title = "";
        instance.rowLabels.Clear();
        instance.rowValues.Clear();
        instance.logLines.Clear();
        instance.needsRedraw = true;
    }

    private static bool CheckInstance()
    {
        if (instance != null) { return true; }

        if (!warnedMissing)
        {
            Debug.LogWarning("DemoHUD: no DemoHUD in this scene. Messages go to the Console only.");
            warnedMissing = true;
        }
        return false;
    }

    // ---------- Unity messages ----------

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("DemoHUD: a second DemoHUD was found and disabled.");
            enabled = false;
            return;
        }
        instance = this;

        if (hudText == null)
        {
            Debug.LogWarning("DemoHUD: drag a TextMeshPro text into the Hud Text field.");
        }
    }

    void OnEnable()
    {
        if (showMemoryStats)
        {
            allocRecorder = Recorder.Get("GC.Alloc");
            allocRecorder.enabled = true;
            secondStartTime = Time.unscaledTime;
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;   // let go of the root
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey) && hudText != null)
        {
            hudText.enabled = !hudText.enabled;
        }

        if (showMemoryStats)
        {
            CountAllocations();
        }

        // Unscaled time, so the HUD keeps drawing while the game is paused.
        if (Time.unscaledTime >= nextRedrawTime && (needsRedraw || showMemoryStats))
        {
            Redraw();
            needsRedraw = false;
            nextRedrawTime = Time.unscaledTime + redrawInterval;
        }
    }

    // ---------- Internals ----------

    private void CountAllocations()
    {
        if (allocRecorder != null && allocRecorder.isValid)
        {
            // The recorder reports the frame that just finished.
            allocationsThisSecond += allocRecorder.sampleBlockCount;
        }

        if (Time.unscaledTime - secondStartTime >= 1f)
        {
            allocationsLastSecond = allocationsThisSecond;
            allocationsThisSecond = 0;
            secondStartTime = Time.unscaledTime;
        }
    }

    private void Redraw()
    {
        if (hudText == null) { return; }

        builder.Clear();

        if (title.Length > 0)
        {
            builder.Append("<b>").Append(title).Append("</b>\n");
        }

        foreach (string label in rowLabels)
        {
            builder.Append(label).Append(": ").Append(rowValues[label]).Append('\n');
        }

        if (showMemoryStats)
        {
            builder.Append("\n<b>Memory</b>\n");
            builder.Append("Allocations / sec: ").Append(allocationsLastSecond).Append('\n');
            builder.Append("Managed heap (MB): ")
                   .Append((Profiler.GetMonoUsedSizeLong() / (1024f * 1024f)).ToString("0.0"))
                   .Append('\n');
            builder.Append("GC collections: ").Append(System.GC.CollectionCount(0)).Append('\n');
        }

        if (logLines.Count > 0)
        {
            builder.Append("\n<b>Log</b>\n");
            foreach (string line in logLines)
            {
                builder.Append(line).Append('\n');
            }
        }

        hudText.SetText(builder);
    }
}
