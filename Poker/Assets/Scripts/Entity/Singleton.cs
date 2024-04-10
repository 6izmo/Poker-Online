using Photon.Pun;
using UnityEngine;

namespace Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
			if (Instance != null && Instance != this)
				Destroy(gameObject);
			else
				Instance = this as T;
		}
    }

	public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour 
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }
    }

	public abstract class SingletonPun<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
	{
		public static T Instance { get; private set; }

		protected virtual void Awake()
		{
			if (Instance != null && Instance != this)
				Destroy(gameObject);
			else
				Instance = this as T;
		}
	}

	public abstract class PersistentSingletonPun<T> : SingletonPun<T> where T : MonoBehaviourPunCallbacks
	{
		protected override  void Awake()
		{
			base.Awake();
			DontDestroyOnLoad(this);
		}
	}
}
