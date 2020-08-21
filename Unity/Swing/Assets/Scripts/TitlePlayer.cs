using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitlePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(playerAction());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Extend") == 1)
        {
            StageController.Instance.NextStage();
        }
    }

    IEnumerator playerAction()
    {
        yield return new WaitForSeconds(1.0f);
        PlayerController.Instance.ropeStatus = PlayerController.RopeStatus.FIRE;

        yield return new WaitForSeconds(1.0f);
        PlayerController.Instance.AddForceToJewel(Vector2.right * 250.0f);
    }
}
