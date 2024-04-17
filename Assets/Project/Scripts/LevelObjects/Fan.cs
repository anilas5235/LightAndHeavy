using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.LevelObjects;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private AreaEffector2D areaEffector2D;
    [SerializeField] private Animator animator;

    [SerializeField] private BoolInteractable[] activator;
    private static readonly int On = Animator.StringToHash("On");

    private void OnEnable()
    {
        foreach (BoolInteractable boolInteractable in activator)
        {
            boolInteractable.onStateChange.AddListener(_stateChange);
        }
    }

    private void OnDisable()
    {
        foreach (BoolInteractable boolInteractable in activator)
        {
            boolInteractable.onStateChange.RemoveListener(_stateChange);
        }
    }

    private void Awake()
    {
        _stateChange(false);
    }

    private void _stateChange(bool arg0)
    {
        boxCollider2D.enabled = arg0;
        areaEffector2D.enabled = arg0;
        animator.SetBool(On, arg0);
    }

    

    

    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
