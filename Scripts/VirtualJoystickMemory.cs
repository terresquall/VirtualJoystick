using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terresquall {
    public partial class VirtualJoystick : MonoBehaviour {

        [Header("Input Memory")]
        public int totalMemoryFrames = 99;
        List<Vector2> memory = new List<Vector2>();
        List<Vector2> selectedRange;

        void FixedUpdate() {
            memory.Insert(0, axis);
            if (memory.Count > 99)
                memory.RemoveRange(totalMemoryFrames, memory.Count - totalMemoryFrames);
        }

        public VirtualJoystick ForFrames(int start, int end) {
            selectedRange = memory.GetRange(start, end);
            return this;
        }

        // Complete this.
        public bool HasInput(float angleStart, float angleEnd, float minIntensity = -1f, float maxIntensity = 1f) {
            if (minIntensity < 0) minIntensity = deadzone;
            return true;
        }

        // Complete this.
        public bool HasNeutral() {
            return true;
        }

        public bool GetDoubleTap()
        {
            bool b1 = ForFrames(1, 20).HasInput(-25, 25, 0.7f, 1f);
            // Check for neutral input.
            bool b2 = ForFrames(10, 50).HasNeutral();
            // In the last 40 to 60 frames.
            bool b3 = ForFrames(40, 60).HasInput(-25, 25, 0.7f, 1f);

            return b1 & b2 & b3;
        }
    }
}
