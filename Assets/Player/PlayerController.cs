using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable, IMoveable, IJumpable, IFallable//, IAnimateable
{
    [SerializeField]
    float playerHeight, playerWidth;

    #region speed variables

    [Header("x speed")]
    public float _maxXSpeed;
    public float _xAcceleration;
    public float _decelerationRate;
    public float _currentXSpeed;
    public float _airAccelrationFactor = 0.25f;

    [Header("y speed")]
    public float _minYSpeed;
    public float _yAcceleration;
    public float _currentYSpeed;

    public float _intialJumpVelocity;

    #endregion

    #region animations

    [Header("Animator")]
    [SerializeField]
    Animator _animator;
    public enum PLAYER_ANIMATION { Idle, Walking, Jumping, Falling, Death }
    PLAYER_ANIMATION _currentAnimation = PLAYER_ANIMATION.Idle;
    [SerializeField]
    SpriteRenderer _renderer;

    #endregion

    #region interface fields
    //IMoveable
    public bool _isFacingRight { get; set; } = true;


    //IFallable
    [field: SerializeField] public bool _isGrounded { get; set; } = true;
    [field: SerializeField] public LayerMask _groundLayers { get; set; }

    //IJumpable 
    public bool _isJumping { get; set; }

    [field: SerializeField, Header("IJumpable")] public float _coyoteTime { get; set; } = 0.1f;

    public float _coyoteTimer { get; set; }

    #endregion

    #region State Machine States
    public StateMachine _StateMachine { get; set; }
    public PlayerIdleState _playerIdleState { get; set; }
    public PlayerWalkingState _playerWalkingState { get; set; }
    public PlayerJumpingState _playerJumpingState { get; set; }
    public PlayerAirborneState _playerAirborneState { get; set; }



    #endregion

    Camera camera;
    [SerializeField]
    public float minCameraX, maxCameraX, minCameraY, maxCameraY, cameraYOffset, cameraSize;

    List<IngredientObject> unlockedIngredients;
    [SerializeField]
    IngredientMixingMenu ingredientMenu;

    [SerializeField]
    LightSource lantern;

    public int touchingDivineLight;

    public Vector3 respawnPosition;
    public bool isDead;

    [SerializeField]
    Image deathFadeImage;
    [SerializeField]
    float deathFadeDuration;
    float deathFadeTimer, fadeInTimer;

    public bool gameOver;
    [SerializeField]
    GameOverManager gameOverManager;

    [SerializeField]
    AudioSource deathSound;

void Awake()
    {
        _StateMachine = new StateMachine();
        _playerIdleState = new PlayerIdleState(this, _StateMachine);
        _playerWalkingState = new PlayerWalkingState(this, _StateMachine);
        _playerJumpingState = new PlayerJumpingState(this, _StateMachine);
        _playerAirborneState = new PlayerAirborneState(this, _StateMachine);
    }

    // Start is called before the first frame update
    void Start()
    {

        _currentAnimation = PLAYER_ANIMATION.Jumping;
        changeAnimation(PLAYER_ANIMATION.Idle);

        _currentXSpeed = 0;
        _currentYSpeed = 0;

        _StateMachine.Initialize(_playerIdleState);

        camera = Camera.main;

        unlockedIngredients = new List<IngredientObject>();
        touchingDivineLight = 0;

        isDead = false;
        fadeInTimer = 0;

        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        _StateMachine.currentState.FrameUpdate();
        if (isDead && !gameOver) { DeathFade(); }
        else if (fadeInTimer < deathFadeTimer && !gameOver) { FadeIn(); }
    }



    #region Killable

    public void Kill()
    {
        if (isDead) { return; }
        changeAnimation(PlayerController.PLAYER_ANIMATION.Death);
        lantern.Kill();
        isDead = true;
        deathFadeTimer = -1;
        deathSound.Play();
    }

    #endregion



    #region Movement
    //Interface implementations
    public void Move()
    {

        CheckIfGrounded();
        CheckForWallCollision();

        Vector3 nextPos = transform.position;
        nextPos.x += _currentXSpeed * Time.deltaTime;
        nextPos.y += _currentYSpeed * Time.deltaTime;
        transform.position = nextPos;

        UpdateCameraPosition();
    }

    public void UpdateCameraPosition()
    {
        Vector3 destPos = camera.transform.position;
        destPos.x = Mathf.Clamp(transform.position.x, minCameraX, maxCameraX);
        destPos.y = Mathf.Clamp(transform.position.y + cameraYOffset, minCameraY, maxCameraY);

        float decay = 20;
        Vector3 newPos = camera.transform.position;
        newPos.x = expDecay(newPos.x, destPos.x, decay, Time.deltaTime);
        newPos.y = expDecay(newPos.y, destPos.y, decay, Time.deltaTime);

        camera.orthographicSize = expDecay(camera.orthographicSize, cameraSize, 5, Time.deltaTime);

        camera.transform.position = newPos;
    }

    //smoother lerp
    public static float expDecay(float a, float b, float decay, float deltaTime)
    {
        return b + ((a - b) * Mathf.Exp(-decay * deltaTime));
    }

    public void checkDirectionToFace(float accel)
    {
        if (accel != 0)
        {
            _isFacingRight = accel > 0;
            _renderer.flipX = !_isFacingRight;
        }
    }

    public void CheckIfGrounded()
    {
        _isGrounded = false;
        int rayCount = 3;
        for (int i = -1; i < rayCount - 1; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x + i * 0.9f * ( playerWidth / 2.0f ), transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, (playerHeight / 2.0f) + 0.1f, _groundLayers);
            if (hit.collider != null)
            {
                _isGrounded = true;
                if (_currentYSpeed < 0)
                {
                    //set player height to ground height to prevent floating or clipping
                    Vector3 groundPos = transform.localPosition;
                    groundPos.y = hit.point.y + playerHeight / 2.0f;
                    transform.position = groundPos;
                    _currentYSpeed = 0;
                }
                _coyoteTimer = 0.0f;
                return;
            }
        }

        Fall();

    }

    public void CheckForWallCollision()
    {
        if (_currentXSpeed == 0) { return; }

        Vector2 collisionCheckDirection;

        if (_currentXSpeed > 0) { collisionCheckDirection = Vector2.right; }
        else { collisionCheckDirection = Vector2.left; }


        int rayCount = 5;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - (playerHeight /2.0f) + playerHeight / i);
            //prevent minor floor clipping from stopping movement
            if (i == 0)
            {
                rayOrigin.y += 0.05f;
            }

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, collisionCheckDirection, (playerWidth / 2.0f) + 0.02f, _groundLayers);
            if (hit.collider != null) { _currentXSpeed = 0; return; }
        }

    }

    public void CheckForHeadbonk()
    {
        int rayCount = 3;
        for (int i = -1; i < rayCount - 1; i++)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x + i * 0.7f * (playerWidth / 2.0f), transform.position.y);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, (playerHeight / 2.0f) + 0.02f, _groundLayers);
            if (hit.collider != null)
            {
                if (_currentYSpeed > 0)
                {
                    _currentYSpeed = 0;
                }
                return;
            }
        }
    }

    //extra
    public void Decelerate()
    {
        float accelertionAmount = _decelerationRate * Time.deltaTime;
        if (_currentXSpeed > 0) { _currentXSpeed = Mathf.Max(0, _currentXSpeed -= accelertionAmount / 2.0f); }
        else if (_currentXSpeed < 0) { _currentXSpeed = Mathf.Min(_currentXSpeed += accelertionAmount / 2.0f, 0); }
    }

    public void Fall()
    {
        _currentYSpeed = Mathf.Max(_minYSpeed, _currentYSpeed += _yAcceleration * Time.deltaTime);
    }

    public void Jump()
    {
        if (isDead) { return; }
        _currentYSpeed = _intialJumpVelocity;// + Mathf.Abs(_currentXSpeed) / _maxXSpeed * _intialJumpVelocity / 6.0f;
        _isJumping = true;
    }

    #endregion

    //#region animationTriggers

    /*public enum ANIMATION_TRIGGER_TYPE { ATTACKING_FRAME, FINISHED_ATTACKING }

    void AnimationTriggerEvent(ANIMATION_TRIGGER_TYPE animationTriggerType)
    {
        _StateMachine.currentState.AnimationTriggerEvent(animationTriggerType);
    }

    #endregion*/

    #region animations
    public void changeAnimation(PLAYER_ANIMATION nextAnimation)
    {
        if (isDead) { return; }
        if (_currentAnimation == nextAnimation) { return; }

        _animator.Play(System.Enum.GetName(typeof(PLAYER_ANIMATION), nextAnimation));
        _currentAnimation = nextAnimation;
    }

    public void PauseAnimation()
    {
        _animator.speed = 0;
    }

    public void UnpauseAnimation()
    {
        _animator.speed = 1.0f;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("ingredient"))
        {
            ingredientMenu.UnlockIngredient(collision.gameObject.GetComponent<Ingredient>().Pickup());
        }
        else if (collision.gameObject.CompareTag("GrenadePickup"))
        {
            collision.gameObject.GetComponent<GrenadeUnlocker>().Pickup();
        }
        else if (collision.gameObject.CompareTag("Note"))
        {
            collision.gameObject.GetComponent<AlchemistNote>().Activate();
        }
        else if (collision.gameObject.CompareTag("DivineLight"))
        {
            touchingDivineLight++;
        }
        else if (collision.gameObject.CompareTag("Trigger"))
        {
            collision.gameObject.GetComponent<Trigger>().TriggerEffects(this);
        }
        else
        {
            _StateMachine.currentState.HandleTriggerCollision(collision);
        }    
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RedLight"))
        {
            if (touchingDivineLight > 0 || lantern.currentLightType == LightSource.LIGHT_TYPE.divine && !gameOver)
            {
                gameOverManager.playNeutralEnd();
                gameOver = true;
                Kill();
            }
            else
            {
                if (lantern.currentLightType != LightSource.LIGHT_TYPE.black) { Kill(); }
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Note"))
        {
            collision.gameObject.GetComponent<AlchemistNote>().Deactivate();
        }
        else if (collision.gameObject.CompareTag("DivineLight"))
        {
            touchingDivineLight--;
        }
    }

    public void Respawn()
    {
        if (lantern.currentLightType == LightSource.LIGHT_TYPE.red) { lantern.changeLightType(LightSource.LIGHT_TYPE.magical); }
        isDead = false;
        changeAnimation(PlayerController.PLAYER_ANIMATION.Falling);
        changeAnimation(PlayerController.PLAYER_ANIMATION.Idle);
        transform.position = respawnPosition;
        lantern.Respawn();
        fadeInTimer = -0.4f;
    }

    void DeathFade()
    {
        deathFadeTimer += Time.deltaTime;
        Color color = Color.black;
        color.a = Mathf.Lerp(0, 1, deathFadeTimer / deathFadeDuration );
        deathFadeImage.color = color;

        if (deathFadeTimer > deathFadeDuration) { Respawn(); }
    }

    void FadeIn()
    {
        fadeInTimer += Time.deltaTime;
        Color color = Color.black;
        color.a = Mathf.Lerp(1, 0, fadeInTimer / deathFadeDuration);
        deathFadeImage.color = color;
    }
}