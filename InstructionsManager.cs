using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsManager : MonoBehaviour
{
    // Idle State = any int
    // Compression State = 1
    // Rescure Breath State = 2
    // Kneeling State = 3
    // Full CPR State = 4

    public GameObject Camera;
    // public Animator animator;

    public int state = 0;
    public int page = 0;
    public string text = "abc";

    public Button nextButton;
    public Button prevButton;
    public TMPro.TMP_Text textUI;
    public Image image;

    public string[] pageText = {
        "Page 1",
        "Page 2",
        "Page 3",
        "Page 4",
        "Page 5",
        "Page 6",
    };

   /* public int[] pageState = {
        0,
        0,
        1,
        1,
        2,
        2,
    }; 
   */
    public bool[] pageImageState = {
        false,
        true,
        false,
        false,
        false,
        false,
    };

    // New list to hold the images for each page
    public Sprite[] pageImages;

    // Start is called before the first frame update
    void Start()
    {
        
        nextButton.onClick.AddListener(next);
        prevButton.onClick.AddListener(prev);
        prevButton.enabled = false;
        nextButton.enabled = true;
        setPage();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void setPage()
    {
        // state = pageState[page];
        text = pageText[page];
        image.sprite = pageImages[page]; // Set the image based on the current page
        image.enabled = pageImageState[page];
        textUI.text = text;
        
    }

    void next()
    {
        page++;
        prevButton.enabled = true;
        if (page >= pageText.Length)
        {
            page = pageText.Length - 1;
            nextButton.enabled = false;
        }
        setPage();
    }

    void prev()
    {
        page--;
        nextButton.enabled = true;
        if (page < 0)
        {
            page = 0;
            prevButton.enabled = false;
        }
        setPage();
    }
}
