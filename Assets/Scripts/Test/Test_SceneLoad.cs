using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Test_SceneLoad : TestBase
{
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        // SceneManager.LoadScene(0);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        SceneManager.UnloadSceneAsync("Main");
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        //SceneManager.LoadScene($"Seemless_{targetX}_{targetY}", LoadSceneMode.Additive);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        //SceneManager.UnloadSceneAsync($"Seemless_{targetX}_{targetY}");
    }
}
