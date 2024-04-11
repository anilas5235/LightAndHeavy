using UnityEngine;

namespace Project.Scripts.LevelObjects
{
    public class WeightButton : BoolInteractable
    {
        [Space,SerializeField,Range(.001f,5f)] private float buttonHeight = .3f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color inActiveColor= Color.white;
        [SerializeField] private Color activeColor=Color.white;

        private Transform _spriteTransform;
        private float _targetHeight;
        private float _startHeight;

        private void Awake()
        {
            _spriteTransform = spriteRenderer.transform;
            spriteRenderer.color = inActiveColor;
            _startHeight = _spriteTransform.localPosition.y;
        }

        private void Update()
        {
            if(Mathf.Abs(_spriteTransform.position.y - _targetHeight) < .001f) return;
            var pos = _spriteTransform.localPosition;
            pos.y = Mathf.Lerp(pos.y, _targetHeight, .1f);
            _spriteTransform.localPosition = pos; 
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                State = true;
                spriteRenderer.color = activeColor;
                _targetHeight = _startHeight -buttonHeight;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                State = false;
                spriteRenderer.color = inActiveColor;
                _targetHeight = _startHeight;
            }
        }
    }
}