using System;
using Bones.Gameplay.Factories.Pools.Base;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Bones.Gameplay.Factories.Pools
{
	public sealed class AudioPool : BasePool<AudioInfo, AudioSource>
    {
        private readonly CompositeDisposable _subscriptions = new();
        private readonly Transform _parent;
        
        public AudioPool()
        {
            _parent = new GameObject("Audio Pool").transform;
        }

        protected override AudioSource Instantiate()
        {
            var instance = new GameObject("Free Audio")
            {
                transform =
                {
                    parent = _parent,
                },
            };
            var source = instance.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        protected override async void OnSpawned(AudioSource entity, AudioInfo param)
        {
            entity.pitch = param.Pitch;

            entity.clip = param.Clip;
            entity.volume = param.Volume;
            entity.name = param.Clip.name;
            
            entity.spatialize = param.Spatialize;
            entity.spatialBlend = param.SpatialBlend;
            
            entity.Play();
            
            await UniTask.Delay(TimeSpan.FromSeconds(param.Clip.length));
            Despawn(entity);
        }

        protected override void OnDespawned(AudioSource entity)
        {
			if (entity == null)
				return;
            entity.name = "Free Audio";
            entity.Stop();
        }
    }
}