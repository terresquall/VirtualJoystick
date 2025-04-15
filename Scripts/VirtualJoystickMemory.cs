using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terresquall
{
    public partial class VirtualJoystick : MonoBehaviour
    {
        [Header("Input Memory")]
        public int totalMemoryFrames = 99;

        List<Vector2> memory = new List<Vector2>();
        List<Vector2> selectedRange;


        void FixedUpdate()
        {
            memory.Insert(0, axis);

            if (memory.Count > totalMemoryFrames)
                memory.RemoveRange(totalMemoryFrames, memory.Count - totalMemoryFrames);
        }

        public VirtualJoystick ForFrames(int start, int end)
        {
            int count = Mathf.Clamp(end - start, 0, memory.Count - start);
            selectedRange = memory.GetRange(start, count);
            return this;
        }

        public bool HasInput(float angleStart, float angleEnd, float minIntensity = -1f, float maxIntensity = 1f)
        {
            if (minIntensity < 0) minIntensity = deadzone;

            foreach (var input in selectedRange)
            {
                float magnitude = input.magnitude;
                if (magnitude < minIntensity || magnitude > maxIntensity)
                    continue;

                float angle = Vector2.SignedAngle(Vector2.right, input.normalized);
                if (angle >= angleStart && angle <= angleEnd)
                    return true;
            }

            return false;
        }

        public bool HasNeutral()
        {
            foreach (var input in selectedRange)
            {
                if (input.magnitude <= deadzone)
                    return true;
            }

            return false;
        }

        public bool GetDoubleTapDirection(out Vector2 direction)
        {
            direction = Vector2.zero;

            if (memory.Count < totalMemoryFrames)
                return false;

            for (int i = 0; i < memory.Count - 30; i++) // look ahead to avoid out of range
            {
                if (!IsStrongTap(memory[i]))
                    continue;

                if (!HasNeutralBetween(i + 1, i + 10))
                    continue;

                int secondTapIndex = FindMatchingSecondTap(i + 10, i + 20, memory[i]);
                if (secondTapIndex != -1)
                {
                    direction = memory[i].normalized;
                    memory.Clear(); // prevent repeated triggers
                    return true;
                }
            }

            return false;
        }

        //check if direction is the same for number of frames
        public bool GetHoldDirection(out Vector2 direction, int holdFrames = 20)
        {
            direction = Vector2.zero;

            if (memory.Count < holdFrames)
                return false;

            Vector2 total = Vector2.zero;
            for (int i = 0; i < holdFrames; i++)
            {
                if (!IsStrongTap(memory[i]))
                    return false;

                total += memory[i];
            }

            direction = total.normalized;
            return true;
        }

        public bool GetFlickedTap(out Vector2 direction)
        {
            direction = Vector2.zero;

            if (memory.Count < totalMemoryFrames)
                return false;

            for (int i = 0; i < memory.Count - 30; i++) // look ahead to avoid out of range
            {
                if (!IsStrongTap(memory[i]))
                    continue;

                if (!HasNeutralBetween(i + 1, i + 5))
                { 
                    direction = memory[i].normalized;
                    memory.Clear(); // prevent repeated triggers
                    return true;
                }
            }

            return false;
        }

        private bool IsStrongTap(Vector2 input)
        {
            float mag = input.magnitude;
            return mag >= 0.7f && mag <= 1f;
        }

        private bool HasNeutralBetween(int start, int end)
        {
            end = Mathf.Min(end, memory.Count);
            for (int i = start; i < end; i++)
            {
                if (memory[i].magnitude <= deadzone)
                    return true;
            }
            return false;
        }

        private int FindMatchingSecondTap(int start, int end, Vector2 firstTap)
        {
            end = Mathf.Min(end, memory.Count);
            for (int i = start; i < end; i++)
            {
                Vector2 secondTap = memory[i];
                if (IsStrongTap(secondTap))
                {
                    float angle = Vector2.Angle(firstTap, secondTap);
                    if (angle <= 30f)
                        return i;
                }
            }
            return -1;
        }

        public List<Vector2> GetInputsInRange(float minIntensity, float maxIntensity)
        {
            List<Vector2> result = new List<Vector2>();

            foreach (var input in selectedRange)
            {
                float mag = input.magnitude;
                if (mag >= minIntensity && mag <= maxIntensity)
                    result.Add(input);
            }

            return result;
        }

        private Vector2 AverageVector(List<Vector2> vectors)
        {
            if (vectors.Count == 0) return Vector2.zero;

            Vector2 sum = Vector2.zero;
            foreach (var v in vectors)
                sum += v;

            return sum / vectors.Count;
        }
    }
}
