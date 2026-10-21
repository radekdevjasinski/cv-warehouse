using CvWarehouse.Presentation.Warehouse;
using UnityEngine;

namespace CvWarehouse.Presentation.Menu
{
    [RequireComponent(typeof(Camera))]
    public sealed class MenuCamera : MonoBehaviour
    {
        [SerializeField] private MenuFloorView floor;
        [SerializeField] private Transform focus;
        [SerializeField] private int spritePixelsPerUnit = 16;
        [SerializeField] private float viewCells = 9f;

        private Camera menuCamera;
        private CameraFraming framing;

        private void Awake()
        {
            menuCamera = GetComponent<Camera>();
            framing = new CameraFraming(floor.Size, new CameraZoomLimits(spritePixelsPerUnit, viewCells, viewCells));
        }

        private void LateUpdate()
        {
            framing.SetViewPixels(new Vector2Int(menuCamera.pixelWidth, menuCamera.pixelHeight));
            framing.ZoomOutFully();
            framing.LookAt(focus.position);
            transform.position = new Vector3(framing.Centre.x, framing.Centre.y, transform.position.z);
            menuCamera.orthographicSize = framing.Size;
        }
    }
}
