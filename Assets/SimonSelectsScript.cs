using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using System;

public class SimonSelectsScript : MonoBehaviour
{

    public KMAudio audio;
    public KMBombInfo bomb;

    public KMSelectable[] buttons;
    private int[] stg1order;
    private int[] stg2order;
    private int[] stg3order;

    public Renderer[] buttonrend;
    public Material[] buttonmats;
    private int[] buttonvals = new int[8];

    private int total;
    private int answer;

    public Light[] lights;
    public Color[] lightcols;

    private bool flashing = false;

    private Material customMat;
    private Color answerCol;
    private byte r;
    private byte g;
    private byte b;

    private bool firstpress = false;
    private int inputs;

    private int stage;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    private Coroutine flash;
    private bool isrunning = false;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        moduleSolved = false;
        foreach (KMSelectable obj in buttons)
        {
            KMSelectable pressed = obj;
            pressed.OnInteract += delegate () { PressButton(pressed); return false; };
        }
        customMat = new Material(Shader.Find("KT/Mobile/DiffuseTint"));
    }

    void Start()
    {
        stage = 1;
        total = 0;
        answer = 0;
        inputs = 0;
        customMat.color = new Color(0f, 0f, 0f);
        generateButtons();
        generateOrders();
        generateAnswer();
        flash = StartCoroutine(flashSequence());
    }

    void PressButton(KMSelectable pressed)
    {
        if (moduleSolved != true)
        {
            pressed.AddInteractionPunch(0.25f);
            if (firstpress == false)
            {
                firstpress = true;
            }
            if (isrunning == true)
            {
                StopCoroutine(flash);
                clearLights();
                isrunning = false;
            }
            if (pressed == buttons[0])
            {
                if (lights[0].enabled == false)
                {
                    audio.PlaySoundAtTransform("beep1", transform);
                    inputs++;
                    total += buttonvals[0];
                    lights[0].enabled = true;
                }
                else
                {
                    lights[0].enabled = false;
                    inputs--;
                    total -= buttonvals[0];
                }
            }
            else if (pressed == buttons[1])
            {
                if (lights[1].enabled == false)
                {
                    audio.PlaySoundAtTransform("beep2", transform);
                    inputs++;
                    total += buttonvals[1];
                    lights[1].enabled = true;
                }
                else
                {
                    lights[1].enabled = false;
                    inputs--;
                    total -= buttonvals[1];
                }
            }
            else if (pressed == buttons[2])
            {
                if (lights[2].enabled == false)
                {
                    audio.PlaySoundAtTransform("beep3", transform);
                    inputs++;
                    total += buttonvals[2];
                    lights[2].enabled = true;
                }
                else
                {
                    lights[2].enabled = false;
                    inputs--;
                    total -= buttonvals[2];
                }
            }
            else if (pressed == buttons[3])
            {
                if (lights[3].enabled == false)
                {
                    audio.PlaySoundAtTransform("beep4", transform);
                    inputs++;
                    total += buttonvals[3];
                    lights[3].enabled = true;
                }
                else
                {
                    lights[3].enabled = false;
                    inputs--;
                    total -= buttonvals[3];
                }
            }
            else if (pressed == buttons[4])
            {
                if (lights[4].enabled == false)
                {
                    audio.PlaySoundAtTransform("beep5", transform);
                    inputs++;
                    total += buttonvals[4];
                    lights[4].enabled = true;
                }
                else
                {
                    lights[4].enabled = false;
                    inputs--;
                    total -= buttonvals[4];
                }
            }
            else if (pressed == buttons[5])
            {
                if (lights[5].enabled == false)
                {
                    audio.PlaySoundAtTransform("beep6", transform);
                    inputs++;
                    total += buttonvals[5];
                    lights[5].enabled = true;
                }
                else
                {
                    lights[5].enabled = false;
                    inputs--;
                    total -= buttonvals[5];
                }
            }
            else if (pressed == buttons[6])
            {
                if (lights[6].enabled == false)
                {
                    audio.PlaySoundAtTransform("beep7", transform);
                    inputs++;
                    total += buttonvals[6];
                    lights[6].enabled = true;
                }
                else
                {
                    lights[6].enabled = false;
                    inputs--;
                    total -= buttonvals[6];
                }
            }
            else if (pressed == buttons[7])
            {
                if (lights[7].enabled == false)
                {
                    audio.PlaySoundAtTransform("beep8", transform);
                    inputs++;
                    total += buttonvals[7];
                    lights[7].enabled = true;
                }
                else
                {
                    lights[7].enabled = false;
                    inputs--;
                    total -= buttonvals[7];
                }
            }
            else if (pressed == buttons[8])
            {
                if(flashing == false)
                {
                    if (answer == total)
                    {
                        if (stage == 3)
                        {
                            Debug.LogFormat("[Simon Selects #{0}] Inputted Number Total is: {1}, which is correct! Module Disarmed!", moduleId, total);
                            clearLights();
                            audio.HandlePlaySoundAtTransform("victory", transform);
                            StartCoroutine(victory());
                        }
                        else
                        {
                            Debug.LogFormat("[Simon Selects #{0}] Inputted Number Total is: {1}, which is correct!", moduleId, total);
                            clearLights();
                            audio.HandlePlaySoundAtTransform("correctans", transform);
                            StartCoroutine(correctFlash());
                        }
                    }
                    else
                    {
                        Debug.LogFormat("[Simon Selects #{0}] Inputted Number Total is: {1}, which is incorrect!", moduleId, total);
                        GetComponent<KMBombModule>().HandleStrike();
                        clearLights();
                        StartCoroutine(wrongFlash());
                    }
                }
            }
            if (isrunning == false && inputs == 0)
            {
                flash = StartCoroutine(flashSequence());
            }
        }
    }

    private void clearLights()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = false;
        }
    }

    private void generateAnswer()
    {
        if (stage == 1)
        {
            for (int i = 0; i < stg1order.Length; i++)
            {
                answer += buttonvals[stg1order[i]];
            }
            answer %= 256;
            answer = 256 - answer;
            r = (byte)answer;
            Debug.LogFormat("[Simon Selects #{0}] Answer Number for Stage 1 is: {1}", moduleId, answer);
        }
        else if (stage == 2)
        {
            for (int i = 0; i < stg2order.Length; i++)
            {
                answer += buttonvals[stg2order[i]];
            }
            answer %= 256;
            answer = 256 - answer;
            g = (byte)answer;
            Debug.LogFormat("[Simon Selects #{0}] Answer Number for Stage 2 is: {1}", moduleId, answer);
        }
        else if (stage == 3)
        {
            for (int i = 0; i < stg3order.Length; i++)
            {
                answer += buttonvals[stg3order[i]];
            }
            answer %= 256;
            answer = 256 - answer;
            b = (byte)answer;
            Debug.LogFormat("[Simon Selects #{0}] Answer Number for Stage 3 is: {1}", moduleId, answer);
        }
    }

    private void generateOrders()
    {
        Debug.LogFormat("[Simon Selects #{0}] -----------------Sequences-----------------", moduleId);
        string build1 = "[Simon Selects #{0}] Stage 1's Color Flashes are: ";
        string build2 = "[Simon Selects #{0}] Stage 2's Color Flashes are: ";
        string build3 = "[Simon Selects #{0}] Stage 3's Color Flashes are: ";
        int rand = UnityEngine.Random.Range(3, 6);
        int rand2 = UnityEngine.Random.Range(3, 6);
        int rand3 = UnityEngine.Random.Range(3, 6);
        stg1order = new int[rand];
        stg2order = new int[rand2];
        stg3order = new int[rand3];
        for (int i = 0; i < rand; i++)
        {
            int rnd = UnityEngine.Random.Range(0, 8);
            stg1order[i] = rnd;
            string temp = buttonrend[rnd].material.name;
            temp = temp.Remove(temp.IndexOf(' '), 11);
            if (i == rand - 1)
            {
                build1 += temp;
            }
            else
            {
                build1 += temp + ", ";
            }
        }
        for (int i = 0; i < rand2; i++)
        {
            int rnd = UnityEngine.Random.Range(0, 8);
            stg2order[i] = rnd;
            string temp = buttonrend[rnd].material.name;
            temp = temp.Remove(temp.IndexOf(' '), 11);
            if (i == rand2 - 1)
            {
                build2 += temp;
            }
            else
            {
                build2 += temp + ", ";
            }
        }
        for (int i = 0; i < rand3; i++)
        {
            int rnd = UnityEngine.Random.Range(0, 8);
            stg3order[i] = rnd;
            string temp = buttonrend[rnd].material.name;
            temp = temp.Remove(temp.IndexOf(' '), 11);
            if (i == rand3 - 1)
            {
                build3 += temp;
            }
            else
            {
                build3 += temp + ", ";
            }
        }
        Debug.LogFormat(build1, moduleId);
        Debug.LogFormat(build2, moduleId);
        Debug.LogFormat(build3, moduleId);
        Debug.LogFormat("[Simon Selects #{0}] -----------------------------------------------", moduleId);
    }

    private void generateButtons()
    {
        Debug.LogFormat("[Simon Selects #{0}] -----------------Buttons-----------------", moduleId);
        int rand = UnityEngine.Random.Range(0, 6);
        for (int i = 0; i < rand; i++)
        {
            buttonmats.Shuffle();
        }
        for (int i = 0; i < 8; i++)
        {
            buttonrend[i].material = buttonmats[i];
            if (buttonmats[i].name.EqualsIgnoreCase("Red"))
            {
                lights[i].color = lightcols[0];
                buttonvals[i] = 1;
            }
            else if (buttonmats[i].name.EqualsIgnoreCase("Orange"))
            {
                lights[i].color = lightcols[1];
                buttonvals[i] = 2;
            }
            else if (buttonmats[i].name.EqualsIgnoreCase("Yellow"))
            {
                lights[i].color = lightcols[2];
                buttonvals[i] = 4;
            }
            else if (buttonmats[i].name.EqualsIgnoreCase("Green"))
            {
                lights[i].color = lightcols[3];
                buttonvals[i] = 8;
            }
            else if (buttonmats[i].name.EqualsIgnoreCase("Blue"))
            {
                lights[i].color = lightcols[4];
                buttonvals[i] = 32;
            }
            else if (buttonmats[i].name.EqualsIgnoreCase("Purple"))
            {
                lights[i].color = lightcols[5];
                buttonvals[i] = 64;
            }
            else if (buttonmats[i].name.EqualsIgnoreCase("Magenta"))
            {
                lights[i].color = lightcols[6];
                buttonvals[i] = 128;
            }
            else if (buttonmats[i].name.EqualsIgnoreCase("Cyan"))
            {
                lights[i].color = lightcols[7];
                buttonvals[i] = 16;
            }
            if (i == 0)
            {
                Debug.LogFormat("[Simon Selects #{0}] The Outer Top Left Button is {1}", moduleId, buttonmats[i].name);
            }
            else if (i == 1)
            {
                Debug.LogFormat("[Simon Selects #{0}] The Outer Top Right Button is {1}", moduleId, buttonmats[i].name);
            }
            else if (i == 2)
            {
                Debug.LogFormat("[Simon Selects #{0}] The Outer Bottom Right Button is {1}", moduleId, buttonmats[i].name);
            }
            else if (i == 3)
            {
                Debug.LogFormat("[Simon Selects #{0}] The Outer Bottom Left Button is {1}", moduleId, buttonmats[i].name);
            }
            else if (i == 4)
            {
                Debug.LogFormat("[Simon Selects #{0}] The Inner Top Left Button is {1}", moduleId, buttonmats[i].name);
            }
            else if (i == 5)
            {
                Debug.LogFormat("[Simon Selects #{0}] The Inner Top Right Button is {1}", moduleId, buttonmats[i].name);
            }
            else if (i == 6)
            {
                Debug.LogFormat("[Simon Selects #{0}] The Inner Bottom Right Button is {1}", moduleId, buttonmats[i].name);
            }
            else if (i == 7)
            {
                Debug.LogFormat("[Simon Selects #{0}] The Inner Bottom Left Button is {1}", moduleId, buttonmats[i].name);
            }
        }
    }

    private IEnumerator wrongFlash()
    {
        flashing = true;
        buttons[8].GetComponentInChildren<TextMesh>().text = ":(";
        for(int i = 0; i < 8; i++)
        {
            buttons[i].OnInteract();
        }
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < 8; i++)
        {
            buttons[i].OnInteract();
        }
        inputs = 0;
        total = 0;
        buttons[8].GetComponentInChildren<TextMesh>().text = ":)";
        flash = StartCoroutine(flashSequence());
        flashing = false;
        StopCoroutine("wrongFlash");
    }

    private IEnumerator victory()
    {
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = true;
        }
        yield return new WaitForSeconds(1.5f);
        answerCol = new Color32(r, g, b, 255);
        customMat.color = answerCol;
        buttons[8].GetComponent<Renderer>().material = customMat;
        moduleSolved = true;
        GetComponent<KMBombModule>().HandlePass();
    }

    private IEnumerator correctFlash()
    {
        flashing = true;
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = true;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = true;
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].enabled = false;
            yield return new WaitForSeconds(0.1f);
        }
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = true;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = false;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = true;
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < 8; i++)
        {
            lights[i].enabled = false;
        }
        inputs = 0;
        total = 0;
        stage++;
        generateAnswer();
        flash = StartCoroutine(flashSequence());
        flashing = false;
        StopCoroutine("correctFlash");
    }

    private IEnumerator flashSequence()
    {
        isrunning = true;
        while (moduleSolved == false)
        {
            yield return new WaitForSeconds(2.0f);
            if (stage == 1)
            {
                for (int i = 0; i < stg1order.Length; i++)
                {
                    if (firstpress == true)
                    {
                        if (stg1order[i] == 0)
                        {
                            audio.PlaySoundAtTransform("beep1", transform);
                        }
                        else if (stg1order[i] == 1)
                        {
                            audio.PlaySoundAtTransform("beep2", transform);
                        }
                        else if (stg1order[i] == 2)
                        {
                            audio.PlaySoundAtTransform("beep3", transform);
                        }
                        else if (stg1order[i] == 3)
                        {
                            audio.PlaySoundAtTransform("beep4", transform);
                        }
                        else if (stg1order[i] == 4)
                        {
                            audio.PlaySoundAtTransform("beep5", transform);
                        }
                        else if (stg1order[i] == 5)
                        {
                            audio.PlaySoundAtTransform("beep6", transform);
                        }
                        else if (stg1order[i] == 6)
                        {
                            audio.PlaySoundAtTransform("beep7", transform);
                        }
                        else if (stg1order[i] == 7)
                        {
                            audio.PlaySoundAtTransform("beep8", transform);
                        }
                    }
                    yield return new WaitForSeconds(0.09f);
                    buttons[stg1order[i]].GetComponentInChildren<Light>().enabled = true;
                    yield return new WaitForSeconds(0.3f);
                    buttons[stg1order[i]].GetComponentInChildren<Light>().enabled = false;
                    yield return new WaitForSeconds(0.3f);
                }
            }
            else if (stage == 2)
            {
                for (int i = 0; i < stg1order.Length; i++)
                {
                    if (firstpress == true)
                    {
                        if (stg1order[i] == 0)
                        {
                            audio.PlaySoundAtTransform("beep1", transform);
                        }
                        else if (stg1order[i] == 1)
                        {
                            audio.PlaySoundAtTransform("beep2", transform);
                        }
                        else if (stg1order[i] == 2)
                        {
                            audio.PlaySoundAtTransform("beep3", transform);
                        }
                        else if (stg1order[i] == 3)
                        {
                            audio.PlaySoundAtTransform("beep4", transform);
                        }
                        else if (stg1order[i] == 4)
                        {
                            audio.PlaySoundAtTransform("beep5", transform);
                        }
                        else if (stg1order[i] == 5)
                        {
                            audio.PlaySoundAtTransform("beep6", transform);
                        }
                        else if (stg1order[i] == 6)
                        {
                            audio.PlaySoundAtTransform("beep7", transform);
                        }
                        else if (stg1order[i] == 7)
                        {
                            audio.PlaySoundAtTransform("beep8", transform);
                        }
                    }
                    yield return new WaitForSeconds(0.09f);
                    buttons[stg1order[i]].GetComponentInChildren<Light>().enabled = true;
                    yield return new WaitForSeconds(0.3f);
                    buttons[stg1order[i]].GetComponentInChildren<Light>().enabled = false;
                    yield return new WaitForSeconds(0.3f);
                }
                yield return new WaitForSeconds(0.6f);
                for (int i = 0; i < stg2order.Length; i++)
                {
                    if (firstpress == true)
                    {
                        if (stg2order[i] == 0)
                        {
                            audio.PlaySoundAtTransform("beep1", transform);
                        }
                        else if (stg2order[i] == 1)
                        {
                            audio.PlaySoundAtTransform("beep2", transform);
                        }
                        else if (stg2order[i] == 2)
                        {
                            audio.PlaySoundAtTransform("beep3", transform);
                        }
                        else if (stg2order[i] == 3)
                        {
                            audio.PlaySoundAtTransform("beep4", transform);
                        }
                        else if (stg2order[i] == 4)
                        {
                            audio.PlaySoundAtTransform("beep5", transform);
                        }
                        else if (stg2order[i] == 5)
                        {
                            audio.PlaySoundAtTransform("beep6", transform);
                        }
                        else if (stg2order[i] == 6)
                        {
                            audio.PlaySoundAtTransform("beep7", transform);
                        }
                        else if (stg2order[i] == 7)
                        {
                            audio.PlaySoundAtTransform("beep8", transform);
                        }
                    }
                    yield return new WaitForSeconds(0.09f);
                    buttons[stg2order[i]].GetComponentInChildren<Light>().enabled = true;
                    yield return new WaitForSeconds(0.3f);
                    buttons[stg2order[i]].GetComponentInChildren<Light>().enabled = false;
                    yield return new WaitForSeconds(0.3f);
                }
            }
            else if (stage == 3)
            {
                for (int i = 0; i < stg1order.Length; i++)
                {
                    if (firstpress == true)
                    {
                        if (stg1order[i] == 0)
                        {
                            audio.PlaySoundAtTransform("beep1", transform);
                        }
                        else if (stg1order[i] == 1)
                        {
                            audio.PlaySoundAtTransform("beep2", transform);
                        }
                        else if (stg1order[i] == 2)
                        {
                            audio.PlaySoundAtTransform("beep3", transform);
                        }
                        else if (stg1order[i] == 3)
                        {
                            audio.PlaySoundAtTransform("beep4", transform);
                        }
                        else if (stg1order[i] == 4)
                        {
                            audio.PlaySoundAtTransform("beep5", transform);
                        }
                        else if (stg1order[i] == 5)
                        {
                            audio.PlaySoundAtTransform("beep6", transform);
                        }
                        else if (stg1order[i] == 6)
                        {
                            audio.PlaySoundAtTransform("beep7", transform);
                        }
                        else if (stg1order[i] == 7)
                        {
                            audio.PlaySoundAtTransform("beep8", transform);
                        }
                    }
                    yield return new WaitForSeconds(0.09f);
                    buttons[stg1order[i]].GetComponentInChildren<Light>().enabled = true;
                    yield return new WaitForSeconds(0.3f);
                    buttons[stg1order[i]].GetComponentInChildren<Light>().enabled = false;
                    yield return new WaitForSeconds(0.3f);
                }
                yield return new WaitForSeconds(0.6f);
                for (int i = 0; i < stg2order.Length; i++)
                {
                    if (firstpress == true)
                    {
                        if (stg2order[i] == 0)
                        {
                            audio.PlaySoundAtTransform("beep1", transform);
                        }
                        else if (stg2order[i] == 1)
                        {
                            audio.PlaySoundAtTransform("beep2", transform);
                        }
                        else if (stg2order[i] == 2)
                        {
                            audio.PlaySoundAtTransform("beep3", transform);
                        }
                        else if (stg2order[i] == 3)
                        {
                            audio.PlaySoundAtTransform("beep4", transform);
                        }
                        else if (stg2order[i] == 4)
                        {
                            audio.PlaySoundAtTransform("beep5", transform);
                        }
                        else if (stg2order[i] == 5)
                        {
                            audio.PlaySoundAtTransform("beep6", transform);
                        }
                        else if (stg2order[i] == 6)
                        {
                            audio.PlaySoundAtTransform("beep7", transform);
                        }
                        else if (stg2order[i] == 7)
                        {
                            audio.PlaySoundAtTransform("beep8", transform);
                        }
                    }
                    yield return new WaitForSeconds(0.09f);
                    buttons[stg2order[i]].GetComponentInChildren<Light>().enabled = true;
                    yield return new WaitForSeconds(0.3f);
                    buttons[stg2order[i]].GetComponentInChildren<Light>().enabled = false;
                    yield return new WaitForSeconds(0.3f);
                }
                yield return new WaitForSeconds(0.6f);
                for (int i = 0; i < stg3order.Length; i++)
                {
                    if (firstpress == true)
                    {
                        if (stg3order[i] == 0)
                        {
                            audio.PlaySoundAtTransform("beep1", transform);
                        }
                        else if (stg3order[i] == 1)
                        {
                            audio.PlaySoundAtTransform("beep2", transform);
                        }
                        else if (stg3order[i] == 2)
                        {
                            audio.PlaySoundAtTransform("beep3", transform);
                        }
                        else if (stg3order[i] == 3)
                        {
                            audio.PlaySoundAtTransform("beep4", transform);
                        }
                        else if (stg3order[i] == 4)
                        {
                            audio.PlaySoundAtTransform("beep5", transform);
                        }
                        else if (stg3order[i] == 5)
                        {
                            audio.PlaySoundAtTransform("beep6", transform);
                        }
                        else if (stg3order[i] == 6)
                        {
                            audio.PlaySoundAtTransform("beep7", transform);
                        }
                        else if (stg3order[i] == 7)
                        {
                            audio.PlaySoundAtTransform("beep8", transform);
                        }
                    }
                    yield return new WaitForSeconds(0.09f);
                    buttons[stg3order[i]].GetComponentInChildren<Light>().enabled = true;
                    yield return new WaitForSeconds(0.3f);
                    buttons[stg3order[i]].GetComponentInChildren<Light>().enabled = false;
                    yield return new WaitForSeconds(0.3f);
                }
            }
        }
    }

    //twitch plays
    private bool commandIsValid(string s)
    {
        char[] valids = { 'r','o','y','g','b','c','p','m' };
        s = s.ToLower();
        foreach(char c in s)
        {
            if (!valids.Contains(c))
            {
                return false;
            }
        }
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} select rgm [Selects/Deselects the specified colors where r=red,o=orange,y=yellow,g=green,b=blue,c=cyan,p=purple,m=magenta] | !{0} submit [Submits the selected colors]";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            buttons[8].OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*select\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if(parameters.Length == 2)
            {
                if (commandIsValid(parameters[1]))
                {
                    yield return null;
                    for(int i = 0; i < parameters[1].Length; i++)
                    {
                        if (parameters[1].ElementAt(i).Equals('r'))
                        {
                            int index = 0;
                            for(int j = 0; j < buttonvals.Length; j++)
                            {
                                if(buttonvals[j] == 1)
                                {
                                    index = j;
                                }
                            }
                            buttons[index].OnInteract();
                        }
                        else if (parameters[1].ElementAt(i).Equals('o'))
                        {
                            int index = 0;
                            for (int j = 0; j < buttonvals.Length; j++)
                            {
                                if (buttonvals[j] == 2)
                                {
                                    index = j;
                                }
                            }
                            buttons[index].OnInteract();
                        }
                        else if (parameters[1].ElementAt(i).Equals('y'))
                        {
                            int index = 0;
                            for (int j = 0; j < buttonvals.Length; j++)
                            {
                                if (buttonvals[j] == 4)
                                {
                                    index = j;
                                }
                            }
                            buttons[index].OnInteract();
                        }
                        else if (parameters[1].ElementAt(i).Equals('g'))
                        {
                            int index = 0;
                            for (int j = 0; j < buttonvals.Length; j++)
                            {
                                if (buttonvals[j] == 8)
                                {
                                    index = j;
                                }
                            }
                            buttons[index].OnInteract();
                        }
                        else if (parameters[1].ElementAt(i).Equals('c'))
                        {
                            int index = 0;
                            for (int j = 0; j < buttonvals.Length; j++)
                            {
                                if (buttonvals[j] == 16)
                                {
                                    index = j;
                                }
                            }
                            buttons[index].OnInteract();
                        }
                        else if (parameters[1].ElementAt(i).Equals('b'))
                        {
                            int index = 0;
                            for (int j = 0; j < buttonvals.Length; j++)
                            {
                                if (buttonvals[j] == 32)
                                {
                                    index = j;
                                }
                            }
                            buttons[index].OnInteract();
                        }
                        else if (parameters[1].ElementAt(i).Equals('p'))
                        {
                            int index = 0;
                            for (int j = 0; j < buttonvals.Length; j++)
                            {
                                if (buttonvals[j] == 64)
                                {
                                    index = j;
                                }
                            }
                            buttons[index].OnInteract();
                        }
                        else if (parameters[1].ElementAt(i).Equals('m'))
                        {
                            int index = 0;
                            for (int j = 0; j < buttonvals.Length; j++)
                            {
                                if (buttonvals[j] == 128)
                                {
                                    index = j;
                                }
                            }
                            buttons[index].OnInteract();
                        }
                        yield return new WaitForSeconds(0.25f);
                    }
                    yield break;
                }
            }
        }
    }
}

