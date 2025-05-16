using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ResetToDefault : MonoBehaviour
{
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private string _targetControlScheme;


    public void ResetAllBindings()
    {
        foreach (InputActionMap map in _inputActions.actionMaps) 
        {
            map.RemoveAllBindingOverrides();
        }
    }

    public void ResetControlSchemeBinding()
    {
        foreach (InputActionMap map in _inputActions.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
    }
}
