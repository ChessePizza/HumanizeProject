using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    GameUI gameUI;
    Gameplay gameplay;
    CameraController cam;

    [Header("Quest")]
    public Text levelTitle;
    public Text Quest1Text;
    public Text Quest2Text;
    public Text Quest3Text;
    public Image Quest1Icon;
    public Image Quest2Icon;
    public Image Quest3Icon;

    [Header("Dialog")]
    public GameObject dialogUI;
    public Text title;
    public Text message;

    [Header("Sprite")]
    public GameObject kid;
    public GameObject player;
    public Sprite checkSprite;
    public Sprite uncheckSprite;

    [Header("Buttons")]
    public Button next;
    public Button skip;
    public Button choice1;
    public Button choice2;

    [Header("Data")]
    public bool isReady = false;
    public bool inRange = false;
    public GameObject sampleEnemy;
    public Health baseHealth;

    float camPosTime;
    float zoomTime;
    float originZoom;
    float targetZoom;
    float camPosDuration;
    float zoomDuration;
    Vector2 originCamPos;
    Vector2 targetCamPos;

    int questState = 0; // Objectives
    int questProgress = 0; // 0 = start, 1 = inquest, 2 = finish
    int dialogState = 0; // Page
    int state = 0;

    bool isUpdatable = true;
    bool isSelectedChoice2 = true;

    bool[] questResult = new bool[3];

    // Start is called before the first frame update
    void Start()
    {
        inRange = false;
        questResult = new bool[3];
        isUpdatable = true;
        isSelectedChoice2 = true;
        questState = 0;
        questProgress = 0;
        dialogState = 0;
        state = 0;

        camPosTime = 0;
        zoomTime = 0;
        camPosDuration = 3;
        zoomDuration = 3;

        gameplay = Camera.main.GetComponent<Gameplay>();
        gameUI = Camera.main.GetComponent<GameUI>();
        cam = Camera.main.GetComponent<CameraController>();
        isReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Update Camera Lerp
        if (cam.forceMode)
        {
            if (camPosTime < 1 && originCamPos != targetCamPos)
            {
                camPosTime += Time.deltaTime / camPosDuration;
            }
            if (zoomTime < 1 && originZoom != targetZoom)
            {
                zoomTime += Time.deltaTime / zoomDuration;
            }
            cam.forcePosition = Vector2.Lerp(originCamPos, targetCamPos, camPosTime);
            cam.forceZoomValue = Mathf.Lerp(originZoom, targetZoom, zoomTime);
        }

        // Update Quest
        if (isReady && isUpdatable) {
            switch (gameplay.levelId)
            {
                case 0: StartCoroutine(UpdateRoute1()); break;
                case 1: StartCoroutine(UpdateRoute2()); break;
                default: break;
            }
        }
    }

    // เนื่องจากไม่ทันเวลา จึงเขียนแบบ hard code
    IEnumerator UpdateRoute1()
    {
        isUpdatable = false;
        if (questState == 0)
        {
            if (questProgress == 0)
            {
                if (dialogState == 0)
                {
                    skip.gameObject.SetActive(true);
                    ClearQuest();
                    SetCamera(new Vector2(0.24f, -1.37f), 0.75f, 3);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 1)
                {
                    ShowDialog("Now is a good time to gather some resources!", false);
                }
                else if (dialogState == 2)
                {
                    ShowDialog("Look at that conveniently placed screw. How Cliché.", false);
                }
                else if (dialogState == 3)
                {
                    CloseDialog();
                    dialogState++;
                }
                else if (dialogState == 4)
                {
                    SetCamera(new Vector2(-10.37f, -6.22f), 0.2f, 2);
                    yield return new WaitForSeconds(2);
                    dialogState++;
                }
                else if (dialogState == 5)
                {
                    yield return new WaitForSeconds(1);
                    SetCamera(new Vector2(0.24f, -1.37f), 0.75f, 2);
                    yield return new WaitForSeconds(2);
                    dialogState++;
                }
                else if (dialogState == 6)
                {
                    ShowDialog("Now go pick it up!", false);
                    ShowChoice("OKay", "No");
                }
                else if (dialogState == 7)
                {
                    if (isSelectedChoice2)
                    {
                        ShowDialog("But you need it to progress. Go pick it up.", false);
                        ShowChoice("OKay", "No");
                    }
                    else
                    {
                        Skip();
                    }
                }
                else if (dialogState == 8)
                {
                    if (isSelectedChoice2)
                    {
                        ShowDialog("I said PICK IT UP!", false);
                        ShowChoice("OKay", "No");
                    }
                    else
                    {
                        Skip();
                    }
                }
                else if (dialogState == 9)
                {
                    if (isSelectedChoice2)
                    {
                        ShowDialog("PLEASE FOR THE LOVE OF GOD PICK IT UP.", false);
                        ShowSingleChoice("OKay");
                    }
                    else
                    {
                        ShowDialog("I SAI- oh… good.", false);
                    }
                }
                else if (dialogState == 10)
                {
                    skip.gameObject.SetActive(false);
                    ShowDialog("Thank You.", false);
                }
                else if (dialogState == 11)
                {
                    CloseDialog();
                    SetQuest1("Pick A Screw Up", false);
                    yield return new WaitForSeconds(3);
                    SetCamera(cam.player.transform.position, cam.preForceZoomValue, 2);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 12)
                {
                    dialogState = 0;
                    questProgress = 1;
                    cam.forceMode = false;
                    gameUI.ShowAll();
                    gameUI.buildcategoryButton.gameObject.SetActive(false);
                }
            }
            else if (questProgress == 1)
            {
                UpdateRoute1Progress();
            }
            else if (questProgress == 2)
            {
                if (dialogState == 0)
                {
                    skip.gameObject.SetActive(false);
                    gameUI.HideAll();
                    SetCamera(new Vector2(0.24f, -1.37f), 0.75f, 3);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 1)
                {
                    ShowDialog("Well Done! Collectable items will shine.\nKeep your eyes open.", false);
                }
                else if (dialogState == 2)
                {
                    SetQuest1("Pick A Screw Up", true);
                    gameUI.ShowAll();
                    gameUI.buildcategoryButton.gameObject.SetActive(false);
                    dialogState = 0;
                    questProgress = 0;
                    questState = 1;
                    cam.forceMode = false;
                    gameplay.pause = false;
                }
            }
        }
        else if (questState == 1)
        {
            if (questProgress == 0)
            {
                if (dialogState == 0)
                {
                    skip.gameObject.SetActive(true);
                    yield return new WaitForSeconds(12);
                    gameplay.pause = true;
                    gameUI.HideAll();
                    SetCamera(new Vector2(0.24f, -1.37f), 0.75f, 3);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 1)
                {
                    ShowDialog("Oh look! What perfect timing!", false);
                }
                else if (dialogState == 2)
                {
                    ShowDialog("There’s an aggressive robot trying to attack us.", false);
                }
                else if (dialogState == 3)
                {
                    ShowDialog("Quick! Build something to defend ourselves.", false);
                    ShowChoice("Got it.", "No");
                }
                else if (dialogState == 4)
                {
                    if (isSelectedChoice2)
                    {
                        ShowDialog("Seriously?!?!? Not this sh*t again.\nGO BUILD SOMETHING.", false);
                        ShowSingleChoice("Fine.");
                    }
                    else
                    {
                        Next();
                    }
                }
                else if (dialogState == 5)
                {
                    skip.gameObject.SetActive(false);
                    ShowDialog("Thank you", false);
                }
                else if (dialogState == 6)
                {
                    CloseDialog();
                    SetQuest2("Build A Gun", false);
                    yield return new WaitForSeconds(3);
                    SetCamera(cam.player.transform.position, cam.preForceZoomValue, 2);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 7)
                {
                    yield return new WaitForSeconds(1);
                    dialogState = 0;
                    questProgress = 1;
                    cam.forceMode = false;
                    StartGame();
                    gameplay.pause = false;
                    gameUI.ShowAll();
                    gameUI.buildcategoryButton.gameObject.SetActive(true);
                }
            }
            else if (questProgress == 1)
            {
                UpdateRoute1Progress();
            }
            else if (questProgress == 2)
            {
                if (dialogState == 0)
                {
                    skip.gameObject.SetActive(true);
                    gameUI.HideAll();
                    SetCamera(new Vector2(0.24f, -1.37f), 0.75f, 3);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 1)
                {
                    ShowDialog("Good Work! You saved us.\nThere is nothing left to teach you.", false);
                }
                else if (dialogState == 2)
                {
                    skip.gameObject.SetActive(false);
                    Destroy(sampleEnemy);
                    ShowDialog("We should start build and protect our base for this night.", false);
                }
                else if (dialogState == 3)
                {
                    gameplay.killCount++;
                    baseHealth.SetHealth(baseHealth.MaxHealth);
                    SetQuest2("Build A Gun", true);
                    gameUI.ShowAll();
                    dialogState = 0;
                    questProgress = 0;
                    questState = 2;
                    cam.forceMode = false;
                    gameplay.pause = false;
                }
            }
        }
        isUpdatable = true;
        yield return null;
    }

    IEnumerator UpdateRoute2()
    {
        isUpdatable = false;
        if (questState == 0)
        {
            if (questProgress == 0)
            {
                if (dialogState == 0)
                {
                    skip.gameObject.SetActive(true);
                    ClearQuest();
                    SetCamera(new Vector2(0.24f, -1.37f), 0.75f, 3);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 1)
                {
                    ShowDialog("I heard a rumour that someone will drop an important information at night", false);
                }
                else if (dialogState == 2)
                {
                    ShowDialog("If I remembered correctly,they will come around 23:00.", false);
                }
                else if (dialogState == 3)
                {
                    ShowDialog("Would you pick it up for me?", false);
                    ShowChoice("OKay", "No");
                }
                else if (dialogState == 4)
                {
                    if (isSelectedChoice2)
                    {
                        ShowDialog("...", false);
                        ShowSingleChoice("Fine. I will do it.");
                    }
                    else
                    {
                        Next();
                    }
                }
                else if (dialogState == 5)
                {
                    skip.gameObject.SetActive(false);
                    ShowDialog("Thank you.", false);
                }
                else if (dialogState == 6)
                {
                    CloseDialog();
                    SetQuest1("Find A Paper (Hint 23:00)", false);
                    yield return new WaitForSeconds(3);
                    SetCamera(cam.player.transform.position, cam.preForceZoomValue, 2);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 7)
                {
                    dialogState = 0;
                    questProgress = 1;
                    cam.forceMode = false;
                    gameUI.ShowAll();
                    gameUI.buildcategoryButton.gameObject.SetActive(true);
                    StartGame();
                    gameplay.pause = false;
                }
            }
            else if (questProgress == 1)
            {
                UpdateRoute2Progress();
            }
            else if (questProgress == 2)
            {
                if (dialogState == 0)
                {
                    skip.gameObject.SetActive(false);
                    gameUI.HideAll();
                    SetCamera(new Vector2(0.24f, -1.37f), 0.75f, 3);
                    yield return new WaitForSeconds(3);
                    dialogState++;
                }
                else if (dialogState == 1)
                {
                    ShowDialog("You found it! Thank you.\nThank you very much.", false);
                }
                else if (dialogState == 2)
                {
                    SetQuest1("Find A Paper (Hint 23:00)", true);
                    gameUI.ShowAll();
                    gameUI.buildcategoryButton.gameObject.SetActive(true);
                    dialogState = 0;
                    questProgress = 0;
                    questState = 1;
                    cam.forceMode = false;
                    gameplay.pause = false;
                }
            }
        }
        isUpdatable = true;
        yield return null;
    }

    //เงื่อนไขเช็คจากตรงนี้
    void UpdateRoute1Progress()
    {
        if (questState == 0)
        {
            if (inRange && gameplay.findItem(ItemType.screw))
            {
                dialogState = 0;
                questProgress = 2;
            }
        }
        else if (questState == 1)
        {
            if (gameplay.isSuccessfullyBuildOnce)
            {
                dialogState = 0;
                questProgress = 2;
            }
        }
    }

    void UpdateRoute2Progress()
    {
        if (questState == 0)
        {
            if (inRange && gameplay.findItem(ItemType.paper))
            {
                dialogState = 0;
                questProgress = 2;
            }
        }
    }

    public void Skip()
    {
        if (gameplay.levelId == 0)
        {
            if (questState == 0)
            {
                if (questProgress == 0)
                {
                    CloseDialog();
                    dialogState = 10;
                }
            }
            else if (questState == 1)
            {
                if (questProgress == 0)
                {
                    CloseDialog();
                    dialogState = 5;
                }
                else if (questState == 2)
                {
                    CloseDialog();
                    dialogState = 2;
                }
            }
        }
        else if (gameplay.levelId == 1)
        {
            if (questState == 0)
            {
                if (questProgress == 0)
                {
                    CloseDialog();
                    dialogState = 5;
                }
            }
        }
    }

    public void Choice1()
    {
        isSelectedChoice2 = false;
        Next();
    }

    public void Choice2()
    {
        isSelectedChoice2 = true;
        Next();
    }

    public void Next()
    {
        CloseDialog();
        dialogState++;
    }

    void CloseDialog()
    {
        dialogUI.SetActive(false);
    }

    void ShowDialog(string text, bool isPlayer)
    {
        if (!isPlayer)
        {
            title.text = "Kid";
            kid.SetActive(true);
            player.SetActive(false);
        }
        else
        {
            title.text = "Robot";
            kid.SetActive(false);
            player.SetActive(true);
        }
        message.text = text;
        next.gameObject.SetActive(true);
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);
        dialogUI.SetActive(true);
    }
    void ShowChoice(string text1, string text2)
    {
        choice1.transform.Find("Text").GetComponent<Text>().text = text1;
        choice2.transform.Find("Text").GetComponent<Text>().text = text2;

        next.gameObject.SetActive(false);
        choice1.gameObject.SetActive(true);
        choice2.gameObject.SetActive(true);
    }

    void ShowSingleChoice(string text)
    {
        choice2.transform.Find("Text").GetComponent<Text>().text = text;

        next.gameObject.SetActive(false);
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(true);
    }

    // hard code
    public bool CheckQuest()
    {
        if (gameplay.levelId == 0)
        {
            return questResult[0] && questResult[1];
        }
        else if (gameplay.levelId == 1)
        {
            return questResult[0];
        }
        return false;
    }

    void ClearQuest()
    {
        questResult[0] = true;
        questResult[1] = true;
        questResult[2] = true;
        Quest1Icon.sprite = uncheckSprite;
        Quest2Icon.sprite = uncheckSprite;
        Quest3Icon.sprite = uncheckSprite;
        Quest1Text.gameObject.SetActive(false);
        Quest2Text.gameObject.SetActive(false);
        Quest3Text.gameObject.SetActive(false);
    }

    void SetQuest1(string text, bool check)
    {
        if (check)
        {
            Quest1Icon.sprite = checkSprite;
            questResult[0] = true;
        }
        else
        {
            Quest1Icon.sprite = uncheckSprite;
            questResult[0] = false;
        }
        Quest1Text.text = text;
        Quest1Text.gameObject.SetActive(true);
        Quest1Icon.gameObject.SetActive(true);
        if(!check) gameUI.Announce("New Objective", text);
        else gameUI.Announce("Objective Complete!", text);
    }
    void SetQuest2(string text, bool check)
    {
        if (check)
        {
            Quest2Icon.sprite = checkSprite;
            questResult[1] = true;
        }
        else
        {
            Quest2Icon.sprite = uncheckSprite;
            questResult[1] = false;
        }
        Quest2Text.gameObject.SetActive(true);
        Quest2Icon.gameObject.SetActive(true);
        if (!check) gameUI.Announce("New Objective", text);
        else gameUI.Announce("Objective Complete!", text);
    }
    void SetQuest3(string text, bool check)
    {
        if (check)
        {
            Quest3Icon.sprite = checkSprite;
            questResult[2] = true;
        }
        else
        {
            Quest3Icon.sprite = uncheckSprite;
            questResult[2] = false;
        }
        Quest3Text.gameObject.SetActive(true);
        Quest3Icon.gameObject.SetActive(true);
        if (!check) gameUI.Announce("New Objective", text);
        else gameUI.Announce("Objective Complete!", text);
    }

    void SetCamera(Vector2 position, float zoom, float duration)
    {
        camPosTime = 0;
        zoomTime = 0;
        originCamPos = cam.forcePosition;
        originZoom = cam.zoom.value;
        targetZoom = zoom;
        targetCamPos = position;
        cam.forceMode = true;
        camPosDuration = duration;
        zoomDuration = duration;
    }

    public void ResetRoute()
    {
        questState = 0;
        questProgress = 0;
        dialogState = 0;
    }

    void StartGame()
    {
        gameplay.InitState(GameState.Prepare);
        gameUI.SetTimer(gameplay.startTime.x, gameplay.startTime.y);
        gameUI.StartTimer();
    }
}
