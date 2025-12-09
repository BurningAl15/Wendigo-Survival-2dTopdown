using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Fire Points")]
    [SerializeField] private Transform firePointR;
    [SerializeField] private Transform firePointL;

    [Header("Weapons")]
    [SerializeField] private List<WeaponBase> weapons = new();
    [SerializeField] private int currentWeaponIndex = 0;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private static readonly int ShootIndexHash = Animator.StringToHash("shootIndex");

    private bool isAttackHeld;
    private int currentFirePointIndex = 0;

    private WeaponBase CurrentWeapon =>
        weapons != null && weapons.Count > 0 &&
        currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Count
            ? weapons[currentWeaponIndex]
            : null;

    private Transform CurrentFirePoint => currentFirePointIndex == 0 ? firePointR : firePointL;

    private void Start()
    {
        if (firePointR == null) firePointR = transform;
        if (firePointL == null) firePointL = transform;
        
        EquipCurrentWeapon();
        
        if (animator != null)
        {
            animator.SetInteger(ShootIndexHash, -1);
        }
    }

    private void Update()
    {
        if (isAttackHeld && CurrentWeapon != null)
        {
            TryAttack();
        }
    }

    public void SetAttackHeld(bool held)
    {
        isAttackHeld = held;
        
        if (!held)
        {
            currentFirePointIndex = 0;
            
            if (animator != null)
            {
                animator.SetInteger(ShootIndexHash, -1);
            }
        }
    }

    private void TryAttack()
    {
        if (CurrentWeapon == null) return;
        if (!CurrentWeapon.CanFire) return;

        currentFirePointIndex = (currentFirePointIndex + 1) % 2;

        if (animator != null)
        {
            animator.SetInteger(ShootIndexHash, currentFirePointIndex);
        }

        CurrentWeapon.AttackFromPoint(CurrentFirePoint);
    }

    public void NextWeapon()
    {
        if (weapons == null || weapons.Count == 0) return;

        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Count)
            currentWeaponIndex = 0;

        EquipCurrentWeapon();
    }

    public void PreviousWeapon()
    {
        if (weapons == null || weapons.Count == 0) return;

        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Count - 1;

        EquipCurrentWeapon();
    }

    private void EquipCurrentWeapon()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] == null) continue;

            bool active = (i == currentWeaponIndex);
            weapons[i].gameObject.SetActive(active);

            if (active) weapons[i].OnSelect();
            else weapons[i].OnDeselect();
        }
    }
}
