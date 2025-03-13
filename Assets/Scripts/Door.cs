using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : InteractiveObject
{
    [SerializeField]
    private float yPosOffset = 1f;
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private bool savePos = false;

    public override void action()
    {
        if (savePos == true) 
        {
            PlayerPrefs.SetInt(PlayerConsts.RESTART_POS_AVALAIB, 1);
            PlayerPrefs.SetFloat(PlayerConsts.X_RESTART_POS, gameObject.transform.position.x);
            PlayerPrefs.SetFloat(PlayerConsts.Y_RESTART_POS, gameObject.transform.position.y - yPosOffset); 
        }
        SceneManager.LoadScene(sceneName);
    }
}
