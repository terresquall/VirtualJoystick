using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Terresquall
{
    public class Player : MonoBehaviour
    {
        //Movement
        public float moveSpeed;
        [HideInInspector]
        public Vector2 moveDir;
        [HideInInspector]
        public float lastHorizontalVector;
        [HideInInspector]
        public float lastVerticalVector;

        public float tmpTime;

        int minutes, seconds;
        float miliseconds;

        public TextMeshProUGUI timeText;
        public TextMeshProUGUI endText;
        public GameObject endScreen;

        public bool dead = false;
        Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            DisplayTimer();
        }

        void FixedUpdate()
        {
            Move();
        }

        void Move()
        {
            rb.velocity = new Vector2(VirtualJoystick.GetAxis("Horizontal") * moveSpeed, VirtualJoystick.GetAxis("Vertical") * moveSpeed);
        }

        void DisplayTimer()
        {
            if (!dead)
            {
                tmpTime += Time.deltaTime;

                minutes = (int)tmpTime / 60;

                seconds = (int)tmpTime % 60;

                miliseconds = tmpTime * 1000;

                miliseconds = miliseconds % 1000;

                timeText.text = string.Format("{00:00}:{01:00}:{02:000}", minutes, seconds, miliseconds);
            }
            else
            {
                endScreen.SetActive(true);
                endText.text = "You Survived: " + string.Format("{00:00}:{01:00}:{2:000}", minutes, seconds, miliseconds);
            }
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.gameObject.CompareTag("Bullet"))
            {
                dead = true;
            }
        }
    }
}

