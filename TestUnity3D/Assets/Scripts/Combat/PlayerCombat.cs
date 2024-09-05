using Main;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Weapon currentWeapon;
    private Animator animator;
    private InputHandler inputHandler; 

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        animator = GetComponent<Animator>();
        currentWeapon = GetComponent<Weapon>(); 
    }

    private void Update()
    {
        if (inputHandler.isAttacking)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        if (currentWeapon != null)
        {
            animator.SetTrigger("isAttacking");
            currentWeapon.Attack();
        }
    }
}
