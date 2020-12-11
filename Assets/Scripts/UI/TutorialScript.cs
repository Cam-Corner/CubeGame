using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{

    [SerializeField]
    private Button prevButton;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Image tutorialImage;

    [SerializeField]
    private Sprite[] tutorialSprites;

    private int tutorialIndex;

    private void Start() 
    {
        tutorialIndex = 0;    
    }

    private void OnEnable() 
    {
        tutorialIndex = 0;
        UpdateImage();
    }

    public void Next()
    {
        ++tutorialIndex;
        if(tutorialIndex >= tutorialSprites.Length)
        {
            Destroy(this.gameObject);
        }
        else
        {
            UpdateImage();
        }
    }

    public void Prev()
    {
        --tutorialIndex;
        UpdateImage();
    }

    private void UpdateImage()
    {
        if(tutorialSprites.Length == 0)
        {
            prevButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            return;
        }
        Sprite sprite  = tutorialSprites[Mathf.Clamp(tutorialIndex, 0, tutorialSprites.Length-1)];
        tutorialImage.sprite = sprite;
        
        nextButton.gameObject.SetActive(true);
        if(tutorialIndex <= 0)
        {
            prevButton.gameObject.SetActive(false);
        }
        else
        {
            prevButton.gameObject.SetActive(true);
        }
    }
}
