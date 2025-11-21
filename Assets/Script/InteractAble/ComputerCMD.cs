using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerCMD : InteractAble
{
    public static ComputerCMD Instance;
    [Header("UI")]
    public TMP_Text outputText, processText;
    public TMP_InputField hiddenInput;
    public ScrollRect scrollRect;

    [Header("References")]
    public RobotController robot;

    private bool isTypingCommand = false, isInteract = false;
    private string currentCommand = "";
    private float defaultPosScroll;
    private void Awake()
    {
        Instance = this;    
    }
    void Start()
    {
        defaultPosScroll = scrollRect.verticalNormalizedPosition;
        hiddenInput.gameObject.SetActive(false);
        PrintLine("<color=white>=== ROBOT THREAD CONTROL TERMINAL ===</color>");
        PrintLine("<color=white>Typing and Press Enter to Execute Code...</color>");
        PrintLine("<color=white>Type: 'dir' to see commands list</color>");
    }

    void Update()
    {
        if (!isInteract) return;

        foreach (char c in Input.inputString)
        {
            if (c == '\b')
            {
                if (currentCommand.Length > 0)
                    currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
            }
            else if (c == '\n' || c == '\r')
            {
                ExecuteCommand();
            }
            else
            {
                currentCommand += c;
            }
        }

        UpdateTypingLine();
        AutoScrollScreen();

        if (robot.IsEating)
        {
            string status = "";
            var cakes = robot.CakeProgress;
            for (int i = 0; i < cakes.Length; i++)
                status += $"Dish {i + 1}: {(int)(cakes[i] * 100)}%\n";

            ShowStatus($"<color=#00BFFF>[Robot Eating...]\n{status}</color>");
        }
    }

    void AutoScrollScreen()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.05f)
        {
            float pos = scrollRect.verticalNormalizedPosition;
            pos += scroll * 0.5f;
            pos = Mathf.Clamp01(pos);
            scrollRect.verticalNormalizedPosition = pos;
        }
    }

    public override void Interact(Transform actor)
    {
        isInteract = !isInteract;
        PlayerMovement player = actor.GetComponent<PlayerMovement>();
        player.enabled = !player.enabled;
    }

    void ExecuteCommand()
    {
        string cmd = currentCommand.Trim().ToLower();
        PrintLine("");
        currentCommand = "";

        if (robot.IsEating && cmd != "pause" && cmd != "resume")
        {
            PrintLine("<color=#FF4040>Robot is busy eating. Please wait...</color>");
        }
        else if (cmd == "dir")
        {
            PrintLine("<color=white>Available commands:</color>");
            PrintLine("<color=#00BFFF>  eat parallel           </color><color=white>: eating in three threads</color>");
            PrintLine("<color=#00BFFF>  eat sequentially       </color><color=white>: eating one by one</color>");
            PrintLine("<color=#00BFFF>  cls                    </color><color=white>: clear screen</color>");
            PrintLine("<color=#00BFFF>  pause                  </color><color=white>: pause eating</color>");
            PrintLine("<color=#00BFFF>  resume                 </color><color=white>: resume eating</color>");
            PrintLine("<color=#00BFFF>  dir                    </color><color=white>: list all commands</color>");

        }
        else if (cmd == "cls")
        {
            ClearScreen();
        }
        else if (cmd == "pause")
        {
            robot.PauseEat();
        }
        else if (cmd == "resume")
        {
            robot.ResumeEat();
        }
        else if (cmd == "eat sequentially")
        {
            PrintLine("<color=#00BFFF>> Executing: Eat Sequentially</color>");
            robot.EatSequentially();
        }
        else if (cmd == "eat parallel")
        {
            PrintLine("<color=#00BFFF>> Executing: Eat Parallel (multi-thread)</color>");
            robot.EatInParallel();
        }
        else if (cmd.StartsWith("stop"))
        {
            PrintLine("<color=#00BFFF>> Stopping threads...</color>");
            //robot.StopEating();
        }
        else if (!string.IsNullOrEmpty(cmd))
        {
            PrintLine($"<color=#FF4040>Unknown command: {cmd}</color>");
        }

        isTypingCommand = false;
    }

    void UpdateTypingLine()
    {
        string[] lines = outputText.text.Split('\n');
        if (lines.Length > 0)
        {
            lines[lines.Length - 1] = "<color=white>Robot Controller/Thread > </color><color=#00BFFF>" + currentCommand + "_</color>";
            outputText.text = string.Join("\n", lines);
        }
    }

    public void PrintLine(string text)
    {
        if (currentCommand != "") currentCommand = "";
        outputText.text += text + "\n";
    }

    void ClearScreen()
    {
        outputText.text = string.Empty;
        PrintLine("<color=white>=== ROBOT THREAD CONTROL TERMINAL ===</color>");
        PrintLine("<color=white>Typing and Press Enter to Execute Code...</color>");
        PrintLine("<color=white>Type: 'dir' to see commands list</color>");
        scrollRect.verticalNormalizedPosition = defaultPosScroll;
    }
    public void PrintProcess(string text)
    {
        processText.text = text;
    }

    void ShowStatus(string text)
    {
        string[] lines = outputText.text.Split('\n');
        if (lines.Length > 5)
            lines[lines.Length - 1] = text;
        else
            outputText.text += "\n" + text;
    }
}
