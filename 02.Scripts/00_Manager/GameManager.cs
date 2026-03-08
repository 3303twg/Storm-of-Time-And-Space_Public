using Photon.Pun;
using System.Collections;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Ship Ship { get; set; }
    public GameObject player;
    public StageManager stageManager;
    
    public Collider2D borderCollider;
    public Vector2 shipSpawnPosition = new Vector2(9f, 11f);
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject shipPrefab;
    
    private int playerCount = 0;
    
    private WaitForSeconds ws = new WaitForSeconds(0.1f);
    

    public override void Awake()
    {
        base.Awake();
        if (PhotonNetwork.IsMasterClient)
        {
            Ship = PhotonNetwork.Instantiate(shipPrefab.name, shipSpawnPosition, Quaternion.identity, 0).GetComponent<Ship>();
        }

    }

    private string[] playerPrefabNames =
    {
        "Player02",
        "Player01",
        "Player03",
        "Player04"
    };


    private void Start()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        StartCoroutine(SpawnPlayer());
        SoundManager.Instance.PlaySFX("PlayerJump");
    }
    
    private IEnumerator SpawnPlayer()
    {
        while (Ship == null) // 저쪽도 생성했는지 확인
        {
            yield return null;
        }
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // 인덱스가 배열 범위를 넘지 않도록 제한
        if (index < 0 || index >= playerPrefabNames.Length)
        {
            Debug.LogWarning($"배정할 프리팹 인덱스가 범위를 벗어났습니다: {index}");
            index = 0; // 또는 기본값
        }

        string prefabName = playerPrefabNames[index];

        Vector3 newVector = new Vector3(0f, 1f, 0f);

        PhotonNetwork.Instantiate(prefabName, Ship.transform.position + newVector, Quaternion.identity);
    }


    public void LoadScene()
    {
        photonView.RPC("RPC_ClearNetwork", RpcTarget.MasterClient);
        //굳이 마스터가 안잡아도 될듯?
        
    }

    [PunRPC]
    void RPC_ClearNetwork()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 1. Instantiate 된 오브젝트 제거
        foreach (var view in GameObject.FindObjectsOfType<PhotonView>())
        {
            if (view.TryGetComponent<SoundManager>(out var a)) { }
            else
            {

                if (view.IsMine)
                    PhotonNetwork.Destroy(view.gameObject);
            }
        }

        // 2. RPC 제거
        foreach (var player in PhotonNetwork.PlayerList)
        {
            PhotonNetwork.RemoveRPCs(player);
        }

        PhotonNetwork.LoadLevel("Lobby");
    }
}
