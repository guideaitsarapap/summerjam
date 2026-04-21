using UnityEngine;
using VInspector;

public class ChangeMaterials : MonoBehaviour
{

    [SerializeField] bool isHasDefaultMaterials = false;

    [Header("Material")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material getHitMaterial;

    private SpriteRenderer SR;

    private void Awake()
    {
        SR = GetComponent<SpriteRenderer>();
        if(!isHasDefaultMaterials)
        {
            defaultMaterial = SR.material;
        }

    }


    public void ChangeToGetHitMaterial()
    {
        SR.material = getHitMaterial;
    }

    public void ChangeToDefaultMaterial()
    {
        SR.material = defaultMaterial;
    }
}
