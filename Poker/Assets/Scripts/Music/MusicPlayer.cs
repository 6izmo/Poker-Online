using Utilities;
using UnityEngine;
using System.Collections.Generic;

namespace Menu
{
    public class MusicPlayer : PersistentSingleton<MusicPlayer>
    {
        [SerializeField] private List<AudioClip> _audioClips;
        private AudioSource _audioSource;
        private bool _canPlaying = true;
        private bool _isFocus = true;
        private int _currentClipId;

        protected override void Awake()
        {
            base.Awake();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _audioClips[0];   
            _audioSource.Play();
        }

        private void Update()
        {
            if (!_audioSource.isPlaying && _canPlaying && _isFocus)
                NextClip();
        }  

        public void Mute(bool turnOff)
        {
            if (turnOff && _audioSource.isPlaying)
            {
                _audioSource.Pause();
                _canPlaying = false;

			}
            else if (!turnOff && !_audioSource.isPlaying)
            {
                _audioSource.UnPause();
                _canPlaying = true;
			}
        }

        private void NextClip()
        {
            _currentClipId = _currentClipId == _audioClips.Count - 1 ? 0 : _currentClipId + 1;            
            _audioSource.clip = _audioClips[_currentClipId];
            _audioSource.Play();   
        }

        private void OnApplicationFocus(bool focus)
        {
			_isFocus = focus;
			if (_isFocus && _canPlaying && !_audioSource.isPlaying)            
				_audioSource.UnPause();
            else if(!_isFocus && _canPlaying)
				_audioSource.Pause();
		}
    }
}


