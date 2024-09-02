using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Weapon currentWeapon;

    private void Start()
    {
        currentWeapon = GetComponent<FistAttack>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.Attack();
        }
    }
}
