using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputTest : MonoBehaviour
{
    public InputAction move;

    public InputAction Axis;

    public InputAction vector2d;
    
    public InputAction mixedAxis;
    // Start is called before the first frame update
    void Start()
    {
        move.Enable();
        
        Axis.Enable();
        
        vector2d.Enable();
        mixedAxis.Enable();
        
        move.started += OnMoveStarted;
        
        move.performed += OnMovePerformed;
        
        move.canceled += OnMoveCanceled;
        
        mixedAxis.performed += OnMixedAxisPerformed;
    }

    private void OnMixedAxisPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Mixed Axis Performed");
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        Debug.Log("Move Ended");
        
        Debug.Log(obj.phase);
        Debug.Log(obj.action.name);
        Debug.Log(obj.control.name);
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        Debug.Log("Move Started");
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Move Performed");
    }
        
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Axis.ReadValue<float>());
        Debug.Log(vector2d.ReadValue<Vector2>());
    }
}
