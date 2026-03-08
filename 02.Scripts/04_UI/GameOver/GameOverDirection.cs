using Cinemachine;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverDirection : MonoBehaviour
{
    public GameObject gameoverUI;
    public CinemachineVirtualCamera virtualCamera;

    public TextMeshProUGUI titleText;
    public List<TextMeshProUGUI> nickName;

    public Button button;



    private void OnEnable()
    {
        EventBus.Subscribe(EventBusType.GameClear, GameClear);
        EventBus.Subscribe(EventBusType.GameOver, GameOver);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(EventBusType.GameClear, GameClear);
        EventBus.Unsubscribe(EventBusType.GameOver, GameOver);
    }
    void Start()
    {
        int cnt = 0;

        for (int i = 0; i < nickName.Count; i++)
        {
            nickName[cnt].text = string.Empty;
        }
        foreach(var player in PhotonNetwork.PlayerList)
        {
            if (player != null)
            {
                nickName[cnt].text = player.NickName;
            }
            cnt++;
        }

        //GameClear(2f);
        //GameOver(2f);

        button.onClick.AddListener(() => GameManager.Instance.LoadScene());
    }

    public void GameClear(object duration)
    {
        titleText.text = "GameClear";
        Invoke("ZoomIn", (float)duration);
    }

    public void GameOver(object duration)
    {
        titleText.text = "GAMEOVER";
        Invoke("ZoomIn", (float)duration);
    }
    public void ZoomIn()
    {
        gameoverUI.SetActive(true);
        virtualCamera.Priority = 20;
    }

    
    }
