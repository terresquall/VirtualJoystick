using UnityEngine;

namespace Terresquall {

    [RequireComponent(typeof(VirtualJoystick))]
    public class VirtualJoystickEdgeFeedback : MonoBehaviour {

        internal AudioSource audioSource;
        VirtualJoystick joystick;
        float cooldown = 0f;

        [Tooltip("The amount of force you will need to hit the edge with to register feedback.")]
        public float deltaThreshold = 10f;
        [Tooltip("After a feedback fires, you will need to wait before another feedback occurs.")]
        public float minimumFeedbackGap = 0.3f;
        [Tooltip("Does the feedback include vibration?")]
        public bool hasVibration = false;

        // Start is called before the first frame update
        void Start() {
            audioSource = GetComponent<AudioSource>();
            joystick = GetComponent<VirtualJoystick>();
        }

        // Update is called once per frame
        void Update() {
            // If not cooled down yet, no need to run the rest of the update.
            if(cooldown > 0) {
                cooldown -= Time.deltaTime;
            }

            // If an audio source is assigned, check if we should play a sound.
            if (joystick.axis.sqrMagnitude >= 1) {
                if ((joystick.GetAxisDelta() / Time.deltaTime).sqrMagnitude > deltaThreshold * deltaThreshold)
                    TriggerFeedback();
            }
        }

        public bool TriggerFeedback()
        {
            if (cooldown > 0f) return false;
            if (audioSource) audioSource.PlayOneShot(audioSource.clip);
            if (hasVibration) Handheld.Vibrate();
            cooldown = minimumFeedbackGap;
            return true;
        }
    }
}