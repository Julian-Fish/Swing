using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class StageController : MonoBehaviour
{
    public static StageController Instance;

	Camera cam;
	Vector3 camTargetPos;
	public float camMaxX;
	public float camMinX;
	public float camMaxY;
	public float camMinY;
    public float deadZoneHeight;
    public float playerHeightFix;
    public string nextStageName;
    public SpriteRenderer blackMask;
	public Text StageText;
    public GameObject gameOverUI_GB;
    float fadeTime;
    public bool isTitle;
	public bool isFinalStage;
    string stageName;

	void Awake()
	{
        Instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        if (SoundPlayer.Instance)
        {
            SoundPlayer.Instance.ResetBool();
        }

        cam = Camera.main;
        fadeTime = 1.0f;
        stageName = SceneManager.GetActiveScene().name;
		blackMask.color = Color.black;
		if (!isFinalStage && !isTitle)
		{
			// text fade in -> text and mask fade out
			Sequence seq = DOTween.Sequence();
			seq.Append(StageText.DOFade(1.0f, fadeTime));
			seq.Append(blackMask.DOFade(0.0f, fadeTime).OnComplete(() => PlayerController.Instance.SetControl(!isTitle)));
			seq.Join(StageText.DOFade(0.0f, fadeTime));
		}
		else
		{
			blackMask.DOFade(0.0f, fadeTime).OnComplete(() => PlayerController.Instance.SetControl(!isTitle));
		}

		if (gameOverUI_GB)
        gameOverUI_GB.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        blackMask.transform.position = (Vector2)cam.transform.position;

		camTargetPos.Set(Mathf.Clamp(PlayerController.Instance.transform.position.x, camMinX, camMaxX), 
						Mathf.Clamp(PlayerController.Instance.transform.position.y + playerHeightFix, camMinY, camMaxY),
						-10.0f);

		cam.transform.position = Vector3.Lerp(cam.transform.position , camTargetPos, 0.05f);

        // call gameover when player's height less than dead height
        if (PlayerController.Instance.transform.position.y < deadZoneHeight
            && PlayerController.Instance.ropeStatus == PlayerController.RopeStatus.AIM)
        {
            if (SoundPlayer.Instance)
            {
                SoundPlayer.Instance.PlaySE("gameover");
            }
            GameOver();
        }
	}

    public void NextStage()
    {
        // black mask fade in
        PlayerController.Instance.SetControl(false);
        if (SoundPlayer.Instance)
        {
            SoundPlayer.Instance.PlaySE("clear");
        }
        blackMask.DOFade(1.0f, fadeTime).OnComplete(() => SceneManager.LoadScene(nextStageName));
        
    }

    public void GameOver()
    {
        gameOverUI_GB.SetActive(true);
		PlayerController.Instance.isGameover = true;
    }

    public void Restart()
    {
        gameOverUI_GB.SetActive(false);
    }

    public string GetStageName()
    {
        return stageName;
    }
}
