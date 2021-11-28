using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    enum PageType { 
        regular = 0,
        special = 1
    }

    public bool isPrologue;
    int pages;
    PageType type;

    [Header("Pages")]
    public Image page1;
    public Image page2;
    public Image page3;
    public Image page1Special;

    public Sprite[] intro;
    public Sprite[] end;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        pages = 0;
        UpdatePages();
        ShowPages();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void UpdatePages()
    {
        if (isPrologue)
        {
            switch (pages)
            {
                case 0:
                    page1.sprite = intro[0];
                    page2.sprite = intro[1];
                    page3.sprite = intro[2];
                    type = PageType.regular;
                    break;
                case 1:
                    page1.sprite = intro[3];
                    page2.sprite = intro[4];
                    page3.sprite = intro[5];
                    type = PageType.regular;
                    break;
                case 2:
                    page1.sprite = intro[6];
                    page2.sprite = intro[7];
                    page3.sprite = intro[8];
                    type = PageType.regular;
                    break;
                default: break;
            };
        }
        else
        {
            switch (pages)
            {
                case 0:
                    page1Special.sprite = end[0];
                    page2.sprite = end[1];
                    type = PageType.special;
                    break;
                case 1:
                    page1.sprite = end[2];
                    page2.sprite = end[3];
                    page3.sprite = end[4];
                    type = PageType.regular;
                    break;
                case 2:
                    page1.sprite = end[5];
                    page2.sprite = end[6];
                    page3.sprite = end[7];
                    type = PageType.regular;
                    break;
                default: break;
            };
        }
    }

    void ShowPages()
    {
        anim.Play("CutSceneEmpty");
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(1);
        if (type == PageType.regular)
        {
            anim.Play("CutSceneNormal");
        }
        else
        {
            anim.Play("CutSceneSpecial");
        }
        yield return null;
    }

    public void NextPage()
    {
        if (pages < 3)
        {
            pages++;
            UpdatePages();
            ShowPages();
        }
        else
        {
            SkipPages();
        }
    }
    public void PreviousPage()
    {
        if (pages > 0)
        {
            pages--;
            UpdatePages();
            ShowPages();
        }
        else if(pages < 0)
        {
            pages = 0;
        }
    }

    public void SkipPages()
    {
        if (isPrologue)
        {
            StartCoroutine(LoadLevel());
        }
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation asyncLoad;
        asyncLoad = SceneManager.LoadSceneAsync("Scenes/Level1", LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
