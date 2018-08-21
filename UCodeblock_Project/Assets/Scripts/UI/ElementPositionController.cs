using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UCodeblock.UI
{
    /// <summary>
    /// Provides functionality for moving an element in local space.
    /// </summary>
    public class LocalPositionController
    {
        private readonly Transform _transform;
        private readonly float _smoothTime;
        private Vector3 _velocity;

        /// <summary>
        /// Returns the target position of the object. This is the position the object goes towards when calling <see cref="UpdatePosition"/>.
        /// </summary>
        public Vector3 TargetLocalPosition { get; set; }

        public LocalPositionController(Transform transform, float smoothTime)
        {
            _transform = transform;
            _smoothTime = smoothTime;

            Freeze();
        }

        /// <summary>
        /// Returns the local position of the object.
        /// </summary>
        public Vector3 LocalPosition
        {
            get { return _transform.localPosition; }
            set { _transform.localPosition = value; }
        }

        /// <summary>
        /// Sets the local position, and at the same time updates the target position.
        /// </summary>
        public void SetLocalPosition (Vector3 position)
        {
            LocalPosition = position;
            TargetLocalPosition = LocalPosition;
        }
        /// <summary>
        /// Updates the position by smoothing towards the <see cref="TargetLocalPosition"/>.
        /// </summary>
        public void UpdatePosition ()
        {
            MoveSmooth(TargetLocalPosition, _smoothTime);
        }
        /// <summary>
        /// Freezes the object.
        /// </summary>
        public void Freeze ()
        {
            TargetLocalPosition = LocalPosition;
        }

        /// <summary>
        /// Moves the object smoothly towards the position.
        /// </summary>
        protected void MoveSmooth(Vector3 position, float smoothTime)
        {
            Vector3 newPosition = Vector3.SmoothDamp(LocalPosition, position, ref _velocity, smoothTime);
            LocalPosition = newPosition;
        }
    }
}