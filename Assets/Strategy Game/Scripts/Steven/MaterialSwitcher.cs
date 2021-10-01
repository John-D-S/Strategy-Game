using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles switching the materials of the game object the script is on. Can be called from playercontroler.
/// </summary>
public class MaterialSwitcher : MonoBehaviour
{
    [Header("Materials"), Tooltip("Set the materials here for the different states.")]
    [SerializeField] private Material enableMaterial;
    [SerializeField] private Material disableMaterial;
    [SerializeField] private Material defaultMaterial;

    private MeshRenderer meshRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }
    
    /// <summary>
    /// Switches the GameObjects material to the set enable material.
    /// </summary>
    public void SwitchToEnableMaterial()
    {
        meshRenderer.material = enableMaterial;
    }
    
    /// <summary>
    /// Switches the GameObjects material to the set disable material.
    /// </summary>
    public void SwitchToDisbleMaterial()
    {
        meshRenderer.material = disableMaterial;
    }
    
    /// <summary>
    /// Switches the GameObjects material to the default material.
    /// </summary>
    public void SwitchToDefaultMaterial()
    {
        meshRenderer.material = defaultMaterial;
    }

    }
