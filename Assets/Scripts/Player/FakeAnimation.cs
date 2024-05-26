using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdaGameJam.Core
{
    public class EnemyAI : MonoBehaviour
    {
        // AI states
        private enum State
        {
            Patrolling,
            Chasing,
            Resting
        }

        // Start and end points for patrol
        private float pointA, pointB;

        // Reference to the RectTransform component
        [SerializeField]
        private RectTransform _rect => GetComponent<RectTransform>();

        // Frame rate for movement
        [SerializeField]
        private float frame = 60f;

        // Time spent moving between points
        private float movementTime;

        // Minimum and maximum x positions for patrol
        private float xMin, xMax;

        // Player tracking variables
        [SerializeField]
        private float chaseRadius = 5f;
        private GameObject player;

        // Current state
        private State currentState;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            currentState = State.Patrolling;
            pointA = -_rect.sizeDelta.x / 2;
            pointB = _rect.sizeDelta.x / 2;
            xMin = pointA;
            xMax = pointB;
        }

        // Update is called once per frame
        void Update()
        {
            switch (currentState)
            {
                case State.Patrolling:
                    Patrol();
                    CheckForPlayer();
                    break;

                case State.Chasing:
                    ChasePlayer();
                    break;

                case State.Resting:
                    // Implement resting behavior if needed
                    break;
            }
        }

        // Method to handle the patrol movement
        private void Patrol()
        {
            movementTime += Time.deltaTime / frame;

            if (movementTime > 1f)
            {
                // Swap the patrol points
                (xMin, xMax) = (xMax, xMin);

                // Reset the movement time
                movementTime = 0f;
            }

            // Calculate the new position using linear interpolation
            float newX = Mathf.Lerp(xMin, xMax, movementTime);
            _rect.transform.localPosition = new Vector3(newX, _rect.transform.localPosition.y, _rect.transform.localPosition.z);
        }

        // Method to handle chasing the player
        private void ChasePlayer()
        {
            if (player == null) return;

            float step = frame * Time.deltaTime;
            Vector3 targetPosition = new Vector3(player.transform.position.x, _rect.transform.localPosition.y, _rect.transform.localPosition.z);
            _rect.transform.localPosition = Vector3.MoveTowards(_rect.transform.localPosition, targetPosition, step);

            if (Vector3.Distance(transform.position, player.transform.position) > chaseRadius)
            {
                currentState = State.Patrolling;
            }
        }

        // Check if the player is within the chase radius
        private void CheckForPlayer()
        {
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= chaseRadius)
            {
                currentState = State.Chasing;
            }
        }
    }
}
