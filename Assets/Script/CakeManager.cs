using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class RobotController : MonoBehaviour
{
    public Image[] cakes;
    public Transform[] caiBanhs;
    private float[] cakeProgress = new float[3];
    private object _lock = new object();
    public Animator animator;
    private Thread[] eatingThreads = new Thread[3];
    private bool isEating = false;
    private bool isPaused = false;
    private bool flagThreadCompleted = false;
    public bool IsEating => isEating;
    public float[] CakeProgress => cakeProgress;
    private int soBanhDaAn, soLuongBanh;
    private float totalProcess, dungTichBanh;

    void Start()
    {
        soBanhDaAn = 0;
        soLuongBanh = 3;
        dungTichBanh = 1000f;
        totalProcess = 0f;
        foreach (var c in cakes)
            c.fillAmount = 0;
    }

    void Update()
    {
        if (isEating)
        {
            for (int i = 0; i < soLuongBanh; i++)
            {
                cakes[i].fillAmount = cakeProgress[i];
                caiBanhs[i].localScale = Vector3.one * (1f - cakeProgress[i]);
            }
            ComputerCMD.Instance.PrintProcess("Total process : " + (totalProcess * 100f).ToString("F2") + " %");
        }
        if (flagThreadCompleted)
        {
            flagThreadCompleted = false;
            ComputerCMD.Instance.PrintProcess("Total process : " + (totalProcess * 100f).ToString("F2") + " %");
            EndEat();
        }
    }
    public void PauseEat()
    {
        if (!isEating) return;
        isPaused = true;
    }
    public void ResumeEat()
    {
        if (!isEating) return;
        isPaused = false;
    }
    public void EatSequentially()
    {
        if (isEating) return;
        StartCoroutine(EatSequentiallyRoutine());
    }

    IEnumerator EatSequentiallyRoutine()
    {
        StartEat();
        for (int i = 0; i < soLuongBanh; i++)
        {
            for (int j = 1; j <= (int)dungTichBanh; j++)
            {
                while (isPaused) yield return null;
                cakeProgress[i] = j / dungTichBanh;
                totalProcess += 1f / dungTichBanh / (float)soLuongBanh;
                yield return new WaitForSeconds(0.001f);
            }
            soBanhDaAn++;
        }
        isEating = false;
        EndEat();
    }
    void EndEat()
    {
        for (int i = 0; i < soLuongBanh; i++)
        {
            cakes[i].fillAmount = 0;
            cakeProgress[i] = 0;
            caiBanhs[i].localScale = Vector3.one;
        }
        if (totalProcess == 100f) Debug.Log("cc");
        animator.CrossFade("IdleSit", 0.15f);
        ComputerCMD.Instance.PrintLine("");
        ComputerCMD.Instance.PrintLine($"<color=green>>Eating done !...Eated {soBanhDaAn} cakes</color>");
        soBanhDaAn = 0;
    }
    void StartEat()
    {
        totalProcess = 0f;
        isEating = true;
        animator.CrossFade("Eating", 0.15f);
        for (int i = 0; i < soLuongBanh; i++)
        {
            caiBanhs[i].localScale = Vector3.one;
        }
    }

    public void EatInParallel()
    {
        if (isEating) return;

        StartEat();

        for (int i = 0; i < soLuongBanh; i++)
        {
            int id = i;
            eatingThreads[i] = new Thread(() => EatCakeThread(id));
            eatingThreads[i].Start();
        }
        new Thread(() =>
        {
            foreach (var t in eatingThreads)
                t.Join(); 

            isEating = false;
            flagThreadCompleted = true;
        }).Start();
    }

    private void EatCakeThread(int index)
    {
        float process = 0f;
        for (int i = 1; i <= (int)dungTichBanh; i++)
        {
            while (isPaused)
            {
                Thread.Sleep(100);
            }

            //cakeProgress[index] = i / dungTichBanh;
            //process = totalProcess + 1f / dungTichBanh / (float)soLuongBanh;
            //totalProcess = process;

            lock (_lock)
            {
                cakeProgress[index] = i / dungTichBanh;
                process = totalProcess + 1f / dungTichBanh / (float)soLuongBanh;
                totalProcess = process;
            }

            Thread.Sleep(3);
        }
        soBanhDaAn++;
    }
}
