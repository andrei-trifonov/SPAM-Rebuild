using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

public enum AmmoType
{
    pistol,
    shotgun
}

public class Weapon
{
    public Weapon(float fT, float rT, int cSize, int dmg, AmmoType ammo)
    {
        this.damage = dmg;
        this.reloadTime = rT;
        this.fireTime = fT;
        this.clipSize = cSize;
        this.ammoType = ammo;
        ammoLeft = clipSize;
    }

    
    public int ammoLeft;
    public float fireTime;
    public float reloadTime;
    public int clipSize;
    public int damage;
    public AmmoType ammoType;
}

public class PlayerController : MonoBehaviour
{

    public List<GameObject> weaponObjects;
    private List<Weapon> Weapons = new List<Weapon>();
    private int pickWeapon;
    private bool weaponHold;

    public enum PlayerMode { FreeMove, Combat }
    public PlayerMode currentMode = PlayerMode.FreeMove;
   
    [Header("Components")]
    public CharacterController controller;
    public Animator animator;
    public Rig armsRig;

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float backWalkSpeed = 3f;
    public float runSpeed = 6f;
    
    public float runRotationSpeed = 10f;
    public float walkRotationSpeed = 10f;
    public float combatMoveSpeed = 2f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    private float verticalVelocity;
    private bool isGrounded;
 
    public bool fireBlock=false;
    private Vector2 input;
    private bool isRunning;

    public bool nearCorpse;

    
    private void Start()
    {
        Weapons.Add(new Weapon(0,0,0,0,0));
        Weapons.Add(new Weapon(1.15f, 0, -1,10,AmmoType.pistol));
        Weapons.Add(new Weapon(0.41f, 3.15f, 7,10,AmmoType.pistol));
    }

    void Update()
    {
        HandleInput();
        HandleModeSwitch();

        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;

        if (currentMode == PlayerMode.FreeMove)
            HandleFreeMove();
        else
            HandleCombatMove();
    }

    void HandleInput()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isRunning = Input.GetKey(KeyCode.LeftShift);
        for(int i=0;i<10;i++)
        {
            if(Input.GetKeyDown((KeyCode)(48+i)))
            {
                pickWeapon = i;
                ChangeWeapon();
            }
        }
    }

    void ChangeWeapon()
    {
        animator.SetBool("Aim", false);
        animator.SetBool("KnifeAttack", false);
        for (int i = 1; i < Weapons.Count; i++)
        {
            Debug.Log(i);
            weaponObjects[i].SetActive(false);
            animator.SetLayerWeight(i, 0);
            weaponHold = true;
        }
          
        

        weaponObjects[pickWeapon].SetActive(true);
        animator.SetLayerWeight(pickWeapon, 1);
       
    }
    void HandleModeSwitch()
    {

        if (weaponHold && Input.GetMouseButtonDown(1)) // ПКМ — вход/выход в боевой режим
        {
            if (currentMode == PlayerMode.FreeMove)
                EnterCombatMode();
            else
                ExitCombatMode();
        }
        if (Input.GetMouseButtonDown(0)&&fireBlock== false && currentMode == PlayerMode.Combat) // ПКМ — вход/выход в боевой режим
        {
            if ( Weapons[pickWeapon].ammoLeft>0 || Weapons[pickWeapon].ammoLeft==-1)
                StartCoroutine(FireBullet());
            else 
                StartCoroutine(Reload());

        }
    }

    IEnumerator FireBullet()
    {
    
        animator.SetInteger("RandAttack", UnityEngine.Random.Range(0,2));       
        fireBlock = true;
        if (Weapons[pickWeapon].ammoLeft>0)
            Weapons[pickWeapon].ammoLeft -= 1;
        //armsRig.weight =0;

        animator.SetTrigger("Fire");
        yield return new WaitForSeconds(Weapons[pickWeapon].fireTime);
        fireBlock = false;
     
    }
    IEnumerator Reload()
    {
        fireBlock = true;
        Weapons[pickWeapon].ammoLeft = Weapons[pickWeapon].clipSize;
        //armsRig.weight =0;
        animator.SetTrigger("Reload");
        yield return new WaitForSeconds(Weapons[pickWeapon].reloadTime);
        fireBlock = false;
    }
    void EnterCombatMode()
    {
        
        if (pickWeapon == 1)
        {
            animator.SetBool("KnifeAttack", true);
        }
        
        else
        {
            animator.SetBool("KnifeAttack", false);
        }
        currentMode = PlayerMode.Combat;
      

    }

    void ExitCombatMode()
    {
        currentMode = PlayerMode.FreeMove;
      

    }

    private float t1;

    void HandleFreeMove()
    {
        animator.SetBool("Aim", false);
   
        
        
        if (armsRig.weight > 0)
        {
            armsRig.weight -= Time.deltaTime * 4;
        }
        
        
        
        if (!weaponHold)
        {
            if (animator.GetLayerWeight(pickWeapon) > 0)
            {
                t = animator.GetLayerWeight(pickWeapon);
                t -= Time.deltaTime * 4;
                animator.SetLayerWeight(pickWeapon, t);

            }
        }
        else{
                 if (animator.GetLayerWeight(pickWeapon) < 1)
                 {
                     t = animator.GetLayerWeight(pickWeapon);
                     t += Time.deltaTime * 4;
                     animator.SetLayerWeight(pickWeapon, t);
                    
                 }
  
            
            
        }
        
        float horizontal = input.x;
        float vertical = input.y;

        // Танковое управление: всегда двигаться вперёд-назад по forward
        Vector3 moveDirection = transform.forward * vertical;
        moveDirection.Normalize();
        
        bool IsBackwards = vertical < 0;
        
        animator.SetBool("IsBackwards", IsBackwards);
        float speed = isRunning&&!IsBackwards ? runSpeed : IsBackwards? backWalkSpeed : walkSpeed;
        
        Vector3 velocity = moveDirection * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        // Вращаем персонажа вокруг своей оси
        transform.Rotate(Vector3.up * horizontal * (isRunning && !IsBackwards ? runRotationSpeed : walkRotationSpeed ) * Time.deltaTime);
        float Speed = Mathf.Abs(vertical) * (isRunning ? 1.5f : 1f);

       
      
        animator.SetFloat("Speed", Speed);
        animator.SetBool("IsRunning", isRunning && !IsBackwards);
        animator.SetBool("IsGrounded", isGrounded);

        bool isTurningLeft = vertical == 0 && horizontal < -0.1f && Speed < 0.1f;
        bool isTurningRight = vertical == 0 && horizontal > 0.1f && Speed < 0.1f;

        animator.SetBool("IsTurningLeft", isTurningLeft);
        animator.SetBool("IsTurningRight", isTurningRight);
    }

    private float t=0;


    void HandleCombatMove()
    {
        
        animator.SetBool("Aim", true);
        
 

        float horizontal = input.x;
        float vertical = input.y;

        // Танковое управление: всегда двигаться вперёд-назад по forward
        Vector3 moveDirection = transform.forward * vertical;
        moveDirection.Normalize();
        
        bool IsBackwards = vertical < 0;
        
        animator.SetBool("IsBackwards", IsBackwards);
        float speed =  walkSpeed;
        
        Vector3 velocity = moveDirection * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
        float Speed = Mathf.Abs(vertical);
     
            // Вращаем персонажа вокруг своей оси
            transform.Rotate(Vector3.up * horizontal * walkRotationSpeed * Time.deltaTime);
          



            animator.SetFloat("Speed", Speed);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsGrounded", isGrounded);

            bool isTurningLeft = vertical == 0 && horizontal < -0.1f && Speed < 0.1f;
            bool isTurningRight = vertical == 0 && horizontal > 0.1f && Speed < 0.1f;

            animator.SetBool("IsTurningLeft", isTurningLeft);
            animator.SetBool("IsTurningRight", isTurningRight);
       
    }
}
