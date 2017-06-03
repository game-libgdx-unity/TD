using System.Collections;
using UnitedSolution;using UnityEngine;

namespace BasicAI
{
    public class PlayerHealth : MonoBehaviour
    {
        public float health = 100.0f;
        public bool canRespawn = true;
        public float respawnDelay = 3f;


        private void Update()
        {
            if (health <= 0)
            {
                // Manage your player's death logic here
                // A small example here, you note that I changed the layer of the target so the AI will ingnore it when it is dead.

                health = 0;
                gameObject.layer = 9;
                if (canRespawn) StartCoroutine("Respawn");
            }
        }

        private void SubtractHealth(float healthAmount)
        {
            health -= healthAmount;
        }

        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(respawnDelay);
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}