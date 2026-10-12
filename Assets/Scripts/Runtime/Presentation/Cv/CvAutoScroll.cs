using System.Collections.Generic;
using CvWarehouse.Core.Cv;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CvWarehouse.Presentation.Cv
{
    [RequireComponent(typeof(ScrollRect))]
    public sealed class CvAutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler, IBeginDragHandler
    {
        private const float CloseEnough = 0.5f;

        [SerializeField] private float idleSecondsBeforeFollowing = 5f;
        [SerializeField] private float secondsBetweenScrolls = 2f;
        [SerializeField] private float glideSeconds = 0.35f;

        private ScrollRect scrollRect;
        private CvFollowTimer timer;
        private CvRevealState revealState;
        private IReadOnlyDictionary<string, CvRevealable> blocks;
        private RectTransform waitingBlock;
        private bool isPointerOver;
        private bool isGliding;
        private float glideTarget;
        private float glideSpeed;

        public bool IsSwitchedOn { get; private set; } = true;

        public bool IsFollowing => IsSwitchedOn && timer != null && timer.IsFollowing;

        public float Offset => (1f - scrollRect.verticalNormalizedPosition) * FurthestOffset;

        private float ViewportHeight => scrollRect.viewport.rect.height;

        private float FurthestOffset => CvScrollMaths.FurthestOffset(ViewportHeight, scrollRect.content.rect.height);

        public void Follow(CvRevealState state, IReadOnlyDictionary<string, CvRevealable> revealables)
        {
            revealState = state;
            blocks = revealables;
            timer = new CvFollowTimer(idleSecondsBeforeFollowing, secondsBetweenScrolls);
            if (isActiveAndEnabled)
                revealState.Delivered += OnDelivered;
        }

        public void Toggle()
        {
            IsSwitchedOn = !IsSwitchedOn;
            isGliding = false;
            waitingBlock = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerOver = true;
            NoteActivity();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOver = false;
            NoteActivity();
        }

        public void OnScroll(PointerEventData eventData)
        {
            NoteActivity();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            NoteActivity();
        }

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable()
        {
            if (revealState != null)
                revealState.Delivered += OnDelivered;
        }

        private void OnDisable()
        {
            isGliding = false;
            waitingBlock = null;
            if (revealState != null)
                revealState.Delivered -= OnDelivered;
        }

        private void LateUpdate()
        {
            if (timer == null)
                return;

            if (isPointerOver)
                timer.NoteActivity();
            timer.Advance(Time.unscaledDeltaTime);
            StartGlideToWaitingBlock();
            if (isGliding)
                Glide();
        }

        private void NoteActivity()
        {
            isGliding = false;
            timer?.NoteActivity();
        }

        private void OnDelivered(string blockId)
        {
            if (IsSwitchedOn && blocks.TryGetValue(blockId, out CvRevealable block))
                waitingBlock = (RectTransform)block.transform;
        }

        private void StartGlideToWaitingBlock()
        {
            if (waitingBlock == null || !IsFollowing)
                return;

            float blockFromTop = DistanceFromTop(waitingBlock);
            float shownOffset = isGliding ? glideTarget : Offset;
            if (CvScrollMaths.IsInView(blockFromTop, shownOffset, ViewportHeight))
            {
                waitingBlock = null;
                return;
            }

            if (!timer.TryScroll())
                return;

            glideTarget = CvScrollMaths.OffsetCentring(blockFromTop, ViewportHeight, scrollRect.content.rect.height);
            isGliding = true;
            waitingBlock = null;
        }

        private float DistanceFromTop(RectTransform block)
        {
            RectTransform content = scrollRect.content;
            Vector3 centre = content.InverseTransformPoint(block.TransformPoint(block.rect.center));
            return content.rect.yMax - centre.y;
        }

        private void Glide()
        {
            float furthestOffset = FurthestOffset;
            if (furthestOffset <= 0f)
            {
                isGliding = false;
                return;
            }

            float offset = Mathf.SmoothDamp(Offset, glideTarget, ref glideSpeed, glideSeconds, Mathf.Infinity, Time.unscaledDeltaTime);
            if (Mathf.Abs(offset - glideTarget) < CloseEnough)
            {
                offset = glideTarget;
                isGliding = false;
                glideSpeed = 0f;
            }

            scrollRect.StopMovement();
            scrollRect.verticalNormalizedPosition = 1f - offset / furthestOffset;
        }
    }
}
