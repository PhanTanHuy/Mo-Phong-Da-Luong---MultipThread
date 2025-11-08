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

    void Start()
    {
        foreach (var c in cakes)
            c.fillAmount = 0;
    }

    void Update()
    {
        if (isEating)
        {
            for (int i = 0; i < 3; i++)
            {
                cakes[i].fillAmount = cakeProgress[i];
                caiBanhs[i].localScale = Vector3.one * (1f - cakeProgress[i]);
            }
        }
        if (flagThreadCompleted)
        {
            flagThreadCompleted = false;
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
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j <= 10; j++)
            {
                while (isPaused) yield return null;
                cakeProgress[i] = j / 10f;
                yield return new WaitForSeconds(0.3f);
            }
        }

        isEating = false;
        EndEat();
    }
    void EndEat()
    {
        for (int i = 0; i < 3; i++)
        {
            cakes[i].fillAmount = 0;
            cakeProgress[i] = 0;
            caiBanhs[i].localScale = Vector3.one;
        }
        animator.CrossFade("IdleSit", 0.15f);
        ComputerCMD.Instance.PrintLine("<color=green>Eating done !...</color>");
    }
    void StartEat()
    {
        isEating = true;
        animator.CrossFade("Eating", 0.15f);
        for (int i = 0; i < 3; i++)
        {
            caiBanhs[i].localScale = Vector3.one;
        }
    }

    public void EatInParallel()
    {
        if (isEating) return;

        StartEat();

        for (int i = 0; i < 3; i++)
        {
            int id = i;
            eatingThreads[i] = new Thread(() => EatCakeThread(id));
            eatingThreads[i].Start();
        }
        // xoa luong giam sat thi main thread bi dung
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
        for (int i = 0; i <= 10; i++)
        {
            while (isPaused)
            {
                Thread.Sleep(100);
            }
            lock (_lock)
            {
                cakeProgress[index] = i / 10f;
            }
            Thread.Sleep(300);
        }
    }
}
