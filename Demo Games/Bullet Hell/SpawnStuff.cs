using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terresquall
{
    public class SpawnStuff : MonoBehaviour
    {
        public float spawnXLength;
        public float spawnTime;
        public GameObject bullet;

        float timer;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            if(timer >= spawnTime)
            {
                float _spawnX = Random.Range(-spawnXLength, spawnXLength);

                Instantiate(bullet, new Vector2(_spawnX, transform.position.y), Quaternion.identity);

                timer = 0;

                spawnTime = Random.Range(0, spawnTime + 0.25f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position - new Vector3(spawnXLength, 0, 0), transform.position + new Vector3(spawnXLength, 0, 0));
        }
    }
}

