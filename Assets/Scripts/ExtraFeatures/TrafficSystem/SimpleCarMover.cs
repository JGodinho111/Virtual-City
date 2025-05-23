using UnityEngine;

/// <summary>
/// Additional Feature Script which handles car movement across preset wayPoints
/// </summary>
public class SimpleCarMover : MonoBehaviour
{
    [SerializeField]
    private Transform[] wayPoints;
    private float carSpeed = 3f;
    private int currentWayPointIndex;

    // Update is called once per frame
    void Update()
    {
        if(wayPoints.Length != 0)
        {
            Transform currentWayPoint = wayPoints[currentWayPointIndex];
            Vector3 movementDirection = (currentWayPoint.position - transform.position).normalized;
            float distance = carSpeed * Time.deltaTime;

            if(movementDirection != Vector3.zero)
            {
                Quaternion carRotation = Quaternion.LookRotation(movementDirection);
                
                if(transform.rotation != carRotation)
                    transform.rotation = Quaternion.Lerp(transform.rotation, carRotation, 0.3f);
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWayPoint.position, distance);

            if (Vector3.Distance(transform.position, currentWayPoint.position) <= 0f)
            {
                currentWayPointIndex += 1;
                {
                    if (currentWayPointIndex >= wayPoints.Length)
                        currentWayPointIndex = 0;
                }
            }
        }
    }
}
