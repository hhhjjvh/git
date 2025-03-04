using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
   private player1 player=> GetComponentInParent<player1>();
    private void AnimationTrigger()
    {
        player.AniantionTrigger();
    }
    public void AnimationEnterTrigger()
    {
       
         AudioManager.instance.PlaySFX(0, null);
    }
    protected virtual void HitAnimationTrigger()
    {
        player.isHitOver = true;
    }
    private void StopAnimationTrigger()
    {
       player.isAnimationStop = true;
    }
    private void OpenAttackTrigger()
    {
        player.OpenCounterAttackWindow();
       
    }
    private void CloseAttackTrigger()
    {
        player.CloseCounterAttackWindow();
    }
    private void AttackTrigger()
    {
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        //foreach (var hit in colliders)
        //{
        //    if(hit.GetComponent<Enemy>()!= null&& !hit.GetComponent<CharacterStats>().isDead)
        //    {
        //        AudioManager.instance.PlaySFX(1, player.transform);

        //        EnemyStats enemy = hit.GetComponent<EnemyStats>();
        //        if (enemy == null) return;
        //        player.stats.DoDamage(enemy);
               
        //    }
        //    if(hit.GetComponent<ArrowController>() != null)
        //    {
        //        AudioManager.instance.PlaySFX(2, player.transform);
        //        hit.GetComponent<ArrowController>().FlipArrow();
        //    }
        //}
    }
    private void AttackBigTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead)
            {

                AudioManager.instance.PlaySFX(Random.Range(8, 9), player.transform);
                AudioManager.instance.PlaySFX(1, player.transform);
                AttackSense.instance.HitPause(15);
                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy == null) return;
                player.stats.DoDamage(enemy, 2f);
                hit.GetComponent<Enemy>().MakeKnockbake(new Vector2(5, 3), 1f);
                //SummonEnemy(hit, enemy);
            }
            if (hit.GetComponent<ArrowController>() != null)
            {
                AudioManager.instance.PlaySFX(2, player.transform);
                hit.GetComponent<ArrowController>().FlipArrow();
            }
        }
    }
   
     private void SlideAttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && !hit.GetComponent<CharacterStats>().isDead)
            {
              
                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy == null) return;
                player.stats.DoDamage(enemy,0.3f);

            }
            if (hit.GetComponent<ArrowController>() != null)
            {
                AudioManager.instance.PlaySFX(2, player.transform);
                hit.GetComponent<ArrowController>().FlipArrow();
            }
        }
    }
    private void SummonEnemy(Collider2D hit, EnemyStats enemy)
    {
        if (enemy.isDead)
        {
            EnemyName enemyname = hit.GetComponent<Enemy>().enemyName;
           // EnemyFactory.Instance.GetCharmEnemy(enemyname, Random.Range(1, player.GetComponent<PlayerStats>().GetLevel()), hit.transform.position);
        }
    }

    private void OnDestroyTrigger()
    {
        player.anim.speed= 0;   
       // Destroy(player.gameObject, 10);
    }
    private void ThrowSword()
    {
        SkillManager.instance.sword.CreateSword();
        AudioManager.instance.PlaySFX(13, player.transform);
    }
        

}
