using System;
using ControllerPlugin.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.Player
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private AdvancedCharacterController2D advancedCharacterController2D;
        private Animator animator;
        private AudioSource _audioSource;
        [SerializeField] private AudioClip[] dashSounds;
        [SerializeField] private AudioClip[] jumpSounds;
        private static readonly int Run = Animator.StringToHash("Run");

        private void Awake()
        {
            animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            advancedCharacterController2D.onStateChanged.AddListener(StateChanged);
        }

        private void OnDisable()
        {
            advancedCharacterController2D.onStateChanged.RemoveListener(StateChanged);
        }

        private void StateChanged(CharacterActionState old, CharacterActionState newState)
        {
            animator.SetBool(Run,newState == CharacterActionState.Walking);
            if (newState == CharacterActionState.Dashing && gameObject.layer == LayerMask.NameToLayer("Heavy"))
            {
                int rndm = Random.Range(0, dashSounds.Length);
                _audioSource.clip = dashSounds[rndm];
                _audioSource.Play();
            }

            if (newState == CharacterActionState.Jumping && gameObject.layer == LayerMask.NameToLayer("Light"))
            {
                int rndm = Random.Range(0, jumpSounds.Length);
                _audioSource.clip = jumpSounds[rndm];
                _audioSource.Play();
            }
        }
    }
}