using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LobbyUI : MonoBehaviourPun
{
    [SerializeField]private string sceneName= "Main";
    public List<TextMeshProUGUI> nickNameText;

    public GameObject startBtn;
    [SerializeField] private Image playerImg;
    [SerializeField] private Sprite[] sprites;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient)
        {
            startBtn.SetActive(true);
        }
        foreach(var name in nickNameText)
        {
            name.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        int cnt = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p != null)
            {
                nickNameText[cnt].text = p.NickName;
            }
            else
            {
                nickNameText[cnt].text = "";
            }
            
            if(p.IsLocal == true)
                playerImg.sprite = sprites[cnt];

            cnt++;
        }
    }

    public void LoadMainScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //이걸 해야 모든클라이언트가 씬을 같이 이동함
            
            PhotonNetwork.LoadLevel(sceneName);
        }
    }
}
