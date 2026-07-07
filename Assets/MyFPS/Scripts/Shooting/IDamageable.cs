namespace MyFPS
{
    /// <summary>
    /// 대미지를 받을 수 있는 오브젝트가 구현하는 인터페이스
    /// EnemyAI, DestructibleObject 등에서 사용
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int amount);
    }
}
