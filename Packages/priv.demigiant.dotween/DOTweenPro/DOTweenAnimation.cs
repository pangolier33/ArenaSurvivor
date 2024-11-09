// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/03/12 15:55

using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using UnityEngine;
#if true // UI_MARKER
using UnityEngine.UI;
#endif
#if true // TEXTMESHPRO_MARKER
using TMPro;
#endif

#pragma warning disable 1591
namespace DG.Tweening
{
    /// <summary>
    /// Attach this to a GameObject to create a tween
    /// </summary>
    [AddComponentMenu("DOTween/DOTween Animation")]
    public class DOTweenAnimation : ABSAnimationComponent
    {
        public enum AnimationType
        {
            None,
            Move, LocalMove,
            Rotate, LocalRotate,
            Scale,
            Color, Fade,
            Text,
            PunchPosition, PunchRotation, PunchScale,
            ShakePosition, ShakeRotation, ShakeScale,
            CameraAspect, CameraBackgroundColor, CameraFieldOfView, CameraOrthoSize, CameraPixelRect, CameraRect,
            UIWidthHeight
        }

        public enum TargetType
        {
            Unset,

            Camera,
            CanvasGroup,
            Image,
            Light,
            RectTransform,
            Renderer, SpriteRenderer,
            Rigidbody, Rigidbody2D,
            Text,
            Transform,

            tk2dBaseSprite,
            tk2dTextMesh,

            TextMeshPro,
            TextMeshProUGUI
        }

        #region EVENTS - EDITOR-ONLY

        /// <summary>Used internally by the editor</summary>
        public static event Action<DOTweenAnimation> OnReset;
        static void Dispatch_OnReset(DOTweenAnimation anim) { if (OnReset != null) OnReset(anim); }

        #endregion

        public bool targetIsSelf = true; // If FALSE allows to set the target manually
        public GameObject targetGO = null; // Used in case targetIsSelf is FALSE
        // If FALSE always uses the GO containing this DOTweenAnimation (and not the one containing the target) as DOTween's SetTarget target
        public bool tweenTargetIsTargetGO = true;

        public float delay;
        public float duration = 1;
        public Ease easeType = Ease.OutQuad;
        public AnimationCurve easeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        public LoopType loopType = LoopType.Restart;
        public int loops = 1;
        public string id = "";
        public bool isRelative;
        public bool isFrom;
        public bool isIndependentUpdate = false;
        public bool autoKill = true;
        public bool autoGenerate = true; // If TRUE automatically creates the tween at startup

        public bool isActive = true;
        public bool isValid;
        public Component target;
        public AnimationType animationType;
        public TargetType targetType;
        public TargetType forcedTargetType; // Used when choosing between multiple targets
        public bool autoPlay = true;
        public bool useTargetAsV3;

        public float endValueFloat;
        public Vector3 endValueV3;
        public Vector2 endValueV2;
        public Color endValueColor = new Color(1, 1, 1, 1);
        public string endValueString = "";
        public Rect endValueRect = new Rect(0, 0, 0, 0);
        public Transform endValueTransform;

        public bool optionalBool0, optionalBool1;
        public float optionalFloat0;
        public int optionalInt0;
        public RotateMode optionalRotationMode = RotateMode.Fast;
        public ScrambleMode optionalScrambleMode = ScrambleMode.None;
        public string optionalString;

        bool _tweenAutoGenerationCalled; // TRUE after the tweens have been autoGenerated
        int _playCount = -1; // Used when calling DOPlayNext

        #region Unity Methods

        void Awake()
        {
            if (!isActive || !autoGenerate) return;

            if (animationType != AnimationType.Move || !useTargetAsV3) {
                // Don't create tweens if we're using a RectTransform as a Move target,
                // because that will work only inside Start
                CreateTween(false, autoPlay);
                _tweenAutoGenerationCalled = true;
            }
        }

        void Start()
        {
            if (_tweenAutoGenerationCalled || !isActive || !autoGenerate) return;

            CreateTween(false, autoPlay);
            _tweenAutoGenerationCalled = true;
        }

        void Reset()
        {
            Dispatch_OnReset(this);
        }

        void OnDestroy()
        {
            if (tween != null && tween.active) tween.Kill();
            tween = null;
        }

        /// <summary>
        /// Creates/recreates the tween without playing it, but first rewinding and killing the existing one if present.
        /// </summary>
        public void RewindThenRecreateTween()
        {
            if (tween != null && tween.active) tween.Rewind();
            CreateTween(true, false);
        }
        /// <summary>
        /// Creates/recreates the tween and plays it, first rewinding and killing the existing one if present.
        /// </summary>
        public void RewindThenRecreateTweenAndPlay()
        {
            if (tween != null && tween.active) tween.Rewind();
            CreateTween(true, true);
        }
        /// <summary>
        /// Creates/recreates the tween from its target's current value without playing it, but first killing the existing one if present.
        /// </summary>
        public void RecreateTween()
        { CreateTween(true, false); }
        /// <summary>
        /// Creates/recreates the tween from its target's current value and plays it, first killing the existing one if present.
        /// </summary>
        public void RecreateTweenAndPlay()
        { CreateTween(true, true); }
        // Used also by DOTweenAnimationInspector when applying runtime changes and restarting
        /// <summary>
        /// Creates the tween manually (called automatically if AutoGenerate is set in the Inspector)
        /// from its target's current value.
        /// </summary>
        /// <param name="regenerateIfExists">If TRUE and an existing tween was already created (and not killed), kills it and recreates it with the current
        /// parameters. Otherwise, if a tween already exists, does nothing.</param>
        /// <param name="andPlay">If TRUE also plays the tween, otherwise only creates it</param>
        public void CreateTween(bool regenerateIfExists = false, bool andPlay = true)
        {
            if (!isValid) {
                if (regenerateIfExists) { // Called manually: warn users
                    Debug.LogWarning(string.Format("{0} :: This DOTweenAnimation isn't valid and its tween won't be created", this.gameObject.name), this.gameObject);
                }
                return;
            }
            if (tween != null) {
                if (tween.active) {
                    if (regenerateIfExists) tween.Kill();
                    else return;
                }
                tween = null;
            }

//            if (target == null) {
//                Debug.LogWarning(string.Format("{0} :: This DOTweenAnimation's target is NULL, because the animation was created with a DOTween Pro version older than 0.9.255. To fix this, exit Play mode then simply select this object, and it will update automatically", this.gameObject.name), this.gameObject);
//                return;
//            }

            GameObject tweenGO = GetTweenGO();
            if (target == null || tweenGO == null) {
                if (targetIsSelf && target == null) {
                    // Old error caused during upgrade from DOTween Pro 0.9.255
                    Debug.LogWarning(string.Format("{0} :: This DOTweenAnimation's target is NULL, because the animation was created with a DOTween Pro version older than 0.9.255. To fix this, exit Play mode then simply select this object, and it will update automatically", this.gameObject.name), this.gameObject);
                } else {
                    // Missing non-self target
                    Debug.LogWarning(string.Format("{0} :: This DOTweenAnimation's target/GameObject is unset: the tween will not be created.", this.gameObject.name), this.gameObject);
                }
                return;
            }

            if (forcedTargetType != TargetType.Unset) targetType = forcedTargetType;
            if (targetType == TargetType.Unset) {
                // Legacy DOTweenAnimation (made with a version older than 0.9.450) without stored targetType > assign it now
                targetType = TypeToDOTargetType(target.GetType());
            }

            switch (animationType) {
            case AnimationType.None:
                break;
            case AnimationType.Move:
                if (useTargetAsV3) {
                    isRelative = false;
                    if (endValueTransform == null) {
                        Debug.LogWarning(string.Format("{0} :: This tween's TO target is NULL, a Vector3 of (0,0,0) will be used instead", this.gameObject.name), this.gameObject);
                        endValueV3 = Vector3.zero;
                    } else {
#if true // UI_MARKER
                        if (targetType == TargetType.RectTransform) {
                            RectTransform endValueT = endValueTransform as RectTransform;
                            if (endValueT == null) {
                                Debug.LogWarning(string.Format("{0} :: This tween's TO target should be a RectTransform, a Vector3 of (0,0,0) will be used instead", this.gameObject.name), this.gameObject);
                                endValueV3 = Vector3.zero;
                            } else {
                                RectTransform rTarget = target as RectTransform;
                                if (rTarget == null) {
                                    Debug.LogWarning(string.Format("{0} :: This tween's target and TO target are not of the same type. Please reassign the values", this.gameObject.name), this.gameObject);
                                } else {
                                    // Problem: doesn't work inside Awake (ararargh!)
                                    endValueV3 = DOTweenModuleUI.Utils.SwitchToRectTransform(endValueT, rTarget);
                                }
                            }
                        } else
#endif
                            endValueV3 = endValueTransform.position;
                    }
                }
                switch (targetType) {
                case TargetType.Transform:
                    tween = ((Transform)target).DOMove(endValueV3, duration, optionalBool0);
                    break;
                case TargetType.RectTransform:
#if true // UI_MARKER
                    tween = ((RectTransform)target).DOAnchorPos3D(endValueV3, duration, optionalBool0);
#else
                    tween = ((Transform)target).DOMove(endValueV3, duration, optionalBool0);
#endif
                    break;
                case TargetType.Rigidbody:
#if PHYSICS_MARKER
                    tween = ((Rigidbody)target).DOMove(endValueV3, duration, optionalBool0);
#else
                    tween = ((Transform)target).DOMove(endValueV3, duration, optionalBool0);
#endif
                    break;
                case TargetType.Rigidbody2D:
#if true // PHYSICS2D_MARKER
                    tween = ((Rigidbody2D)target).DOMove(endValueV3, duration, optionalBool0);
#else
                    tween = ((Transform)target).DOMove(endValueV3, duration, optionalBool0);
#endif
                    break;
                }
                break;
            case AnimationType.LocalMove:
                tween = tweenGO.transform.DOLocalMove(endValueV3, duration, optionalBool0);
                break;
            case AnimationType.Rotate:
                switch (targetType) {
                case TargetType.Transform:
                    tween = ((Transform)target).DORotate(endValueV3, duration, optionalRotationMode);
                    break;
                case TargetType.Rigidbody:
#if PHYSICS_MARKER
                    tween = ((Rigidbody)target).DORotate(endValueV3, duration, optionalRotationMode);
#else
                    tween = ((Transform)target).DORotate(endValueV3, duration, optionalRotationMode);
#endif
                    break;
                case TargetType.Rigidbody2D:
#if true // PHYSICS2D_MARKER
                    tween = ((Rigidbody2D)target).DORotate(endValueFloat, duration);
#else
                    tween = ((Transform)target).DORotate(endValueV3, duration, optionalRotationMode);
#endif
                    break;
                }
                break;
            case AnimationType.LocalRotate:
                tween = tweenGO.transform.DOLocalRotate(endValueV3, duration, optionalRotationMode);
                break;
            case AnimationType.Scale:
                switch (targetType) {
#if false // TK2D_MARKER
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)target).DOScale(optionalBool0 ? new Vector3(endValueFloat, endValueFloat, endValueFloat) : endValueV3, duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)target).DOScale(optionalBool0 ? new Vector3(endValueFloat, endValueFloat, endValueFloat) : endValueV3, duration);
                    break;
#endif
                default:
                    tween = tweenGO.transform.DOScale(optionalBool0 ? new Vector3(endValueFloat, endValueFloat, endValueFloat) : endValueV3, duration);
                    break;
                }
                break;
#if true // UI_MARKER
            case AnimationType.UIWidthHeight:
                tween = ((RectTransform)target).DOSizeDelta(optionalBool0 ? new Vector2(endValueFloat, endValueFloat) : endValueV2, duration);
                break;
#endif
            case AnimationType.Color:
                isRelative = false;
                switch (targetType) {
                case TargetType.Renderer:
                    tween = ((Renderer)target).material.DOColor(endValueColor, duration);
                    break;
                case TargetType.Light:
                    tween = ((Light)target).DOColor(endValueColor, duration);
                    break;
#if true // SPRITE_MARKER
                case TargetType.SpriteRenderer:
                    tween = ((SpriteRenderer)target).DOColor(endValueColor, duration);
                    break;
#endif
#if true // UI_MARKER
                case TargetType.Image:
                    tween = ((Graphic)target).DOColor(endValueColor, duration);
                    break;
                case TargetType.Text:
                    tween = ((Text)target).DOColor(endValueColor, duration);
                    break;
#endif
#if false // TK2D_MARKER
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)target).DOColor(endValueColor, duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)target).DOColor(endValueColor, duration);
                    break;
#endif
#if true // TEXTMESHPRO_MARKER
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)target).DOColor(endValueColor, duration);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)target).DOColor(endValueColor, duration);
                    break;
#endif
                }
                break;
            case AnimationType.Fade:
                isRelative = false;
                switch (targetType) {
                case TargetType.Renderer:
                    tween = ((Renderer)target).material.DOFade(endValueFloat, duration);
                    break;
                case TargetType.Light:
                    tween = ((Light)target).DOIntensity(endValueFloat, duration);
                    break;
#if true // SPRITE_MARKER
                case TargetType.SpriteRenderer:
                    tween = ((SpriteRenderer)target).DOFade(endValueFloat, duration);
                    break;
#endif
#if true // UI_MARKER
                case TargetType.Image:
                    tween = ((Graphic)target).DOFade(endValueFloat, duration);
                    break;
                case TargetType.Text:
                    tween = ((Text)target).DOFade(endValueFloat, duration);
                    break;
                case TargetType.CanvasGroup:
                    tween = ((CanvasGroup)target).DOFade(endValueFloat, duration);
                    break;
#endif
#if false // TK2D_MARKER
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)target).DOFade(endValueFloat, duration);
                    break;
                case TargetType.tk2dBaseSprite:
                    tween = ((tk2dBaseSprite)target).DOFade(endValueFloat, duration);
                    break;
#endif
#if true // TEXTMESHPRO_MARKER
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)target).DOFade(endValueFloat, duration);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)target).DOFade(endValueFloat, duration);
                    break;
#endif
                }
                break;
            case AnimationType.Text:
#if true // UI_MARKER
                switch (targetType) {
                case TargetType.Text:
                    tween = ((Text)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
                    break;
                }
#endif
#if false // TK2D_MARKER
                switch (targetType) {
                case TargetType.tk2dTextMesh:
                    tween = ((tk2dTextMesh)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
                    break;
                }
#endif
#if true // TEXTMESHPRO_MARKER
                switch (targetType) {
                case TargetType.TextMeshProUGUI:
                    tween = ((TextMeshProUGUI)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
                    break;
                case TargetType.TextMeshPro:
                    tween = ((TextMeshPro)target).DOText(endValueString, duration, optionalBool0, optionalScrambleMode, optionalString);
                    break;
                }
#endif
                break;
            case AnimationType.PunchPosition:
                switch (targetType) {
                case TargetType.Transform:
                    tween = ((Transform)target).DOPunchPosition(endValueV3, duration, optionalInt0, optionalFloat0, optionalBool0);
                    break;
#if true // UI_MARKER
                case TargetType.RectTransform:
                    tween = ((RectTransform)target).DOPunchAnchorPos(endValueV3, duration, optionalInt0, optionalFloat0, optionalBool0);
                    break;
#endif
                }
                break;
            case AnimationType.PunchScale:
                tween = tweenGO.transform.DOPunchScale(endValueV3, duration, optionalInt0, optionalFloat0);
                break;
            case AnimationType.PunchRotation:
                tween = tweenGO.transform.DOPunchRotation(endValueV3, duration, optionalInt0, optionalFloat0);
                break;
            case AnimationType.ShakePosition:
                switch (targetType) {
                case TargetType.Transform:
                    tween = ((Transform)target).DOShakePosition(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool0, optionalBool1);
                    break;
#if true // UI_MARKER
                case TargetType.RectTransform:
                    tween = ((RectTransform)target).DOShakeAnchorPos(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool0, optionalBool1);
                    break;
#endif
                }
                break;
            case AnimationType.ShakeScale:
                tween = tweenGO.transform.DOShakeScale(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool1);
                break;
            case AnimationType.ShakeRotation:
                tween = tweenGO.transform.DOShakeRotation(duration, endValueV3, optionalInt0, optionalFloat0, optionalBool1);
                break;
            case AnimationType.CameraAspect:
                tween = ((Camera)target).DOAspect(endValueFloat, duration);
                break;
            case AnimationType.CameraBackgroundColor:
                tween = ((Camera)target).DOColor(endValueColor, duration);
                break;
            case AnimationType.CameraFieldOfView:
                tween = ((Camera)target).DOFieldOfView(endValueFloat, duration);
                break;
            case AnimationType.CameraOrthoSize:
                tween = ((Camera)target).DOOrthoSize(endValueFloat, duration);
                break;
            case AnimationType.CameraPixelRect:
                tween = ((Camera)target).DOPixelRect(endValueRect, duration);
                break;
            case AnimationType.CameraRect:
                tween = ((Camera)target).DORect(endValueRect, duration);
                break;
            }

            if (tween == null) return;

            // Created

            if (isFrom) {
                ((Tweener)tween).From(isRelative);
            } else {
                tween.SetRelative(isRelative);
            }
            GameObject setTarget = GetTweenTarget();
            tween.SetTarget(setTarget).SetDelay(delay).SetLoops(loops, loopType).SetAutoKill(autoKill)
                .OnKill(()=> tween = null);
            if (isSpeedBased) tween.SetSpeedBased();
            if (easeType == Ease.INTERNAL_Custom) tween.SetEase(easeCurve);
            else tween.SetEase(easeType);
            if (!string.IsNullOrEmpty(id)) tween.SetId(id);
            tween.SetUpdate(isIndependentUpdate);

            if (hasOnStart) {
                if (onStart != null) tween.OnStart(onStart.Invoke);
            } else onStart = null;
            if (hasOnPlay) {
                if (onPlay != null) tween.OnPlay(onPlay.Invoke);
            } else onPlay = null;
            if (hasOnUpdate) {
                if (onUpdate != null) tween.OnUpdate(onUpdate.Invoke);
            } else onUpdate = null;
            if (hasOnStepComplete) {
                if (onStepComplete != null) tween.OnStepComplete(onStepComplete.Invoke);
            } else onStepComplete = null;
            if (hasOnComplete) {
                if (onComplete != null) tween.OnComplete(onComplete.Invoke);
            } else onComplete = null;
            if (hasOnRewind) {
                if (onRewind != null) tween.OnRewind(onRewind.Invoke);
            } else onRewind = null;

            if (andPlay) tween.Play();
            else tween.Pause();

            if (hasOnTweenCreated && onTweenCreated != null) onTweenCreated.Invoke();
        }

        #endregion

        #region Public Methods

        #region Special

        /// <summary>
        /// Returns the tweens (if generated and not killed) created by all DOTweenAnimations on this gameObject,
        /// in the same order as they appear in the Inspector (top to bottom).<para/>
        /// Note that a tween is generated inside the Awake call (except RectTransform tweens which are generated inside Start),
        /// so this method won't return them before that
        /// </summary>
        public List<Tween> GetTweens()
        {
            List<Tween> result = new List<Tween>();
            DOTweenAnimation[] anims = this.GetComponents<DOTweenAnimation>();
            foreach (DOTweenAnimation anim in anims) {
                if (anim.tween != null && anim.tween.active) result.Add(anim.tween);
            }
            return result;
        }

        /// <summary>
        /// Sets the animation target (which must be of the same type of the one set in the Inspector).
        /// This is useful if you want to change it BEFORE this <see cref="DOTweenAnimation"/>
        /// creates a tween, while after that it won't have any effect.<para/>
        /// Consider that a <see cref="DOTweenAnimation"/> creates its tween inside its Awake (except for special tweens),
        /// so you will need to sure your code runs before this object's Awake (via ScriptExecutionOrder or enabling/disabling methods)
        /// </summary>
        /// <param name="tweenTarget">
        /// New target for the animation (must be of the same type of the previous one)</param>
        /// <param name="useTweenTargetGameObjectForGroupOperations">If TRUE also uses tweenTarget's gameObject when settings the target-ID of the tween
        /// (which is used with DOPlay/DORestart/etc to apply the same operation on all tweens that have the same target-id).<para/>
        /// You should usually leave this to TRUE if you change the target.
        /// </param>
        public void SetAnimationTarget(Component tweenTarget, bool useTweenTargetGameObjectForGroupOperations = true)
        {
            TargetType newTargetType = TypeToDOTargetType(target.GetType());
            if (newTargetType != targetType) {
                Debug.LogError("DOTweenAnimation ► SetAnimationTarget: the new target is of a different type from the one set in the Inspector");
                return;
            }
            target = tweenTarget;
            targetGO = target.gameObject;
            tweenTargetIsTargetGO = useTweenTargetGameObjectForGroupOperations;
        }

        #endregion

        /// <summary>
        /// Plays all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        public override void DOPlay()
        {
            DOTween.Play(GetTweenTarget());
        }

        /// <summary>
        /// Plays backwards all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        public override void DOPlayBackwards()
        {
            DOTween.PlayBackwards(GetTweenTarget());
        }

        /// <summary>
        /// Plays foward all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        public override void DOPlayForward()
        {
            DOTween.PlayForward(GetTweenTarget());
        }

        /// <summary>
        /// Pauses all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        public override void DOPause()
        {
            DOTween.Pause(GetTweenTarget());
        }

        /// <summary>
        /// Pauses/unpauses (depending on the current state) all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        public override void DOTogglePause()
        {
            DOTween.TogglePause(GetTweenTarget());
        }

        /// <summary>
        /// Rewinds all tweens created by this animation in the correct order
        /// </summary>
        public override void DORewind()
        {
        	_playCount = -1;
            // Rewind using Components order (in case there are multiple animations on the same property)
            DOTweenAnimation[] anims = this.gameObject.GetComponents<DOTweenAnimation>();
            for (int i = anims.Length - 1; i > -1; --i) {
                Tween t = anims[i].tween;
                if (t != null && t.IsInitialized()) anims[i].tween.Rewind();
            }
            // DOTween.Rewind(GetTweenTarget());
        }

        /// <summary>
        /// Restarts all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        public override void DORestart()
        { DORestart(false); }
        /// <summary>
        /// Restarts all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        /// <param name="fromHere">If TRUE, re-evaluates the tween's start and end values from its current position.
        /// Set it to TRUE when spawning the same DOTweenAnimation in different positions (like when using a pooling system)</param>
        public override void DORestart(bool fromHere)
        {
        	_playCount = -1;
            if (tween == null) {
                if (Debugger.logPriority > 1) Debugger.LogNullTween(tween); return;
            }
            if (fromHere && isRelative) ReEvaluateRelativeTween();
            DOTween.Restart(GetTweenTarget());
        }

        /// <summary>
        /// Completes all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        public override void DOComplete()
        {
            DOTween.Complete(GetTweenTarget());
        }

        /// <summary>
        /// Kills all tweens whose target-id is the same as the one set by this animation
        /// </summary>
        public override void DOKill()
        {
            DOTween.Kill(GetTweenTarget());
            tween = null;
        }

        #region Specifics

        /// <summary>
        /// Plays all tweens with the given ID and whose target-id is the same as the one set by this animation
        /// </summary>
        public void DOPlayById(string id)
        {
            DOTween.Play(GetTweenTarget(), id);
        }
        /// <summary>
        /// Plays all tweens with the given ID (regardless of their target gameObject)
        /// </summary>
        public void DOPlayAllById(string id)
        {
            DOTween.Play(id);
        }

        /// <summary>
        /// Pauses all tweens that with the given ID (regardless of their target gameObject)
        /// </summary>
        public void DOPauseAllById(string id)
        {
            DOTween.Pause(id);
        }

        /// <summary>
        /// Plays backwards all tweens with the given ID and whose target-id is the same as the one set by this animation
        /// </summary>
        public void DOPlayBackwardsById(string id)
        {
            DOTween.PlayBackwards(GetTweenTarget(), id);
        }
        /// <summary>
        /// Plays backwards all tweens with the given ID (regardless of their target gameObject)
        /// </summary>
        public void DOPlayBackwardsAllById(string id)
        {
            DOTween.PlayBackwards(id);
        }

        /// <summary>
        /// Plays forward all tweens with the given ID and whose target-id is the same as the one set by this animation
        /// </summary>
        public void DOPlayForwardById(string id)
        {
            DOTween.PlayForward(GetTweenTarget(), id);
        }
        /// <summary>
        /// Plays forward all tweens with the given ID (regardless of their target gameObject)
        /// </summary>
        public void DOPlayForwardAllById(string id)
        {
            DOTween.PlayForward(id);
        }

        /// <summary>
        /// Plays the next animation on this animation's gameObject (if any)
        /// </summary>
        public void DOPlayNext()
        {
            DOTweenAnimation[] anims = this.GetComponents<DOTweenAnimation>();
            while (_playCount < anims.Length - 1) {
                _playCount++;
                DOTweenAnimation anim = anims[_playCount];
                if (anim != null && anim.tween != null && anim.tween.active && !anim.tween.IsPlaying() && !anim.tween.IsComplete()) {
                    anim.tween.Play();
                    break;
                }
            }
        }

        /// <summary>
        /// Rewinds all tweens with the given ID and whose target-id is the same as the one set by this animation,
        /// then plays the next animation on this animation's gameObject (if any)
        /// </summary>
        public void DORewindAndPlayNext()
        {
            _playCount = -1;
            DOTween.Rewind(GetTweenTarget());
            DOPlayNext();
        }

        /// <summary>
        /// Rewinds all tweens with the given ID (regardless of their target gameObject)
        /// </summary>
        public void DORewindAllById(string id)
        {
            _playCount = -1;
            DOTween.Rewind(id);
        }

        /// <summary>
        /// Restarts all tweens with the given ID and whose target-id is the same as the one set by this animation
        /// </summary>
        public void DORestartById(string id)
        {
            _playCount = -1;
            DOTween.Restart(GetTweenTarget(), id);
        }
        /// <summary>
        /// Restarts all tweens with the given ID (regardless of their target gameObject)
        /// </summary>
        public void DORestartAllById(string id)
        {
            _playCount = -1;
            DOTween.Restart(id);
        }

        /// <summary>
        /// Kills all tweens with the given ID and whose target-id is the same as the one set by this animation
        /// </summary>
        public void DOKillById(string id)
        {
            DOTween.Kill(GetTweenTarget(), id);
        }
        /// <summary>
        /// Kills all tweens with the given ID (regardless of their target gameObject)
        /// </summary>
        public void DOKillAllById(string id)
        {
            DOTween.Kill(id);
        }

        #endregion

        #region Internal (also used by Inspector)

        public static TargetType TypeToDOTargetType(Type t)
        {
            string str = t.ToString();
            int dotIndex = str.LastIndexOf(".");
            if (dotIndex != -1) str = str.Substring(dotIndex + 1);
            if (str.IndexOf("Renderer") != -1 && (str != "SpriteRenderer")) str = "Renderer";
//#if PHYSICS_MARKER
//            if (str == "Rigidbody") str = "Transform";
//#endif
//#if true // PHYSICS2D_MARKER
//            if (str == "Rigidbody2D") str = "Transform";
//#endif
#if true // UI_MARKER
//            if (str == "RectTransform") str = "Transform";
            if (str == "RawImage" || str == "Graphic") str = "Image"; // RawImages/Graphics are managed like Images for DOTweenAnimation (color and fade use Graphic target anyway)
#endif
            return (TargetType)Enum.Parse(typeof(TargetType), str);
        }

        // Editor preview system
        /// <summary>
        /// Previews the tween in the editor. Only for DOTween internal usage: don't use otherwise.
        /// </summary>
        public Tween CreateEditorPreview()
        {
            if (Application.isPlaying) return null;

            // CHANGE: first param switched to TRUE otherwise changing an animation and replaying in editor would still play old one
            CreateTween(true, autoPlay);
            return tween;
        }

        #endregion

        #endregion

        #region Private

        /// <summary>
        /// Returns the gameObject whose target component should be animated
        /// </summary>
        /// <returns></returns>
        GameObject GetTweenGO()
        {
            return targetIsSelf ? this.gameObject : targetGO;
        }

        /// <summary>
        /// Returns the GameObject which should be used/retrieved for SetTarget
        /// </summary>
        GameObject GetTweenTarget()
        {
            return targetIsSelf || !tweenTargetIsTargetGO ? this.gameObject : targetGO;
        }

        // Re-evaluate relative position of path
        void ReEvaluateRelativeTween()
        {
            GameObject tweenGO = GetTweenGO();
            if (tweenGO == null) {
                Debug.LogWarning(string.Format("{0} :: This DOTweenAnimation's target/GameObject is unset: the tween will not be created.", this.gameObject.name), this.gameObject);
                return;
            }
            if (animationType == AnimationType.Move) {
                ((Tweener)tween).ChangeEndValue(tweenGO.transform.position + endValueV3, true);
            } else if (animationType == AnimationType.LocalMove) {
                ((Tweener)tween).ChangeEndValue(tweenGO.transform.localPosition + endValueV3, true);
            }
        }

        #endregion
    }

    public static class DOTweenAnimationExtensions
    {
//        // Doesn't work on Win 8.1
//        public static bool IsSameOrSubclassOf(this Type t, Type tBase)
//        {
//            return t.IsSubclassOf(tBase) || t == tBase;
//        }

        public static bool IsSameOrSubclassOf<T>(this Component t)
        {
            return t is T;
        }
    }
}
