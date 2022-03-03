using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : Controller
{
    public Camera cam;
    public GameObject weapen;
    [SerializeField] InputActionReference movementController;
    [SerializeField] InputActionReference RotatController;
    [SerializeField] InputActionReference attack;
    [SerializeField] InputActionReference jump;
    [SerializeField] Sword Sword;
    GameObject vfx;
    [SerializeField] Vector3 Velocity;
    [SerializeField] Vector3 playervelocity;
    float speed = 5;
    float vfxlifetime;
    public Vector3 Direction;
    public bool IsDamage;
    public bool IsGrounded;
    public bool CloseGround;
    bool IsJump;
    public float RotatSpeed = .2f;
    private float gravityValue = -9.81f;
    private float rs;
    bool Pressattack;
   [SerializeField] bool NearGround = true;

    private void OnEnable()
    {
        movementController.action.Enable();
        attack.action.Enable();
        jump.action.Enable();
        RotatController.action.Enable();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    private void OnDisable()
    {
        movementController.action.Disable();
        attack.action.Disable();
        jump.action.Disable();
        RotatController.action.Disable();
        Cursor.visible = true;
    }
    void Start()
    {
        cam = Camera.main;
        DamageTag = "Enemy";
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        SetSwordSetting(Sword);
    }
    private void Update()
    {
        IsGrounded = controller.isGrounded;       
       NearGround = Physics.Raycast(transform.position,-transform.up,.3f);
        if (IsDathe) return;
        IsDamage = animator.GetCurrentAnimatorStateInfo(0).IsTag("Damage");
        Vector2 Dir = movementController.action.ReadValue<Vector2>();
        Direction = new Vector3(Dir.x, 0, Dir.y);
        IsAttack = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
        if (!IsAttack)
        {
            IsAttack = false;
            CanDamage = false;
        }
        else Pressattack = false;


        if (IsDamage) return;
        //IsJump = animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump");

        if (attack.action.triggered)
            Attack();
        if (IsGrounded)
        {
            playervelocity = Vector3.zero;
           
            if (jump.action.triggered)
                Jump();
        }
        if(NearGround || IsGrounded)
        Run(Direction);
        animator.SetFloat("Y_velocity", playervelocity.y);
        playervelocity.y += gravityValue * Time.deltaTime;
        animator.SetBool("Grounded", IsGrounded);
        controller.Move(playervelocity * Time.deltaTime);
    }

    void Jump()
    {
        if (IsAttack || Pressattack) return;
        IsJump = true;
        NearGround = false;
        Debug.Log("you just Jump");
        playervelocity = transform.forward * 3;
        playervelocity.y += Mathf.Sqrt(-3.0f * gravityValue);
        animator.SetTrigger("Jump");
    }
    void Run(Vector3 Direction)
    {
        print(transform.forward);

        speed = 3;
        if (IsAttack)
        {
            Velocity = Vector3.zero;
            return;
        }
        Vector3 Dir = Direction.normalized;
        animator.SetInteger("Speed", (int)(Dir.normalized.magnitude));
        Velocity = speed * Dir.magnitude * Time.deltaTime * transform.forward;
        Velocity.y = 0;
        controller.Move(Velocity);
        if (Direction.magnitude < 0.1f)
        {
            return;
        }
        float angle = (Mathf.Atan2(Dir.x, Dir.z)) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        angle = Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, angle, ref rs, .12f);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
    void Attack()
    {
        if (!controller.isGrounded) return;
        Pressattack = true;
        if (IsAttack)
        {
            animator.SetTrigger("Next");

            return;
        }
        animator.SetTrigger("Attack");

    }
    public void VFXAxe()
    {

        GameObject vf = Instantiate(vfx);
        FindObjectOfType<SoundManager>().Play("Attack");
        ActivVfx pos = vf.GetComponent<ActivVfx>();
        if (pos)
            pos.SetPlayer(weapen);
        Destroy(vf, vfxlifetime);
    }

    public void VFXSword()
    {
        GameObject vf = Instantiate(vfx);
        vf.transform.position = weapen.transform.position;
        vf.transform.rotation = weapen.transform.rotation;
        FindObjectOfType<SoundManager>().Play("Attack");
        Destroy(vf, vfxlifetime);
    }


    public void SetSwordSetting(Sword item)
    {
        animator.SetInteger("WeapenId", item.ID);
        vfxlifetime = item.VFXtime;
        vfx = item.VFX;
    }
    public void SetSword(Sword i)
    {
        Sword = i;
    }
}