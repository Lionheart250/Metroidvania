using UnityEngine;
using System.Collections;

namespace Pathfinding {
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
    public class BatCasterAiDestinationSetter : VersionedMonoBehaviour {
        public Transform target; // The object that the AI should move to
        IAstarAI ai;

        // Additional variables for circular movement
        public float circleRadius = 5f;
        public float circleSpeed = 2f;
        private float angle = 0f;

        // Variables for erratic movement
        public float erraticSpeed = 1f;
        public float erraticChangeInterval = 2f;
        private float erraticTimer = 0f;

        void OnEnable() {
            ai = GetComponent<IAstarAI>();
            if (ai != null) ai.onSearchPath += Update;
            SetPlayerAsTarget(); // Add this line to set the player as the initial target
        }

        void OnDisable() {
            if (ai != null) ai.onSearchPath -= Update;
        }

        void Update() {
            if (target != null && ai != null) {
                // Circular movement
                angle += circleSpeed * Time.deltaTime;
                Vector3 circularOffset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * circleRadius;

                // Erratic movement
                erraticTimer += Time.deltaTime;
                if (erraticTimer >= erraticChangeInterval) {
                    erraticTimer = 0f;
                    Vector3 randomOffset = Random.onUnitSphere * circleRadius * 0.5f; // Adjust the multiplier as needed

                    // Combine circular and erratic movement
                    ai.destination = target.position + circularOffset + randomOffset;
                }

                // Set the destination to a position above the player in a circular and erratic motion
                ai.destination = target.position + circularOffset;
            }
        }

        // Method to set the player as the target
        void SetPlayerAsTarget() {
            // Find the player using a tag (adjust as needed)
            target = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (target == null) {
                Debug.LogError("Player not found. Make sure the player GameObject has the correct tag.");
            }
        }
    }
}
