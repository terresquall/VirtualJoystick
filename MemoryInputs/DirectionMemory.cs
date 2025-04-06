using System.Collections.Generic;
using Terresquall;
using UnityEngine;

public class DirectionMemory : MonoBehaviour
{
    private VirtualJoystick joystick;

    private List<JoystickData> inputMemory = new List<JoystickData>();
    private Vector2 lastDirection = Vector2.zero;

    public InputData[] inputData;   
    private int frameTimer = 0; 
    private const int maxFrames = 100;    
    private bool comboTriggered = false;  

    [System.Serializable]
    public struct JoystickData
    {
        public Vector2 direction;
        public float intensity;
        public Direction namedDirection;

        public JoystickData(Vector2 dir)
        {
            direction = dir.normalized;
            intensity = dir.magnitude;
            namedDirection = GetNamedDirection(dir);
        }
    }

    //translates joystick input to direction (e.g. DownLeft)
    public static Direction GetNamedDirection(Vector2 input)
    {
        if (input.magnitude < 0.1f)
            return Direction.Neutral;

        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        if (angle >= 337.5f || angle < 22.5f)
            return Direction.Right;
        if (angle >= 22.5f && angle < 67.5f)
            return Direction.UpRight;
        if (angle >= 67.5f && angle < 112.5f)
            return Direction.Up;
        if (angle >= 112.5f && angle < 157.5f)
            return Direction.UpLeft;
        if (angle >= 157.5f && angle < 202.5f)
            return Direction.Left;
        if (angle >= 202.5f && angle < 247.5f)
            return Direction.DownLeft;
        if (angle >= 247.5f && angle < 292.5f)
            return Direction.Down;
        if (angle >= 292.5f && angle < 337.5f)
            return Direction.DownRight;

        return Direction.Neutral;
    }

    void Start()
    {
        joystick = GetComponent<VirtualJoystick>();
    }

    void Update()
    {
        if (joystick != null)
        {
            Vector2 axis = joystick.GetAxis();
            Vector2 filteredAxis = axis.magnitude < joystick.deadzone ? Vector2.zero : axis.normalized;

            // ensures that the direction is only added when it is different f
            if (filteredAxis != lastDirection)
            {
                JoystickData data = new JoystickData(filteredAxis);
                inputMemory.Add(data);
                lastDirection = filteredAxis;
                frameTimer = 0; 
            }

            //ensure input memory wont exceed 30 inputs
            if (inputMemory.Count > 30)
                inputMemory.RemoveAt(0);
        }

        frameTimer++; // Increase frame timer every frame

        // checks if any of the combo sequences matches
        foreach (InputData comboData in inputData)
        {
            if (!comboTriggered && HasInputSequence(comboData.comboSequence))
            {
                comboTriggered = true; 
                Debug.Log($"{comboData.comboName} triggered");
            }
        }

        //resets the memory every 60frames
        if (frameTimer > maxFrames)
        {
            inputMemory.Clear(); 
            frameTimer = 0;
            comboTriggered = false;
        }
    }

    public bool HasInputSequence(List<Direction> sequence)
    {
        // Check if the sequence length matches and the sequence matches the input memory
        if (inputMemory.Count < sequence.Count)
            return false;

        // check if inputs matches any combosequence
        for (int i = 0; i < sequence.Count; i++)
        {
            if (inputMemory[inputMemory.Count - sequence.Count + i].namedDirection != sequence[i])
            {
                return false;
            }
        }

        return true;
    }
}
