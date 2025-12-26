using UnityEngine;

    public class FitCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [Header("Map Size (world units)")]
        [SerializeField] private float _mapWidth = 16f;
        [SerializeField] private float _mapHeight = 9f; 

        [Header("Padding (world units)")]
        [SerializeField] private float _extraWidth = 0f;
        [SerializeField] private float _extraHeight = 0f; 

        [Header("Clamp")]
        [SerializeField] private float _minOrthoSize = 7f;

        int _lastW, _lastH;

        private void Reset() => _camera = Camera.main;

        private void Awake()
        {
            if (_camera == null) _camera = Camera.main;
        }

        private void Start() => Apply();

        private void Update()
        {
            if (Screen.width != _lastW || Screen.height != _lastH)
                Apply();
        }

        private void Apply()
        {
            if (_camera == null) return;

            _lastW = Screen.width;
            _lastH = Screen.height;

            float aspect = (float)Screen.width / Screen.height;
        
            float targetWidth = Mathf.Max(0.01f, _mapWidth + _extraWidth);
            float sizeByWidth = (targetWidth / 2f) / aspect;

    
            float targetHeight = Mathf.Max(0.01f, _mapHeight + _extraHeight);
            float sizeByHeight = targetHeight / 2f;

            _camera.orthographic = true;

            _camera.orthographicSize = Mathf.Max(sizeByWidth, sizeByHeight, _minOrthoSize);
        }
    }
