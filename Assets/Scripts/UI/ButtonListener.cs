using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonListener : MonoBehaviour
{
    public void OnButtonClicked()
    {
        SceneManager.LoadScene("Loading");
    }
}
