using UnityEngine;
using System.Collections;

namespace Pathfinding {
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
    public class AIDestinationSetter : VersionedMonoBehaviour {
        public Transform target; // The object that the AI should move to
        IAstarAI ai;

        void OnEnable() {
            ai = GetComponent<IAstarAI>();
            if (ai != null) ai.onSearchPath += Update;
            SetPlayerAsTarget(); // Add this line to set the player as the initial target
        }

        void OnDisable() {
            if (ai != null) ai.onSearchPath -= Update;
        }

        void Update() {
            if (target != null && ai != null) ai.destination = target.position;
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
