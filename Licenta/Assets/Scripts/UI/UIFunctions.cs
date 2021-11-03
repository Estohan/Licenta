using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFunctions : MonoBehaviour
{
    public void ButtonSaveGame() {
        GameManager.instance.SaveGame();
    }

    public void ButtonLoadGame() {
        GameManager.instance.LoadGame();
    }
}
