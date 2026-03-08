using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField;
    private string text;

    public TextMeshProUGUI ConnectionStatus;
    public TextMeshProUGUI IDtext;
    public Button connetBtn;


    private void Awake()
    {
        text = string.Empty;
        connetBtn.interactable = false;
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.ConnectUsingSettings();

        connetBtn.onClick.AddListener(() => JoinRoom());

    }


    void Update()
    {
        //서버 상태 표기
        ConnectionStatus.text = PhotonNetwork.NetworkClientState.ToString();
        inputField.onValueChanged.AddListener(OnTextChanged);
        if(text.Length > 0)
        {
            connetBtn.interactable = true;
        }
        else
        {
            connetBtn.interactable = false;
        }
    }

    void OnTextChanged(string text1)
    {
        text = text1;
    }

    public override void OnConnectedToMaster()
    {
        //로비 접속
        PhotonNetwork.JoinLobby();
    }


    //로비 접속 완료시
    public override void OnJoinedLobby()
    {

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 랜덤 룸이 없다면 새로 생성
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20; // 최대 인원 설정

        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;



        //룸 생성하는건데 추가 조건이나 설정 가능함 근데 지금은 안해도될듯?
        PhotonNetwork.CreateRoom("test", roomOptions); // 이름없이 생성하면 포톤이 자동으로 이름 지정
    }


    //룸 생성
    public override void OnCreatedRoom()
    {

    }


    //룸 입장완료
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }



    //버튼으로 호출할 함수
    public void JoinRoom()
    {

        string txt = text;
        PhotonNetwork.NickName = txt;
        //player_info.GetComponent<PlayerInfo>().nick_name = txt;

        //방이 하나밖에 없을예정이라 랜덤으로하긴했음
        //직접 선택하려면 이름 입력해야함
        PhotonNetwork.JoinRandomRoom();

    }
}
