using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private Weapon currentWeapon;
    private PlayerControls inputActions;
    private Animator animator;

    private void Awake()
    {
        inputActions = new PlayerControls();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.PlayerActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.PlayerActions.Disable();
    }

    private void Start()
    {
        //there could be a choice of weapons but there isn't one yet :(
        currentWeapon = GetComponent<FistAttack>();

        inputActions.PlayerActions.Hit.performed += i => PerformAttack();
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
