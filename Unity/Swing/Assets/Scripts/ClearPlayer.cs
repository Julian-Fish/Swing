using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClearPlayer : MonoBehaviour
{
    public HeadRotateController headRotateController;
    public string clearText;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(playerAction());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator playerAction()
    {
        yield return new WaitForSeconds(0.5f);
        //headRotateController.SetAim(Vector2.right);
        //PlayerController.Instance.ropeStatus = PlayerController.RopeStatus.FIRE;
        //yield return new WaitForSeconds(2.5f);
        //PlayerController.Instance.transform.DOLocalMoveX(10.0f, 5.0f);
    }
}
