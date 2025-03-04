using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    protected Enemy enemy => GetComponentInParent<Enemy>();

    private void Start()
    {
        // enemy = GetComponentInParent<Enemy>();
    }
    protected virtual void AnimationTrigger()
    {
        enemy.AniantionTrigger();
    }
    protected virtual void HitAnimationTrigger()
    {
        enemy.isHitOver = true;
    }
    private void StopAnimationTrigger()
    {
        enemy.isAnimationStop = true;
    }
    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (var hit in colliders)
        {

            if (hit.GetComponent<entity>() != null)
            {
                // Debug.Log(enemy.GetAttackLayer());
                //Debug.Log(hit.GetComponent<entity>().GetMyLayer());
                if (hit.gameObject.layer == LayerMask.NameToLayer(enemy.GetAttackLayerName()) && !hit.GetComponent<CharacterStats>().isDead)
                {
                    if (hit.GetComponent<entity>().CanBeStunned())
                    {
                        if (hit.GetComponent<player1>() != null)
                        {
                        AudioManager.instance.PlaySFX(2, null);
                           // AttackSense.instance.HitPause(30);
                           // hit.GetComponent<player1>().stateMachine.ChangeState(hit.GetComponent<player1>().idleState);
                            hit.GetComponent<player1>().entityFX.ScreenShake(0.8f, 0.9f);
                        return;
                        }
                        //enemy.stateMachine.ChangeState(enemy.idleState);

                    }
                    enemy.stats.DoDamage(hit.GetComponent<entity>().stats, enemy);
                }
            }


        }
    }

    private void OnDestroyTrigger()
    {
        enemy.anim.speed = 0;
        //Destroy(enemy.gameObject, 10);
        // enemy.ZeroVelocity();
    }
    protected void OpenCounterAttackWindow() => enemy.OpenCounterAttackWindow();
    protected void CloseCounterAttackWindow() => enemy.CloseCounterAttackWindow();
    protected virtual Transform closeEnemy(Transform checktransform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checktransform.position, 50f);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<entity>() != null)
            {
                if (hit.gameObject.layer == LayerMask.NameToLayer(enemy.GetAttackLayerName()) && !hit.GetComponent<CharacterStats>().isDead)
                {


                    float distance = Vector2.Distance(enemy.transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = hit.transform;
                    }
                }
            }
        }
        if (closestEnemy == null)
        {
            closestEnemy = checktransform;
        }
        return closestEnemy;
    }
}
