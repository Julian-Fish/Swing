using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance;
    static bool isExist;

    public AudioSource bgm;
    public AudioSource collide;
    public AudioSource gameover;
    public AudioSource attach;
    public AudioSource reclaim;
    public AudioSource clear;

    bool gameoverPlayed;
    bool clearPlayed;

    void Awake()
    {
        if (isExist)
        {
            Destroy(this.gameObject);
            return;
        }
        ResetBool();

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        DontDestroyOnLoad(this.gameObject);
        isExist = true;
        bgm.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    PlaySE("collide");
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    PlaySE("gameover");
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    PlaySE("attach");
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    PlaySE("reclaim");
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    PlaySE("clear");
        //}
    }

    public void ResetBool()
    {
        gameoverPlayed = false;
        clearPlayed = false;
    }

    public void PlaySE(string name)
    {
        switch(name)
        {
            case "collide":
                collide.Play();
                break;
            case "gameover":
                if (!gameoverPlayed)
                {
                    gameover.Play();
                    gameoverPlayed = true;
                }
                break;
            case "attach":
                attach.Play();
                break;
            case "reclaim":
                reclaim.Play();
                break;
            case "clear":
                if (!clearPlayed)
                {
                    Debug.Log("play clear");
                    clear.Play();
                    clearPlayed = true;
                }
                break;
        }
    }
}
