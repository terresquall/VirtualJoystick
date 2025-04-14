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

            for (int i = 0; i < memory.Count - 30; i++) // Ensure enough frames ahead
            {
                // Check for first tap
                var firstTap = memory[i];
                if (firstTap.magnitude < 0.7f || firstTap.magnitude > 1f)
                    continue;

                // Check for neutral zone after first tap
                bool neutralFound = false;
                for (int j = i + 1; j < i + 10; j++)
                {
                    if (memory[j].magnitude <= deadzone)
                    {
                        neutralFound = true;
                        break;
                    }
                }

                if (!neutralFound)
                    continue;

                // Check for second tap after neutral
                for (int k = i + 10; k < i + 20; k++)
                {
                    Vector2 secondTap = memory[k];
                    if (secondTap.magnitude >= 0.7f && secondTap.magnitude <= 1f)
                    {
                        float angleBetween = Vector2.Angle(firstTap, secondTap);
                        if (angleBetween <= 30f)
                        {
                            direction = firstTap.normalized;
                            memory.Clear(); // Clear after detection
                            return true;
                        }
                    }
                }
            }

            return false;
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
