using System.Collections;
using AttributesLibrary.ReadOnly;
using Project.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Project.Scripts.LevelObjects
{
    public class FinishDoor : MonoBehaviour, IHaveElementType
    {
        [SerializeField] private ElementType elementType;
        [SerializeField] private float doorHeight;
        [SerializeField] private SpriteRenderer doorSprite;
        [SerializeField,Range(.001f,5)] private float timeToOpen  = 1;

        #pragma warning disable 414 
        [Header("Info"),SerializeField,ReadOnly] private bool detectedPlayer;
        #pragma warning restore 414
        [SerializeField,ReadOnly] private bool open;
        private Transform _doorTransform;
        private float _startHeight;
        private Coroutine _doorAction;

        public bool Open => open;
        [Space] public UnityEvent onDoorOpened;
        private void Awake()
        {
            _doorTransform = doorSprite.transform;
            _startHeight = _doorTransform.localPosition.y;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            if(other.gameObject.GetComponent<IHaveElementType>()?.GetElementType() == elementType) DoorOpening();
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            detectedPlayer = false;
            if(other.gameObject.GetComponent<IHaveElementType>()?.GetElementType() == elementType) DoorClosing();
        }
        private void DoorOpening()
        {
            if(!gameObject.activeInHierarchy) return;
            detectedPlayer = true;
            if (_doorAction != null) StopCoroutine(_doorAction);
            _doorAction = StartCoroutine(OpenDoorAction(timeToOpen));
        }
        private void DoorClosing()
        {
            if(!gameObject.activeInHierarchy) return;
            detectedPlayer = false;
            if (_doorAction != null) StopCoroutine(_doorAction);
            _doorAction = StartCoroutine(CloseDoorAction(timeToOpen));
        }

        private IEnumerator OpenDoorAction(float time)
        {
            var height = doorHeight+_startHeight;
            var startHeight = _doorTransform.localPosition.y;
            var usedTime = 0f;
            while (usedTime < time)
            {
                usedTime += Time.fixedDeltaTime;
                var pos = _doorTransform.localPosition;
                pos.y = Mathf.Lerp(startHeight, height, usedTime/time);
                _doorTransform.localPosition = pos;
                yield return new WaitForFixedUpdate();
            }
            open = true;
            _doorAction = null;
            onDoorOpened?.Invoke();
        }
        
        private IEnumerator CloseDoorAction(float time)
        {
            open = false;
            var height = _doorTransform.localPosition.y;
            float usedTime = 0;
            while (usedTime < time)
            {
                usedTime += Time.fixedDeltaTime;
                var pos = _doorTransform.localPosition;
                pos.y = Mathf.Lerp( height,_startHeight, usedTime/time);
                _doorTransform.localPosition = pos;
                yield return new WaitForFixedUpdate();
            }
            _doorAction = null;
        }

        public ElementType GetElementType()
        {
            return elementType;
        }
    }
}