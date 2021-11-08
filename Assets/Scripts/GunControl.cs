using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class GunControl : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 2f)]
    float fireRate = .5f;
    [SerializeField]
    [Tooltip("Changes how far the bullets are spread. 0 means no spread")]
    [Range(0f, 1f)]
    float spreadModifier = .2f;
    [SerializeField]
    private float controllerDeadzone = 0.1f;
    [SerializeField]
    float rotationSmoothing = 1000f;

    //this is where the bullet spawns
    GameObject projectileStartPos;
    //this is a shortcut to the parent object but i probably dont need this
    GameObject player;

    private PlayerActionControls playerActionControls;
    private PlayerInput playerInput;

    [SerializeField]
    Pool bulletPool;

    Vector3 direction;
    Vector3 mousePos;
    Vector3 targetLoc;

    GameObject obj;

    bool coolDown;
    float fireTimer;

    bool isGamepad;
    bool isAttacking = false;

    private void Awake()
    {
        playerActionControls = new PlayerActionControls();
    }

    private void OnEnable()
    {
        playerActionControls.Enable();
    }

    private void OnDisable()
    {
        playerActionControls.Disable();
    }
    //Turned on during special scenes like door transitions
    bool freezeFire;
    void Start()
    {
        player = this.gameObject;
        ObjectPool_Projectiles.Instance.InstantiatePool(bulletPool);

        projectileStartPos = this.gameObject.transform.GetChild(0).gameObject;

        coolDown = false;
        fireTimer = 0;
        freezeFire = false;
    }

    void Update()
    {
        Aim();
        //Shoot();
        SpreadShoot();

        if (coolDown)
        {
            //this means our shooting cooldown is affected by time slow
            fireTimer += Time.deltaTime;
            //this could be fun maybe? just need to get the bullets to not explode on one another
            //fireTimer += Time.unscaledDeltaTime;

            if (fireTimer >= fireRate)
            {
                fireTimer = 0;
                coolDown = false;
            }
        }
    }

    void Aim()
    {
        var aim = playerActionControls.Player.Aim.ReadValue<Vector2>();
        if (isGamepad)
        {
            if (Mathf.Abs(aim.x)>controllerDeadzone||Mathf.Abs(aim.y)>controllerDeadzone)
            {
                Vector3 direction = Vector3.right * aim.x + Vector3.forward * aim.y;

                if (direction.sqrMagnitude > 0.0f)
                {
                    Quaternion newRot = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRot, rotationSmoothing * Time.unscaledDeltaTime);
                }
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(aim);
            Plane groundPlane = new Plane(Vector3.up, player.transform.position);

            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
        

        //var aimPos = playerActionControls.Player.Aim.ReadValue<Vector2>();
        //var aimWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, player.transform.position.y));


        //direction = aimWorldPos - player.transform.position;
        //direction.y = 0;
        //transform.forward = direction;

    }

    private void LookAt(Vector3 point)
    {
        Vector3 heightCorrectPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(heightCorrectPoint);
    }

    Vector3 GetMousePos()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
        {
            return hitInfo.point;
        }
        else
            return Vector3.zero;
    }

    void Shoot()
    {
        if (Input.GetMouseButton(0) && !coolDown && !freezeFire)
        {
            //var ray = cam.ScreenPointToRay(Input.mousePosition);
            //if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
            //{
            //    targetLoc = hitInfo.point;
            //    targetLoc.y = 0;
            //    targetLoc = targetLoc.normalized;
            //}
            direction = direction.normalized;
            var obj = ObjectPool_Projectiles.Instance.GetProjectile(bulletPool.prefab.name);
            obj.GetComponent<Projectile>().SetUp(direction, projectileStartPos.transform.position, this.gameObject.tag);
            coolDown = true;
        }
    }

    /// <summary>
    /// Freezes firing during transitions
    /// </summary>
    public void FrezeFire()
    {
        freezeFire = true;
    }
    /// <summary>
    /// UnFreezes firing during transitions
    /// </summary>
    public void UnFrezeFire()
    {
        freezeFire = false;
    }
    
    void SpreadShoot()
    {
        //float offset = (float)Random.Range(-maxSpread, maxSpread);
        if (isAttacking && !coolDown)
        {
            Vector3 target = transform.forward + new Vector3(Random.Range(-spreadModifier, spreadModifier), 0, Random.Range(-spreadModifier, spreadModifier));

            var obj = ObjectPool_Projectiles.Instance.GetProjectile(bulletPool.prefab.name);
            obj.GetComponent<Projectile>().SetUp(target, projectileStartPos.transform.position, this.gameObject.tag);
            coolDown = true;
        }    
    }

    public void OnFire(CallbackContext context)
    {
        if (context.performed)
            isAttacking = true;
        if (context.canceled)
            isAttacking = false;
    }

    public void OnDeviceChange(PlayerInput pi)
    {
        isGamepad = pi.currentControlScheme.Equals("Gamepad") ? true : false;
    }
}
