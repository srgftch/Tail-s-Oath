using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chect : InteractiveObject
{
    [SerializeField]
    private GameObject ActionUI; // Надпись возможности действия
    [SerializeField]
    private string chestName;
    public override void action()
    {
        ActionUI.SetActive(true);
    }
}
