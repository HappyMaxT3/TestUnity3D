using UnityEngine;
using System.Collections;
using System.Linq;

public class FistAttack : Weapon
{
    public Collider fistHitbox;
    public Animator animator;

    private bool isAttacking = false;

    private void Awake()
    {
        fistHitbox = GetComponentsInChildren<SphereCollider>(true).FirstOrDefault(collider => collider.gameObject.name == "mixamorig:RightHand");
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        fistHitbox.isTrigger = true;
        fistHitbox.enabled = false;
    }

    public override void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("isAttacking");
            StartCoroutine(EnableHitboxForTime());
        }
    }

    private IEnumerator EnableHitboxForTime()
    {
        fistHitbox.enabled = true;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        fistHitbox.enabled = false;
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit!");
        }
    }
}
