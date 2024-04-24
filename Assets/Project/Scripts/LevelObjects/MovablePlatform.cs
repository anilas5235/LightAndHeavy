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
        [SerializeField] private float speed = 5;
        [SerializeField] private SaveTeleporter saveTeleporter;
       
        private Vector3 targetPoint;
        private int currentIndex;
        private Coroutine moveRoutine;
        private List<GameObject> children = new List<GameObject>();

        protected override void OnEnable()
        {
            base.OnEnable();
            saveTeleporter.enabled = false;
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
                if(gameObject.activeSelf)other.transform.parent = null;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (var child in children) child.transform.parent = null;
            
        }

        protected override void StateChanged(bool newState)
        {
            base.StateChanged(newState);
            if(moveRoutine != null)StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(PlatformMove(State));
        }
        private IEnumerator PlatformMove(bool rise)
        {
            targetPoint = rise ? worldPoints[Math.Clamp(currentIndex+1,0,worldPoints.Length-1)].position : 
                worldPoints[Math.Clamp(currentIndex-1,0,worldPoints.Length-1)].position;
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
                saveTeleporter.enabled = !rise && currentIndex < 1;
                yield return null;
            } while (!done);

            moveRoutine = null;
            saveTeleporter.enabled = false;
        }
    }
}