using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class MovablePlatform : BoolInteractable
    {
        [SerializeField] private Transform[] worldPoints;

        [SerializeField] private BoolInteractable[] triggers;
        [SerializeField] private float speed = 5;
       
        private Vector3 targetPoint;
        private int currentIndex;
        private Coroutine moveRoutine;
        private List<GameObject> children = new List<GameObject>();
     
        private void OnEnable()
        {
            foreach (var trigger in triggers)trigger.onStateChange.AddListener(CheckTriggers);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.attachedRigidbody)
            {
                children.Add(other.gameObject);
                other.transform.parent = transform;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.attachedRigidbody)
            {
                children.Remove(other.gameObject);
                other.transform.parent = null;
            }
        }

        private void OnDisable()
        {
            foreach (var child in children)
            {
                child.transform.parent = null;
            }
            foreach (var trigger in triggers)trigger.onStateChange.RemoveListener(CheckTriggers);
        }

        private void CheckTriggers(bool arg0)
        {
            State = triggers.Any(trigger => trigger.State);
            if(moveRoutine != null)StopCoroutine(moveRoutine);
            StartCoroutine(PlatformMove(State));
        }
        private IEnumerator PlatformMove(bool rise)
        {
            targetPoint = rise ? worldPoints[0].position : worldPoints[^1].position;
            var done=false;
            do
            {
                var vec = targetPoint - transform.position;
                if (vec.magnitude > .1f)
                {
                    transform.Translate(vec.normalized * (Time.deltaTime * speed));
                }
                else
                {
                    currentIndex += rise ? 1 : -1;
                    if (currentIndex < 0)
                    {
                        currentIndex = 0;
                        done = true;
                    }
                    else if (currentIndex >= worldPoints.Length)
                    {
                        currentIndex = worldPoints.Length - 1;
                        done = true;
                    }

                    targetPoint = worldPoints[currentIndex].position;
                }

                yield return null;
            } while (!done);

            moveRoutine = null;
        }
    }
}