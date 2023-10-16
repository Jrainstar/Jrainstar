//==============================================================
// Demo Buttons
//==============================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{
    public Button button4;
    public Button button5;
    public Text Button4Text; // Button text
    public Text Button5Text; // Button text
    private bool musicOn; // True or false

    private void Update()
    {
        if (AudioManager.Instance.CoroutineRun)
        {
            button4.interactable = false;
            button5.interactable = false;
        }
        else
        {
            button4.interactable = true;
            button5.interactable = true;
        }
    }

    public void Button1()
    {
        AudioManager.Instance.PlaySound2D("Gunshot");
    }
    public void Button2()
    {
        AudioManager.Instance.PlaySound2D("Boink");
    }
    public void Button3()
    {
        AudioManager.Instance.PlaySound2D("Buzz");
    }
    public void Button4()
    {
        if (!AudioManager.Instance.CoroutineRun)
            ToggleMusic();
    }
    public void Button5()
    {
        if (!AudioManager.Instance.CoroutineRun)
            ToggleMusicFade();
    }
    public void Button6()
    {
        AudioManager.Instance.StopAmbient();
        AudioManager.Instance.PlayAmbient("Factory", 0);
    }
    public void Button7()
    {
        AudioManager.Instance.StopAmbient();
        AudioManager.Instance.PlayAmbient("Forest", 0);
    }
    public void Button8()
    {
        AudioManager.Instance.StopAmbient();
        AudioManager.Instance.PlayAmbient("Spooky", 0);
    }
    public void Button9()
    {
        AudioManager.Instance.StopAmbient();
    }
    public void ToggleMusic()
    {
        if (!musicOn)
        {
            Button4Text.text = "Stop Music";
            Button5Text.text = "Stop Music (Fade)";
            AudioManager.Instance.PlayMusic("Funky", 0);
            musicOn = true;
        }
        else
        {
            Button4Text.text = "Funky Music";
            Button5Text.text = "Funky Music (Fade)";
            AudioManager.Instance.StopMusic();
            musicOn = false;
        }
    }
    public void ToggleMusicFade()
    {
        if (!musicOn)
        {
            Button4Text.text = "Stop Music";
            Button5Text.text = "Stop Music (Fade)";
            StartCoroutine(AudioManager.Instance.PlayMusicFade("Funky", 2f));
            musicOn = true;
        }
        else
        {
            Button4Text.text = "Funky Music";
            Button5Text.text = "Funky Music (Fade)";
            StartCoroutine(AudioManager.Instance.StopMusicFade(2f));
            musicOn = false;
        }
    }
}
