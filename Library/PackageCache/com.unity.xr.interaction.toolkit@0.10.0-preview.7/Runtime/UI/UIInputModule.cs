using System;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Base class for input modules that send UI input.
    /// </summary>
    /// <remarks>
    /// Multiple input modules may be placed on the same event system. In such a setup,
    /// the modules will synchronize with each other.
    /// </remarks>
    public abstract class UIInputModule : BaseInputModule
    {
        [SerializeField, FormerlySerializedAs("clickSpeed")]
        [Tooltip("The maximum time (in seconds) between two mouse presses for it to be consecutive click.")]
        float m_ClickSpeed = 0.3f;
        /// <summary>
        /// The maximum time (in seconds) between two mouse presses for it to be consecutive click.
        /// </summary>
        public float clickSpeed
        {
            get => m_ClickSpeed;
            set => m_ClickSpeed = value;
        }

        [SerializeField, FormerlySerializedAs("moveDeadzone")]
        [Tooltip("The absolute value required by a move action on either axis required to trigger a move event.")]
        float m_MoveDeadzone = 0.6f;
        /// <summary>
        /// The absolute value required by a move action on either axis required to trigger a move event.
        /// </summary>
        public float moveDeadzone
        {
            get => m_MoveDeadzone;
            set => m_MoveDeadzone = value;
        }

        [SerializeField, FormerlySerializedAs("repeatDelay")]
        [Tooltip("The Initial delay (in seconds) between an initial move action and a repeated move action.")]
        float m_RepeatDelay = 0.5f;
        /// <summary>
        /// The Initial delay (in seconds) between an initial move action and a repeated move action.
        /// </summary>
        public float repeatDelay
        {
            get => m_RepeatDelay;
            set => m_RepeatDelay = value;
        }

        [FormerlySerializedAs("repeatRate")]
        [SerializeField, Tooltip("The speed (in seconds) that the move action repeats itself once repeating.")]
        float m_RepeatRate = 0.1f;
        /// <summary>
        /// The speed (in seconds) that the move action repeats itself once repeating.
        /// </summary>
        public float repeatRate
        {
            get => m_RepeatRate;
            set => m_RepeatRate = value;
        }

        [FormerlySerializedAs("trackedDeviceDragThresholdMultiplier")]
        [SerializeField, Tooltip("Scales the EventSystem.pixelDragThreshold, for tracked devices, to make selection easier.")]
        float m_TrackedDeviceDragThresholdMultiplier = 2f;
        /// <summary>
        /// Scales the <see cref="EventSystem.pixelDragThreshold"/>, for tracked devices, to make selection easier.
        /// </summary>
        public float trackedDeviceDragThresholdMultiplier
        {
            get => m_TrackedDeviceDragThresholdMultiplier;
            set => m_TrackedDeviceDragThresholdMultiplier = value;
        }

        /// <summary>
        /// The <see cref="Camera"/> that is used to perform 2D raycasts when determining the screen space location of a tracked device cursor.
        /// </summary>
        public Camera uiCamera { get; set; }

        AxisEventData m_CachedAxisEvent;
        PointerEventData m_CachedPointerEvent;
        TrackedDeviceEventData m_CachedTrackedDeviceEventData;

        /// <summary>
        /// Process the current tick for the module.
        /// </summary>
        /// <remarks>
        /// Executed once per Update call.
        /// </remarks>
        protected abstract void DoProcess();

        /// <inheritdoc />
        public override void Process()
        {
            if (eventSystem.currentSelectedGameObject != null)
            {
                var data = GetBaseEventData();
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            }

            DoProcess();
        }

        RaycastResult PerformRaycast(PointerEventData eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            eventSystem.RaycastAll(eventData, m_RaycastResultCache);
            var result = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();
            return result;
        }

        /// <summary>
        /// Takes an existing <see cref="MouseModel"/> and dispatches all relevant changes through the event system.
        /// It also updates the internal data of the <see cref="MouseModel"/>.
        /// </summary>
        /// <param name="mouseState">The mouse state you want to forward into the UI Event System.</param>
        internal void ProcessMouse(ref MouseModel mouseState)
        {
            if (!mouseState.changedThisFrame)
                return;

            var eventData = GetOrCreateCachedPointerEvent();
            eventData.Reset();

            mouseState.CopyTo(eventData);

            eventData.pointerCurrentRaycast = PerformRaycast(eventData);

            // Left Mouse Button
            // The left mouse button is 'dominant' and we want to also process hover and scroll events as if the occurred during the left click.
            var buttonState = mouseState.leftButton;
            buttonState.CopyTo(eventData);
            ProcessMouseButton(buttonState.lastFrameDelta, eventData);

            ProcessMouseMovement(eventData);
            ProcessMouseScroll(eventData);

            mouseState.CopyFrom(eventData);

            ProcessMouseButtonDrag(eventData);

            buttonState.CopyFrom(eventData);
            mouseState.leftButton = buttonState;

            // Right Mouse Button
            buttonState = mouseState.rightButton;
            buttonState.CopyTo(eventData);

            ProcessMouseButton(buttonState.lastFrameDelta, eventData);
            ProcessMouseButtonDrag(eventData);

            buttonState.CopyFrom(eventData);
            mouseState.rightButton = buttonState;

            // Middle Mouse Button
            buttonState = mouseState.middleButton;
            buttonState.CopyTo(eventData);

            ProcessMouseButton(buttonState.lastFrameDelta, eventData);
            ProcessMouseButtonDrag(eventData);

            buttonState.CopyFrom(eventData);
            mouseState.middleButton = buttonState;

            mouseState.OnFrameFinished();
        }

        void ProcessMouseMovement(PointerEventData eventData)
        {
            var currentPointerTarget = eventData.pointerCurrentRaycast.gameObject;

            // If we have no target or pointerEnter has been deleted,
            // we just send exit events to anything we are tracking
            // and then exit.
            if (currentPointerTarget == null || eventData.pointerEnter == null)
            {
                foreach (var go in eventData.hovered)
                    ExecuteEvents.Execute(go, eventData, ExecuteEvents.pointerExitHandler);

                eventData.hovered.Clear();

                if (currentPointerTarget == null)
                {
                    eventData.pointerEnter = null;
                    return;
                }
            }

            if (eventData.pointerEnter == currentPointerTarget && currentPointerTarget)
                return;

            var commonRoot = FindCommonRoot(eventData.pointerEnter, currentPointerTarget);

            // We walk up the tree until a common root and the last entered and current entered object is found.
            // Then send exit and enter events up to, but not including, the common root.
            if (eventData.pointerEnter != null)
            {
                var t = eventData.pointerEnter.transform;

                while (t != null)
                {
                    if (commonRoot != null && commonRoot.transform == t)
                        break;

                    ExecuteEvents.Execute(t.gameObject, eventData, ExecuteEvents.pointerExitHandler);

                    eventData.hovered.Remove(t.gameObject);

                    t = t.parent;
                }
            }

            eventData.pointerEnter = currentPointerTarget;
            if (currentPointerTarget != null)
            {
                var t = currentPointerTarget.transform;

                while (t != null && t.gameObject != commonRoot)
                {
                    ExecuteEvents.Execute(t.gameObject, eventData, ExecuteEvents.pointerEnterHandler);

                    eventData.hovered.Add(t.gameObject);

                    t = t.parent;
                }
            }
        }

        void ProcessMouseButton(ButtonDeltaState mouseButtonChanges, PointerEventData eventData)
        {
            var currentOverGo = eventData.pointerCurrentRaycast.gameObject;

            if ((mouseButtonChanges & ButtonDeltaState.Pressed) != 0)
            {
                eventData.eligibleForClick = true;
                eventData.delta = Vector2.zero;
                eventData.dragging = false;
                eventData.pressPosition = eventData.position;
                eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

                var selectHandlerGO = ExecuteEvents.GetEventHandler<ISelectHandler>(currentOverGo);

                // If we have clicked something new, deselect the old thing
                // and leave 'selection handling' up to the press event.
                if (selectHandlerGO != eventSystem.currentSelectedGameObject)
                    eventSystem.SetSelectedGameObject(null, eventData);

                // search for the control that will receive the press.
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.pointerDownHandler);

                // We didn't find a press handler, so we search for a click handler.
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                var time = Time.unscaledTime;

                if (newPressed == eventData.lastPress && ((time - eventData.clickTime) < m_ClickSpeed))
                    ++eventData.clickCount;
                else
                    eventData.clickCount = 1;

                eventData.clickTime = time;

                eventData.pointerPress = newPressed;
                eventData.rawPointerPress = currentOverGo;

                // Save the drag handler for drag events during this mouse down.
                eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (eventData.pointerDrag != null)
                    ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
            }

            if ((mouseButtonChanges & ButtonDeltaState.Released) != 0)
            {
                ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                if (eventData.pointerPress == pointerUpHandler && eventData.eligibleForClick)
                {
                    ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
                }
                else if (eventData.dragging && eventData.pointerDrag != null)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.dropHandler);
                    ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
                }

                eventData.eligibleForClick = eventData.dragging = false;
                eventData.pointerPress = eventData.rawPointerPress = eventData.pointerDrag = null;
            }
        }

        void ProcessMouseButtonDrag(PointerEventData eventData, float pixelDragThresholdMultiplier = 1.0f)
        {
            if (!eventData.IsPointerMoving() ||
                Cursor.lockState == CursorLockMode.Locked ||
                eventData.pointerDrag == null)
            {
                return;
            }

            if (!eventData.dragging)
            {
                if ((eventData.pressPosition - eventData.position).sqrMagnitude >= ((eventSystem.pixelDragThreshold * eventSystem.pixelDragThreshold) * pixelDragThresholdMultiplier))
                {
                    ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
                    eventData.dragging = true;
                }
            }

            if (eventData.dragging)
            {
                // If we moved from our initial press object, process an up for that object.
                if (eventData.pointerPress != eventData.pointerDrag)
                {
                    ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                    eventData.rawPointerPress = null;
                }
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
            }
        }

        void ProcessMouseScroll(PointerEventData eventData)
        {
            var scrollDelta = eventData.scrollDelta;
            if (!Mathf.Approximately(scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerEnter);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, eventData, ExecuteEvents.scrollHandler);
            }
        }

        internal void ProcessTouch(ref TouchModel touchState)
        {
            if (!touchState.changedThisFrame)
                return;

            var eventData = GetOrCreateCachedPointerEvent();
            eventData.Reset();

            touchState.CopyTo(eventData);

            eventData.pointerCurrentRaycast = (touchState.selectPhase == TouchPhase.Canceled) ? new RaycastResult() : PerformRaycast(eventData);
            eventData.button = PointerEventData.InputButton.Left;

            ProcessMouseButton(touchState.selectDelta, eventData);
            ProcessMouseMovement(eventData);
            ProcessMouseButtonDrag(eventData);

            touchState.CopyFrom(eventData);

            touchState.OnFrameFinished();
        }

        internal void ProcessTrackedDevice(ref TrackedDeviceModel deviceState, bool force = false)
        {
            if (!deviceState.changedThisFrame && !force)
                return;

            var eventData = GetOrCreateCachedTrackedDeviceEvent();
            eventData.Reset();
            deviceState.CopyTo(eventData);

            eventData.button = PointerEventData.InputButton.Left;

            eventData.pointerCurrentRaycast = PerformRaycast(eventData);

            // Get associated camera, or main-tagged camera, or camera from raycast, and if *nothing* exists, then abort processing this frame.
            var camera = uiCamera ? uiCamera : Camera.main;
            if (camera == null)
            {
                var module = eventData.pointerCurrentRaycast.module;
                if (module)
                    camera = module.eventCamera;
            }

            if (camera != null)
            {
                Vector2 screenPosition;
                if (eventData.pointerCurrentRaycast.isValid)
                {
                    screenPosition = camera.WorldToScreenPoint(eventData.pointerCurrentRaycast.worldPosition);
                }
                else
                {
                    var endPosition = eventData.rayPoints.Count > 0 ? eventData.rayPoints[eventData.rayPoints.Count - 1] : Vector3.zero;
                    screenPosition = camera.WorldToScreenPoint(endPosition);
                    eventData.position = screenPosition;
                }

                var thisFrameDelta = screenPosition - eventData.position;
                eventData.position = screenPosition;
                eventData.delta = thisFrameDelta;

                ProcessMouseButton(deviceState.selectDelta, eventData);
                ProcessMouseMovement(eventData);
                ProcessMouseButtonDrag(eventData, m_TrackedDeviceDragThresholdMultiplier);

                deviceState.CopyFrom(eventData);
            }

            deviceState.OnFrameFinished();
        }

        // TODO Update UIInputModule to make use of unused ProcessJoystick method
        /// <summary>
        /// Takes an existing JoystickModel and dispatches all relevant changes through the event system.
        /// It also updates the internal data of the JoystickModel.
        /// </summary>
        /// <param name="joystickState">The joystick state you want to forward into the UI Event System</param>
        internal void ProcessJoystick(ref JoystickModel joystickState)
        {
            var implementationData = joystickState.implementationData;

            var usedSelectionChange = false;
            if (eventSystem.currentSelectedGameObject != null)
            {
                var data = GetBaseEventData();
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
                usedSelectionChange = data.used;
            }

            // Don't send move events if disabled in the EventSystem.
            if (!eventSystem.sendNavigationEvents)
                return;

            var movement = joystickState.move;
            if (!usedSelectionChange && (!Mathf.Approximately(movement.x, 0f) || !Mathf.Approximately(movement.y, 0f)))
            {
                var time = Time.unscaledTime;

                var moveVector = joystickState.move;

                var moveDirection = MoveDirection.None;
                if (moveVector.sqrMagnitude > m_MoveDeadzone * m_MoveDeadzone)
                {
                    if (Mathf.Abs(moveVector.x) > Mathf.Abs(moveVector.y))
                        moveDirection = (moveVector.x > 0) ? MoveDirection.Right : MoveDirection.Left;
                    else
                        moveDirection = (moveVector.y > 0) ? MoveDirection.Up : MoveDirection.Down;
                }

                if (moveDirection != implementationData.lastMoveDirection)
                {
                    implementationData.consecutiveMoveCount = 0;
                }

                if (moveDirection != MoveDirection.None)
                {
                    var allow = true;
                    if (implementationData.consecutiveMoveCount != 0)
                    {
                        if (implementationData.consecutiveMoveCount > 1)
                            allow = (time > (implementationData.lastMoveTime + m_RepeatRate));
                        else
                            allow = (time > (implementationData.lastMoveTime + m_RepeatDelay));
                    }

                    if (allow)
                    {
                        var eventData = GetOrCreateCachedAxisEvent();
                        eventData.Reset();

                        eventData.moveVector = moveVector;
                        eventData.moveDir = moveDirection;

                        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, eventData, ExecuteEvents.moveHandler);
                        usedSelectionChange = eventData.used;

                        implementationData.consecutiveMoveCount++;
                        implementationData.lastMoveTime = time;
                        implementationData.lastMoveDirection = moveDirection;
                    }
                }
                else
                    implementationData.consecutiveMoveCount = 0;
            }
            else
                implementationData.consecutiveMoveCount = 0;

            if (!usedSelectionChange)
            {
                if (eventSystem.currentSelectedGameObject != null)
                {
                    var data = GetBaseEventData();
                    if ((joystickState.submitButtonDelta & ButtonDeltaState.Pressed) != 0)
                        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

                    if (!data.used && (joystickState.cancelButtonDelta & ButtonDeltaState.Pressed) != 0)
                        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
                }
            }

            joystickState.implementationData = implementationData;
            joystickState.OnFrameFinished();
        }

        PointerEventData GetOrCreateCachedPointerEvent()
        {
            var result = m_CachedPointerEvent;
            if (result == null)
            {
                result = new PointerEventData(eventSystem);
                m_CachedPointerEvent = result;
            }

            return result;
        }

        TrackedDeviceEventData GetOrCreateCachedTrackedDeviceEvent()
        {
            var result = m_CachedTrackedDeviceEventData;
            if (result == null)
            {
                result = new TrackedDeviceEventData(eventSystem);
                m_CachedTrackedDeviceEventData = result;
            }

            return result;
        }

        AxisEventData GetOrCreateCachedAxisEvent()
        {
            var result = m_CachedAxisEvent;
            if (result == null)
            {
                result = new AxisEventData(eventSystem);
                m_CachedAxisEvent = result;
            }

            return result;
        }
    }
}
