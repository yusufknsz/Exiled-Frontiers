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
        // Eğer bina yerleştirme modundaysak, köylüye hareket emri gönderme
        if (BuildingManager.Instance != null && BuildingManager.Instance.IsPlacingBuilding())
        {
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0; 

        // Eğer fare ile tıkladığımız yerde bir "ResourceNode" varsa, hareketi o başlatmalı, InputManager değil.
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.GetComponent<ResourceNode>() != null)
        {
            return; // Kaynak tıklamasını devralma, bırak ResourceNode kendi yapsın
        }

        if (selectedSettler != null)
        {
            selectedSettler.MoveTo(worldPos);
        }
    }
}
} 
