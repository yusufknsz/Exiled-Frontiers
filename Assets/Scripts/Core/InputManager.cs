using UnityEngine;

public class InputManager : MonoBehaviour
{
    public SettlerAI selectedSettler; 
    private Camera mainCamera;

    void Start() => mainCamera = Camera.main;

void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        Vector3 mousePos = Input.mousePosition;
        
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0; 

        if (selectedSettler != null)
        {
            selectedSettler.MoveTo(worldPos);
        }
    }
}
} 
