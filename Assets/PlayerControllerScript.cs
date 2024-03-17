using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    public PlayerInputs inputs;
    public CarScript carScript;

    private void OnEnable()
    {
        inputs = new PlayerInputs();

        inputs.PlayerControls.Enable();
    }





    // Start is called before the first frame update
    void Awake()
    {

        //carScript = XizukiMethods.GameObjects.Xi_Helper_GameObjects.ConditionalAssignment<CarScript>(GetComponent<CarScript>());
    }

    public bool hasInput;

    // Update is called once per frame 
    void FixedUpdate()
    {
        hasInput = false;

        FocusedInputs();
   
    }
    private void LateUpdate()
    {
        if (!hasInput)
        {
            carScript.AutoTilt();
        }
    }

    public void FocusedInputs()
    {
        if(GameManager.instance.state == GameState.Interval) { return; }
        if (!Application.isFocused) return;

        if (inputs.PlayerControls.MoveLeft.IsPressed())
        {
            MoveLeft();
        }
        if (inputs.PlayerControls.MoveRight.IsPressed())
        {
            MoveRight();
        }
    }

    public void MoveLeft()
    {
        carScript.Move(-1);
        hasInput = true;
    }
 
    public void MoveRight()
    {
        carScript.Move(1);
        hasInput = true;
    }


    public void EEGMoveLeft()
    {
        if (GameManager.instance.state == GameState.Interval) { return; }
        if (Application.isFocused) return;

        MoveLeft();
    }

    public void EEGMoveRight()
    {
        if (GameManager.instance.state == GameState.Interval) { return; }
        if (Application.isFocused) return;

        MoveRight();
        }
}
