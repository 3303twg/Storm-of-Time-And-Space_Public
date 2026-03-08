public interface IInteractable 
{
    // 상호작용 가능 여부를 나타내는 프로퍼티
    bool IsInteractable { get; }

    // 플레이어가 상호작용 키를 눌렀을 때 호출될 함수
    bool TryInteract(PlayerController player);
}
