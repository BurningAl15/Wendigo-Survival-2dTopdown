using UnityEngine;

public class PlayerLivingEntity : LivingEntity
{
    protected override void Die()
    {
        Debug.Log("Player died!");
        
        // TODO: Trigger Game Over
        // GameManager.Instance.GameOver();
        
        base.Die();
    }
}
