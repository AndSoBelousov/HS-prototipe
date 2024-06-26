using UnityEngine;

public class ChangeTransparency : MonoBehaviour
{
    
    void Start()
    {
        

            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                Debug.Log("номер" + renderers);

                Debug.Log("рендер - " + renderer);
                if (renderer.material.HasProperty("_Color"))
                {
                    Material material = renderer.material;

                  
                    Color color = material.color;
                    color.a = 0.5f; 
                    material.color = color;
                }

            }

            GetComponent<MeshRenderer>().enabled = false;
            
    }

    
}
