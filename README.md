## Storm Of Time And Space

### 프로젝트 개요

- **장르**: 2D 멀티플레이 협동 슈팅 / 액션  
- **엔진 / 언어**: Unity, C#  
- **네트워크**: Photon PUN 2  
- **핵심 콘셉트**: 여러 플레이어가 하나의 거대한 우주선(Ship)을 함께 조종하며, 선체 이동, 캐논(포탑), 방어막, 인벤토리/상점 등 역할을 나누어 수행하면서 웨이브로 등장하는 적과 보스를 협력해서 격파하는 게임.

---

### 게임 플레이

- **기본 루프**
  - 로비에서 닉네임을 입력하고 룸에 입장.
  - 모든 플레이어가 준비되면 마스터 클라이언트가 메인 씬을 로드.
  - 공용 Ship 위에 각 플레이어가 소환되고, Ship에 설치된 여러 스테이션(조작 장치)을 맡아 협동 플레이 진행.
  - 적 스포너를 파괴해 나가며 웨이브를 진행하고, 마지막에는 보스전을 통해 스테이지를 클리어.

- **역할 분담**
  - **ShipController**: 우주선의 방향과 부스트를 조작해 장애물을 피하며 전장을 이동.
  - **CannonController**: 캐논 각도를 조절하고 포탄을 발사해 적을 공격, 공격력 버프를 활용해 화력을 담당.
  - **ShieldController**: 방패를 회전시켜 탄막을 막고, 필요 시 방패 내구도를 회복.
  - **InventoryController / Inventory**: Ship 공용 인벤토리의 아이템을 관리하고 사용해 Ship의 능력(이동 속도, 공격력, 회복 등)을 강화.

---

### 멀티플레이 & 협동 구조

- **로비 / 매치메이킹**
  - `LobbyManager`에서 게임 실행 시 Photon 서버에 자동 접속하고 로비에 합류.
  - 닉네임 입력 후 `JoinRandomRoom`으로 간단한 매치메이킹을 구현.
  - `LobbyUI`에서 룸에 접속한 플레이어 닉네임을 슬롯별로 표시하고, 마스터 클라이언트만 **게임 시작 버튼**을 활성화.
  - `PhotonNetwork.AutomaticallySyncScene`을 사용해 마스터가 씬을 변경하면 모든 클라이언트가 동일하게 씬을 이동.

- **Ship 및 플레이어 생성**
  - `GameManager`는 싱글톤으로 게임 전역 상태를 관리.
  - 마스터 클라이언트만 `PhotonNetwork.Instantiate`로 공용 Ship 오브젝트를 한 번만 생성.
  - 각 클라이언트는 자신의 `ActorNumber`에 따라 다른 Player 프리팹(`Player01~04`)을 Ship 근처에 소환해, **한 우주선 위에 여러 플레이어가 함께 탑승하는 구조**를 구현.
  - 플레이어는 Ship 위를 이동하며 원하는 스테이션에 탑승해 역할을 수행.

- **상호작용 추상화 (`BaseInteractionController`)**
  - Ship의 각 조작 스테이션(ShipController, CannonController, ShieldController, InventoryController 등)은 모두 `BaseInteractionController`를 상속.
  - 플레이어가 상호작용 키를 누르면 `TryInteract(PlayerController)`를 통해
    - 좌석 점유/해제 (한 번에 한 명만 조작)
    - PhotonView 소유권 요청 (`RequestOwnership`)  
    - 상호작용 가능 여부를 RPC로 전체 동기화
    를 공통 처리.
  - 덕분에 새로운 조작 스테이션을 추가할 때, `BaseInteractionController`만 상속하면 동일한 협동 구조에 자연스럽게 편입 가능.

---

### 적 / 보스 시스템

- **웨이브 & 스포너 (`StageManager`, `SpawnerManager`)**
  - `StageManager`가 현재 남은 스포너 수를 관리하고, 처음에는 제한된 영역에 첫 스포너를 배치.
  - `SpawnerManager`는 마스터 클라이언트에서만 동작하며, 일정 범위 내 랜덤 위치를 `OverlapCircle`로 검사해 겹치지 않는 자리에 스포너를 Photon으로 생성.
  - 스포너가 파괴될 때마다 카운트를 감소시키고, 모두 제거되면 보스 페이즈를 시작.

- **일반 적 AI (`EnemyController` + FSM)**
  - `EnemyController`는 `EnemyStateMachine`과 여러 `EnemyState`(Idle/Move/Attack 등)로 구성된 상태 머신 기반 AI를 사용.
  - A* Pathfinding Project(`AIPath`, `AIDestinationSetter`)를 활용해 Ship 주변을 추적/배회하는 움직임 구현.
  - 마스터 클라이언트만 Raycast와 거리 계산을 수행해 공격 가능 여부를 판정하고, 상태 머신을 업데이트.
  - 체력 등 주요 전투 데이터는 마스터에서만 변경하고, `OnPhotonSerializeView`와 RPC로 다른 클라이언트에 동기화.

- **보스전 및 분절형 몸체**
  - 보스는 `isBoss` 플래그로 일반 적과 구분되며, 몸체가 여러 세그먼트로 나뉜 지렁이 형태의 패턴을 가짐.
  - `BodyContainer`, `WormSegment` 등을 통해 각 세그먼트가 Ship을 둘러싸거나 추적하는 패턴을 구현.
  - 보스 체력 변화는 RPC를 통해 `EventBusType.BossHealthChange` 이벤트로 UI에 반영되어, 모든 클라이언트에서 동일한 보스 HP 바를 표시.
  - 특정 몸체가 파괴되면 리스트를 재구성하고 타겟/인덱스를 재세팅해, 네트워크 환경에서도 일관된 분절 구조 유지.

---

### 인프라 & 아키텍처

- **EventBus 기반 모듈 간 통신**
  - `EventBus`는 제네릭 / 문자열 / Enum 기반 이벤트를 모두 지원하는 공용 이벤트 허브.
  - UI, 카메라, Ship 상태, 보스/플레이어 로직 등이 서로 직접 참조하지 않고 `EventBus.Publish(EventBusType.XXX, param)` 형태로 통신.
  - Ship/보스 체력, 카메라 줌, 인벤토리/슬롯 활성화, 상점 토글 등 다양한 시스템 간 의존도를 낮추고 확장성과 유지보수성을 확보.

- **MasterClient 권한 위임**
  - 적 AI, 스포너 생성, 아이템 드랍, Ship/보스 체력 관리 등 게임 규칙에 영향을 주는 로직은 **마스터 클라이언트에서만 실행**.
  - 다른 클라이언트는 `OnPhotonSerializeView`와 RPC를 통해 결과를 반영만 하는 구조로 설계해, 네트워크 지연이 있어도 게임 규칙이 어긋나지 않도록 함.

- **공용 Ship 기반 협동 설계**
  - Ship은 `Inventory`, `CoinWallet`, `ShipCondition`, `ShieldCondition`, `ShopController` 등 다양한 컴포넌트를 보유한 “협동의 중심 오브젝트”.
  - 아이템 획득과 사용, 스탯 강화, 체력/방어막 관리가 모두 Ship 단위로 이루어지기 때문에, 플레이어가 자연스럽게 **공동 자원을 관리하고 역할을 나누는 플레이 경험**을 제공.

---

### 기술 스택

- **엔진**: Unity (2D)  
- **언어**: C#  
- **네트워크**: Photon PUN 2  
- **AI / 경로 탐색**: A* Pathfinding Project  
- **UI**: Unity UI, TextMeshPro  
- **아키텍처**: 싱글톤 매니저(`GameManager`, `UIManager`), EventBus, 상태 머신 기반 AI, MasterClient 중심 권한 구조

