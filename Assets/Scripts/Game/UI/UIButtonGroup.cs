using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 * Defines the UI Button group at the main
 */
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
[System.Serializable]
public class UIButtonGroup : MonoBehaviour {

    //Children objects
    public Button button;
    public Text textBox;
    public GameObject parentObject;
    public bool isLeft;
    public bool startHidden;

    RectTransform rectTransform;
    float moveTime = 0.85f;
    float targetDisplacement;
    float startingX;
    bool isHidden;
    bool isRunningLock;
    string textCache;

    void Start() {
        rectTransform = GetComponent<RectTransform>();
        targetDisplacement = rectTransform.rect.width;
        startingX = rectTransform.localPosition.x;

        if (startHidden) {
            isHidden = true;
            if (isLeft) {
                ClampLeft();
            }
            else {
                ClampRight();
            }
        }
    }

    public void SetModel(GameObject newMesh) {
        //Change Weapon UI Info        
        //remove all children of the parent
        foreach (Transform child in parentObject.transform) {
            GameObject.Destroy(child.gameObject);
        }

        //add in new child
        GameObject newWeaponIcon = GameObject.Instantiate(newMesh,
            parentObject.transform.position, new Quaternion(45, 90, 250, 0)) as GameObject;
        //newWeaponIcon.transform.parent = transform;
    }

    public void SetText(string nextText) {
        textBox.text = nextText;
        textCache = nextText;
    }

    public void Hide() {
        if (isHidden) {
            print("UIButtonGroup " + name + " was set to hide, but is already hidden.");
            return;
        }
        isHidden = true;

        if (isLeft) {
            StartCoroutine(MoveLeft());
        }
        else {
            StartCoroutine(MoveRight());
        }

        //hide the text
        StartCoroutine(HideInfoDelay());
    }

    public void Show() {
        if (!isHidden) {
            print("UIButtonGroup " + name + " was set to show, but is already shown.");
            return;
        }
        isHidden = false;

        if (isLeft) {
            StartCoroutine(MoveRight());
        }
        else {
            StartCoroutine(MoveLeft());
        }

        //show the text
        textBox.text = textCache;
        parentObject.SetActive(true);
    }

    public void ChangeObjectDuringGame(GameObject model, string newText) {
        StartCoroutine(ChangeObjectCoroutine(model, newText));
    }

    IEnumerator MoveLeft() {
        //Handle lock
        while (isRunningLock) {
            yield return null;
        }
        isRunningLock = true;

        float displacementAmount = targetDisplacement * Time.deltaTime / moveTime;

        while (rectTransform.localPosition.x > startingX - targetDisplacement) {
            rectTransform.localPosition += Vector3.left * displacementAmount;
            yield return null;
        }

        //finally, lock the position
        ClampLeft();
        isRunningLock = false;
        yield return null;
    }

    IEnumerator MoveRight() {
        //Handle lock
        while (isRunningLock) {
            yield return null;
        }
        isRunningLock = true;

        float displacementAmount = targetDisplacement * Time.deltaTime / moveTime;

        while (rectTransform.localPosition.x < startingX + targetDisplacement) {
            rectTransform.localPosition += Vector3.right * displacementAmount;
            yield return null;
        }

        //finally, lock the position
        ClampRight();
        isRunningLock = false;
        yield return null;
    }

    /*
     * Don't question this implementation
     */
    IEnumerator ChangeObjectCoroutine(GameObject obj, string newText) {
        if (!isHidden) {
            Hide();
            yield return new WaitForSeconds(moveTime);
        }
        SetModel(obj);
        SetText(newText);
        Show();

    }

    IEnumerator HideInfoDelay() {
        yield return new WaitForSeconds(moveTime);
        textBox.text = "";
        parentObject.SetActive(false);
    }
    
    private void ClampLeft() {
        //set new startingX
        startingX -= targetDisplacement;
        rectTransform.localPosition = new Vector3(startingX, rectTransform.localPosition.y);
    }

    private void ClampRight() {
        //set new startingX
        startingX += targetDisplacement;
        rectTransform.localPosition = new Vector3(startingX, rectTransform.localPosition.y);
    }

    public void Deactivate() {
        if (parentObject != null) {
            parentObject.SetActive(false);
        }
    }
}
