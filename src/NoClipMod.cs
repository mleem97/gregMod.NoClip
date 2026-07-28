using System;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(NoClip.NoClipMod), "gregMod.NoClip", "1.0.2", "TeamGreg Modding")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace NoClip
{
    public class NoClipMod : MelonMod
    {
        // ── Preferences ───────────────────────────────────────────────────────
        private static MelonPreferences_Entry<string> ToggleKeyEntry;
        private static MelonPreferences_Entry<float>  SpeedEntry;
        private static MelonPreferences_Entry<float>  FastMultiplierEntry;

        // ── Runtime state ─────────────────────────────────────────────────────
        private static Key _toggleKey = Key.F4;
        private static bool    _active    = false;

        // ── Cached scene references (cleared on scene change) ─────────────────
        private static CharacterController _cc;
        private static Rigidbody           _rb;
        private static Transform           _playerRoot;
        private static Camera              _camera;

        // ── GUI ───────────────────────────────────────────────────────────────
        private static GUIStyle _labelStyle;

        // ─────────────────────────────────────────────────────────────────────

        public override void OnInitializeMelon()
        {
            var cat = MelonPreferences.CreateCategory("gregMod.NoClip");

            ToggleKeyEntry = cat.CreateEntry("ToggleKey", "F4", "Toggle Key",
                "Input System key to toggle noclip on/off (e.g. F4, F9, Backquote).");
            SpeedEntry = cat.CreateEntry("Speed", 5f, "Speed",
                "Fly speed in metres per second.");
            FastMultiplierEntry = cat.CreateEntry("FastMultiplier", 3f, "Fast Multiplier",
                "Speed multiplier applied while holding Left Shift.");

            if (Enum.TryParse<Key>(ToggleKeyEntry.Value, true, out var k) && k != Key.None)
                _toggleKey = k;
            else
                LoggerInstance.Warning($"[NoClip] Unknown KeyCode '{ToggleKeyEntry.Value}', defaulting to F4.");

            LoggerInstance.Msg($"[NoClip] Loaded. Press {_toggleKey} to toggle.");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            // Scene reload invalidates all cached component pointers.
            _cc         = null;
            _rb         = null;
            _playerRoot = null;
            _camera     = null;

            if (_active)
            {
                _active = false;
                LoggerInstance.Msg("[NoClip] Scene changed — noclip auto-disabled.");
            }
        }

        public override void OnUpdate()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            if (keyboard[_toggleKey].wasPressedThisFrame)
                Toggle();

            if (!_active) return;

            // Lazily resolve player refs if they got cleared (e.g. after late scene init).
            if (_playerRoot == null && !TryFindPlayer())
                return;

            Camera cam = GetCamera();
            if (cam == null) return;

            float speed = SpeedEntry.Value;
            if (keyboard.leftShiftKey.isPressed) speed *= FastMultiplierEntry.Value;

            // Full 6DoF free-fly relative to camera orientation.
            Vector3 move = Vector3.zero;
            if (keyboard.wKey.isPressed)           move += cam.transform.forward;
            if (keyboard.sKey.isPressed)           move -= cam.transform.forward;
            if (keyboard.dKey.isPressed)           move += cam.transform.right;
            if (keyboard.aKey.isPressed)           move -= cam.transform.right;
            if (keyboard.spaceKey.isPressed)       move += Vector3.up;
            if (keyboard.leftCtrlKey.isPressed)    move -= Vector3.up;

            if (move.sqrMagnitude > 0.001f)
                _playerRoot.position += move.normalized * (speed * Time.deltaTime);
        }

        public override void OnGUI()
        {
            if (!_active) return;

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle()
                {
                    fontSize  = 14,
                    fontStyle = FontStyle.Bold,
                };
                _labelStyle.normal.textColor = new Color(0.2f, 1f, 0.4f, 0.9f);
            }

            GUI.Label(
                new Rect(10f, 10f, 400f, 24f),
                $"NOCLIP ON  [{_toggleKey} to exit]   W/A/S/D  Space=Up  LCtrl=Down  LShift=Fast",
                _labelStyle);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static void Toggle()
        {
            _active = !_active;

            if (_active)
            {
                if (!TryFindPlayer())
                {
                    _active = false;
                    MelonLogger.Warning("[NoClip] Could not find player object — noclip cancelled.");
                    return;
                }

                // Disable physics-driven movement so the player doesn't fall.
                if (_cc != null) _cc.enabled = false;
                if (_rb != null) { _rb.isKinematic = true; _rb.useGravity = false; }

                MelonLogger.Msg("[NoClip] ON");
            }
            else
            {
                // Restore physics.
                if (_cc != null) _cc.enabled = true;
                if (_rb != null) { _rb.isKinematic = false; _rb.useGravity = true; }

                MelonLogger.Msg("[NoClip] OFF");
            }
        }

        /// <summary>
        /// Tries to locate the player's root Transform (and its physics components).
        /// Strategy: PlayerManager singleton → scene-wide CC search → camera parent fallback.
        /// Returns true if a usable transform was found.
        /// </summary>
        private static bool TryFindPlayer()
        {
            // 1. PlayerManager singleton — the game's own movement controller.
            try
            {
                var pm = PlayerManager.instance;
                if (pm != null)
                {
                    _cc = pm.GetComponent<CharacterController>();
                    if (_cc == null) _cc = pm.GetComponentInChildren<CharacterController>();

                    _rb = pm.GetComponent<Rigidbody>();
                    if (_rb == null) _rb = pm.GetComponentInChildren<Rigidbody>();

                    // Use the CharacterController's transform as the root so that
                    // moving it carries the camera child along with it.
                    _playerRoot = _cc != null ? _cc.transform : pm.transform;
                    return true;
                }
            }
            catch { /* PlayerManager absent in this scene */ }

            // 2. Scene-wide CharacterController search.
            _cc = UnityEngine.Object.FindObjectOfType<CharacterController>();
            if (_cc != null)
            {
                _playerRoot = _cc.transform;
                return true;
            }

            // 3. Camera parent as a last resort (covers camera-only rigs).
            Camera cam = GetCamera();
            if (cam != null)
            {
                _playerRoot = cam.transform.parent != null ? cam.transform.parent : cam.transform;
                MelonLogger.Warning("[NoClip] No CharacterController found — noclipping camera transform.");
                return true;
            }

            return false;
        }

        private static Camera GetCamera()
        {
            if (_camera != null) return _camera;
            try { _camera = MainGameManager.instance?.playerCamera; } catch { }
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }
}
