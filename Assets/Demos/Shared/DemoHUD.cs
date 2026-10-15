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
// Turn on "Show Memory Stats" in the Inspector for demos about allocation, GC,
// or speed. It shows FPS, the worst frame, and dropped frames, then the memory numbers.
// The allocation numbers come from Unity's profiler, so they only work in the
// Editor or in a Development Build. The HUD itself allocates a little every time it redraws;
// that cost is the same no matter which demo is running, so compare demos
// against each other, not against zero.
//
// Press H to hide or show the HUD. Press M to switch the memory stats on or off.

[DefaultExecutionOrder(-1000)]   // wake up before any demo tries to write to us
public class DemoHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text hudText;
    [SerializeField] private bool showMemoryStats = false;
    [SerializeField] private int maxLogLines = 8;
    [SerializeField] private float redrawInterval = 0.25f;
    [SerializeField] private KeyCode toggleKey = KeyCode.H;
    [SerializeField] private KeyCode statsKey = KeyCode.M;

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

    // The GC belongs to the whole process, not to our game. In the Editor that
    // process is Unity itself, which has been collecting since it opened. We
    // remember the count when the HUD wakes up and report only what happens after.
    private int collectionsAtStart = 0;

    // Frame stats. A frame counts as "dropped" when it takes more than 1.5x the
    // frame budget (1 / targetFrameRate): long enough that the screen had to show
    // the previous image again. The first second after Play is skipped, because
    // loading the scene always makes a few slow frames.
    [SerializeField] private int targetFrameRate = 60;
    private float frameStatsStartTime = 0f;
    private float frameSecondStartTime = 0f;
    private int framesThisSecond = 0;
    private int framesLastSecond = 0;
    private float worstFrameThisSecond = 0f;
    private float worstFrameLastSecond = 0f;
    private int droppedFrames = 0;

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
        collectionsAtStart = System.GC.CollectionCount(0);
        frameStatsStartTime = Time.unscaledTime + 1f;
        frameSecondStartTime = frameStatsStartTime;

        if (hudText == null)
        {
            Debug.LogWarning("DemoHUD: drag a TextMeshPro text into the Hud Text field.");
            return;
        }

        // Start visible, however the text was left in the scene. H only flips
        // the component, so an inactive GameObject would make it look dead.
        hudText.gameObject.SetActive(true);
        hudText.enabled = true;
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

        if (Input.GetKeyDown(statsKey))
        {
            showMemoryStats = !showMemoryStats;
            needsRedraw = true;
        }

        CountFrames();   // always, so dropped frames covers the whole run

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

    private void CountFrames()
    {
        if (Time.unscaledTime < frameStatsStartTime) { return; }

        float frameTime = Time.unscaledDeltaTime;   // seconds this frame took
        framesThisSecond++;

        if (frameTime > worstFrameThisSecond)
        {
            worstFrameThisSecond = frameTime;
        }

        float frameBudget = 1f / targetFrameRate;
        if (frameTime > frameBudget * 1.5f)
        {
            droppedFrames++;
        }

        if (Time.unscaledTime - frameSecondStartTime >= 1f)
        {
            framesLastSecond = framesThisSecond;
            worstFrameLastSecond = worstFrameThisSecond;
            framesThisSecond = 0;
            worstFrameThisSecond = 0f;
            frameSecondStartTime = Time.unscaledTime;
        }
    }

    private void CountAllocations()
    {
        // Start the recorder the first time stats are needed, so stats can be
        // switched on mid-run (Inspector checkbox or the stats key).
        if (allocRecorder == null)
        {
            allocRecorder = Recorder.Get("GC.Alloc");
            allocRecorder.enabled = true;
            allocationsThisSecond = 0;
            secondStartTime = Time.unscaledTime;
            return;
        }

        if (allocRecorder.isValid)
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

        bool isEmpty = title.Length == 0 && rowLabels.Count == 0 && logLines.Count == 0;
        if (isEmpty && !showMemoryStats)
        {
            builder.Append("<alpha=#88>DemoHUD ready  (H hide, M stats)");
        }

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
            builder.Append("\n<b>Frames</b>\n");
            builder.Append("FPS: ").Append(framesLastSecond).Append('\n');
            builder.Append("Worst frame (ms): ")
                   .Append((worstFrameLastSecond * 1000f).ToString("0.0"))
                   .Append('\n');
            builder.Append("Dropped frames (this run): ").Append(droppedFrames)
                   .Append("  <alpha=#88>(> ")
                   .Append((1500f / targetFrameRate).ToString("0.0"))
                   .Append(" ms)<alpha=#FF>\n");

            builder.Append("\n<b>Memory</b>\n");
            builder.Append("Allocations / sec: ").Append(allocationsLastSecond).Append('\n');
            builder.Append("Managed heap (MB): ")
                   .Append((Profiler.GetMonoUsedSizeLong() / (1024f * 1024f)).ToString("0.0"))
                   .Append('\n');
            builder.Append("GC collections (this run): ")
                   .Append(System.GC.CollectionCount(0) - collectionsAtStart)
                   .Append('\n');
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
