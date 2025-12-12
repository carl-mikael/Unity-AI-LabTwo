using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float _speed;

    private InputSystem_Actions _inputActionMap;
    private InputAction moveAction;

    private Vector2 move = Vector2.zero;

    void Awake()
    {
        _inputActionMap = new();
        moveAction = _inputActionMap.Player.Move;
    }

    void OnEnable()
    {
        _inputActionMap.Enable();
    }

    void OnDisable()
    {
        _inputActionMap.Disable();
    }

    void Update()
    {
        move = moveAction.ReadValue<Vector2>();
        move *= _speed * Time.deltaTime;
        this.transform.Translate(move.x, 0, move.y);
    }
}
