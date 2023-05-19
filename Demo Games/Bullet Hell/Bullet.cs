using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terresquall
{
    public class Bullet : MonoBehaviour
    {
        public float speed;
        public float lifetime;
        public bool playerBullet;
        public bool asteroid;

        Player player;
        Vector3 dir;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, lifetime);
            dir = playerBullet ? transform.up : Vector3.down;

            player = FindObjectOfType<Player>();
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += speed * Time.deltaTime * dir;
        }

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if(_other.gameObject.CompareTag("Player Bullet") && asteroid)
            {
                player.points++;
                Destroy(gameObject);
            }

            if(_other.gameObject.CompareTag("Bullet") && playerBullet)
            {
                Destroy(gameObject);
            }
        }
    }
}

