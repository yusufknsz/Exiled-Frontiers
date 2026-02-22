using UnityEngine;

public class SettlerAI : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private ResourceNode currentTargetNode;

public void StartHarvesting(ResourceNode node)
{
    currentTargetNode = node;
    MoveTo(node.transform.position);
}

void Update()
{
    if (isMoving)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            
            // Eğer bir kaynağa gidiyorsak ve vardık ise toplamaya başla
            if (currentTargetNode != null)
            {
                Invoke("CompleteGathering", currentTargetNode.gatherTime);
            }
        }
    }
}

void CompleteGathering()
{
    if (currentTargetNode != null)
    {
        currentTargetNode.Gather();
        currentTargetNode = null; // Hedefi temizle
    }
}

    void Start()
    {
        // Başlangıçta karakterin olduğu yeri hedef belirle ki hemen hareket etmesin
        targetPosition = transform.position;
    }


    public void MoveTo(Vector3 destination)
    {
        targetPosition = destination;
        targetPosition.z = 0; // İzometrik düzlemde Z her zaman 0 olmalı
        isMoving = true;
    }
}