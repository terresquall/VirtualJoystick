using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Terresquall
{
    public class Player : MonoBehaviour
    {

        //Movement
        public float moveSpeed;
        public float tmpTime;

        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI endText;

        public GameObject endScreen;
        public GameObject playerBullet;

        public Transform gunPivot;

        public bool asteroidGame = false;

        public int points = 0;

        int minutes, seconds;
        float miliseconds, gunTimer;
        bool dead = false;
        Rigidbody2D rb;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            DisplayUI();
        }

        void FixedUpdate()
        {
            Move();
        }

        void Move()
        {
            rb.velocity = new Vector2(VirtualJoystick.GetAxis("Horizontal") * moveSpeed, VirtualJoystick.GetAxis("Vertical") * moveSpeed);
        }

        void DisplayUI()
        {
            if (!asteroidGame)
            {
                if (!dead)
                {
                    tmpTime += Time.deltaTime;

                    minutes = (int)tmpTime / 60;

                    seconds = (int)tmpTime % 60;

                    miliseconds = tmpTime * 1000;

                    miliseconds = miliseconds % 1000;

                    scoreText.text = string.Format("{00:00}:{01:00}:{02:000}", minutes, seconds, miliseconds);
                }
                else
                {
                    endScreen.SetActive(true);
                    endText.text = "You Survived: " + string.Format("{00:00}:{01:00}:{2:000}", minutes, seconds, miliseconds);
                }
            }
            else
            {
                if (!dead)
                {
                    Shoot();
                    scoreText.text = points.ToString();
                }
                else
                {
                    endScreen.SetActive(true);
                    endText.text = "You Scored " + points.ToString() + " Points";
                }                
            }
            
        }

        void Shoot()
        {
            gunPivot.eulerAngles = new Vector3(gunPivot.rotation.x, gunPivot.rotation.y, 
                -Mathf.Atan2(VirtualJoystick.GetAxis("Horizontal", 1), VirtualJoystick.GetAxis("Vertical", 1)) * Mathf.Rad2Deg);

            if (gunTimer < 0.5f)
            {
                gunTimer += Time.deltaTime;
            }
            else
            {
                gunTimer = 0;

                if(VirtualJoystick.GetAxis(1) != Vector2.zero)
                {
                    Instantiate(playerBullet, gunPivot.position, gunPivot.rotation);
                }
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

