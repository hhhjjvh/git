using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    // Start is called before the first frame update
    private Player player;
    void Start()
    {
        player = GetComponentInParent<Player>();
        
    }
    private void AnimationTrigger()
    {
        player.AttackOver();
    }
    // Update is called once per frame
    void Update()
    {

    }
}