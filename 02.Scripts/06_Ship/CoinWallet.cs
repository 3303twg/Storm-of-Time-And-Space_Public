public class CoinWallet
{
    public int Coin { get; private set; }

    public CoinWallet(int initialCoin = 0)
    {
        Coin = initialCoin;
    }

    public void Init()
    {
        EventBus.Publish(EventBusType.CoinValueUpdate, Coin);
    }

    public void AddCoin(int amount) // 코인 획득
    {
        Coin += amount;
        EventBus.Publish(EventBusType.CoinValueUpdate, Coin);
    }

    public bool TrySpendCoin(int amount) // 코인 소모
    {
        if (Coin < amount)
        {
            Logger.Log("골드가 부족합니다.");
            return false;
        }

        Coin -= amount;
        EventBus.Publish(EventBusType.CoinValueUpdate, Coin);
        return true;
    }
}
