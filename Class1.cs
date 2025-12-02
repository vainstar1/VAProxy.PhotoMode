using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using Invector.vCamera;
using Invector.vCharacterController;
using Invector.vMelee;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FreecamMod
{
    internal class Freecam : MonoBehaviour
    {
        vThirdPersonCamera original_camera;
        public float desiredMoveSpeed
        {
            get => _desiredMoveSpeed;
            private set
            {
                _desiredMoveSpeed = value;
            }
        }
        float _desiredMoveSpeed = 5;
        IInputSystem InputManager = UnityInput.Current;
        internal bool freecamActive = false;
        void Start()
        {
            original_camera = GetComponent<vThirdPersonCamera>();
        }

        public void setActive(bool active)
        {
            StartCoroutine(SetMovementComponents(!active));
            freecamActive = active;
            original_camera.enabled = !active;
            GameObject sen = GameObject.Find("S-105.1");
            Rigidbody body = sen.GetComponent<Rigidbody>();
            body.velocity = Vector3.zero;
        }

        private Vector3? anchPosition;

        IEnumerator SetMovementComponents(bool active)
        {
            GameObject sen = GameObject.Find("S-105.1");
            while (true)
            {
                if (sen)
                    break;
                sen = GameObject.Find("S-105.1");
                yield return new WaitForEndOfFrame();
            }
            GameObject v06 = GameObject.Find("V-06");
            while (true)
            {
                if (v06)
                    break;
                v06 = GameObject.Find("V-06");
                yield return new WaitForEndOfFrame();
            }
            sen.GetComponent<vThirdPersonController>().enabled = active;
            sen.GetComponent<vShooterMeleeInput>().enabled = active;
            sen.GetComponent<vMeleeManager>().enabled = active;
            v06.GetComponent<Drone>().enabled = active;
            Rigidbody body = sen.GetComponent<Rigidbody>();
            body.useGravity = active;
            if (!active)
                body.velocity = Vector3.zero;
        }

        void EnsureVelocityIsZero()
        {
            GameObject sen = GameObject.Find("S-105.1");
            if (anchPosition == null)
                anchPosition = sen.transform.localPosition;
            sen.transform.localPosition = anchPosition.Value;
            Rigidbody body = sen.GetComponent<Rigidbody>();
            body.velocity = Vector3.zero;
        }

        void Update()
        {
            if (freecamActive)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                desiredMoveSpeed += scroll * 10;
                EnsureVelocityIsZero();
            }
            else
                anchPosition = null;
        }

        void LateUpdate()
        {
            if (freecamActive)
            {
                float moveSpeed = desiredMoveSpeed * 0.25f;

                if (InputManager.GetKey(KeyCode.LeftShift) || InputManager.GetKey(KeyCode.RightShift))
                    moveSpeed *= 4f;
                if (InputManager.GetKey(KeyCode.LeftArrow) || InputManager.GetKey(KeyCode.A))
                    transform.position += transform.right * -1 * moveSpeed;
                if (InputManager.GetKey(KeyCode.RightArrow) || InputManager.GetKey(KeyCode.D))
                    transform.position += transform.right * moveSpeed;
                if (InputManager.GetKey(KeyCode.UpArrow) || InputManager.GetKey(KeyCode.W))
                    transform.position += transform.forward * moveSpeed;
                if (InputManager.GetKey(KeyCode.DownArrow) || InputManager.GetKey(KeyCode.S))
                    transform.position += transform.forward * -1 * moveSpeed;
                if (InputManager.GetKey(KeyCode.Space) || InputManager.GetKey(KeyCode.PageUp))
                    transform.position += transform.up * moveSpeed;
                if (InputManager.GetKey(KeyCode.LeftControl) || InputManager.GetKey(KeyCode.PageDown))
                    transform.position += transform.up * -1 * moveSpeed;
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                float newRotationX = transform.localEulerAngles.y + mouseX;
                float newRotationY = transform.localEulerAngles.x - mouseY;
                transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
            }
        }
    }
}

namespace FreecamPauseMod
{
    [BepInPlugin("vainstar.FreecamPause", "FreecamPause", "1.0.0")]
    public class FreecamPausePlugin : BaseUnityPlugin
    {
        ConfigEntry<KeyCode> freecamPauseKey;
        FreecamMod.Freecam freecam;
        GameObject ui;
        bool sceneReady;

        void Awake()
        {
            freecamPauseKey = Config.Bind("Keybinds", "FreecamPauseKey", KeyCode.G, "Key that pauses, toggles freecam, and hides UI.");
            SceneManager.activeSceneChanged += OnSceneChanged;
            CacheForScene(SceneManager.GetActiveScene());
        }

        void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        void Update()
        {
            if (!sceneReady)
            {
                if (Time.timeScale != 1f)
                    Time.timeScale = 1f;
                return;
            }
            EnsureReferences();
            if (freecam == null)
                return;
            if (UnityInput.Current.GetKeyDown(freecamPauseKey.Value))
                Toggle();
        }

        void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            CacheForScene(newScene);
        }

        void CacheForScene(Scene scene)
        {
            sceneReady = scene.name != "Intro" && scene.name != "Menu";
            if (!sceneReady)
            {
                if (freecam != null && freecam.freecamActive)
                    freecam.setActive(false);
                if (Time.timeScale != 1f)
                    Time.timeScale = 1f;
                freecam = null;
                ui = null;
                return;
            }
            EnsureReferences();
        }

        void EnsureReferences()
        {
            if (!sceneReady)
                return;
            if (freecam == null)
            {
                vThirdPersonCamera cam = FindObjectOfType<vThirdPersonCamera>();
                if (cam != null)
                    freecam = cam.gameObject.GetComponent<FreecamMod.Freecam>() ?? cam.gameObject.AddComponent<FreecamMod.Freecam>();
            }
            if (ui == null)
                ui = GameObject.Find("UI/Stuff/UI/ui/StandardUI");
        }

        void Toggle()
        {
            bool activating = !freecam.freecamActive;
            freecam.setActive(activating);
            if (activating)
            {
                Time.timeScale = 0f;
                DisableUI();
            }
            else
            {
                Time.timeScale = 1f;
                EnableUI();
            }
        }

        void EnableUI()
        {
            if (ui != null)
                ui.SetActive(true);
        }

        void DisableUI()
        {
            if (ui != null)
                ui.SetActive(false);
        }
    }
}
