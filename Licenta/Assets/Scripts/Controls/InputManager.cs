// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Controls/InputManager.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputManager : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputManager()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputManager"",
    ""maps"": [
        {
            ""name"": ""UI"",
            ""id"": ""440c2db3-b522-413f-9c6f-823b915ae3da"",
            ""actions"": [
                {
                    ""name"": ""Open map"",
                    ""type"": ""Button"",
                    ""id"": ""d2b92ea9-0b12-46c2-90fa-f8fa47be348a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Map zoom"",
                    ""type"": ""Value"",
                    ""id"": ""877e87a4-57fa-46dc-842d-cead3f49d08f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""Normalize(min=-120,max=120)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Show/Hide minimap"",
                    ""type"": ""Button"",
                    ""id"": ""c0cbbcfd-3c2a-4725-9751-0ecb1f4de11e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Grab map"",
                    ""type"": ""Button"",
                    ""id"": ""0023138f-a218-4a57-8d85-c48b2735e95f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""7bc0b244-d3aa-4da5-b589-0bc36b467464"",
                    ""path"": ""<Keyboard>/o"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Show/Hide minimap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de910ae0-d544-45fd-828c-c2804cf3321b"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Open map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9cb4c6c8-2e4b-4e8a-b21f-9bc8d8072a49"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Map zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e578e094-8967-4948-a1a0-fb11cc7a042b"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Grab map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Others"",
            ""id"": ""3b603769-37af-438f-9a9e-ba84173a515c"",
            ""actions"": [
                {
                    ""name"": ""Restart Game"",
                    ""type"": ""Button"",
                    ""id"": ""f30d97bc-ccab-40e2-8adc-832697e282bd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pause Game"",
                    ""type"": ""Button"",
                    ""id"": ""b8f33bda-c84f-4593-959e-059b1f7c7dac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""9500bcd5-155a-410d-9130-0b901f687d01"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Restart Game"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de4f073b-1f71-4196-9ffe-13c110f86d80"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause Game"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9b394d2d-4f42-4a64-b499-d7a188b3a9eb"",
                    ""path"": ""<XInputController>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause Game"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player Movement"",
            ""id"": ""e6fd53c5-cdc3-4478-be30-41e1ede25f0a"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""be8a8cc2-231f-4e9d-9aba-a86f7e9c68e1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""f9ffc281-93e3-4f59-8e0e-0b119876421f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": ""Hold""
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Button"",
                    ""id"": ""5d863fee-16c6-4c8a-ab40-0927fc5b9c64"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crawl"",
                    ""type"": ""Button"",
                    ""id"": ""ef94e375-2893-4ac7-b53f-0413bb0c3854"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sneak"",
                    ""type"": ""Button"",
                    ""id"": ""23f9c744-8235-464e-b18c-290c731fcb3b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""4c4cb9b1-3b05-4bd2-9e5e-6065aaca6265"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Alternate move"",
                    ""type"": ""Button"",
                    ""id"": ""5fa93342-371c-4f7b-ad4c-0a01222f4640"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": ""Hold""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""924b8e66-1683-4b1d-8cea-893ea211bd53"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a30f7e27-d36a-4dea-b5f3-1bb306beeac5"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b44f6990-b201-4ad1-b0b6-1b3c997cb18a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9103f81e-70ba-42c2-a6ef-1ecb191bdb0f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""db98542f-4be4-4462-8617-c5883242c616"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e1ecf12a-5305-4b53-985d-9931e3284d38"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone"",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""120fbd36-0e25-4ebd-9664-fc669fe33b7c"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84afe528-6f1f-4b51-a69f-c6866f7c5fe6"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""11506265-b0d9-4f94-961b-11829838df96"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1a4c58f-a0b6-4190-9035-83f9545a755c"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ded1b1ff-0aad-4069-b8d7-6ed6b47d80bd"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crawl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6cd5330d-e604-48b9-8494-6168cb6d5b89"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crawl"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""32bac46d-2f0a-44ac-9d16-af660221bef4"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sneak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""48305520-85e7-4e41-bcf6-efe0e2eab36f"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sneak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""389ec09f-b3e8-45e1-962b-8c792e1a964e"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e114b550-9184-45eb-bffa-68e824605ffe"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""292d9a11-6224-44a7-a48b-dac9bafcec3e"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Alternate move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Openmap = m_UI.FindAction("Open map", throwIfNotFound: true);
        m_UI_Mapzoom = m_UI.FindAction("Map zoom", throwIfNotFound: true);
        m_UI_ShowHideminimap = m_UI.FindAction("Show/Hide minimap", throwIfNotFound: true);
        m_UI_Grabmap = m_UI.FindAction("Grab map", throwIfNotFound: true);
        // Others
        m_Others = asset.FindActionMap("Others", throwIfNotFound: true);
        m_Others_RestartGame = m_Others.FindAction("Restart Game", throwIfNotFound: true);
        m_Others_PauseGame = m_Others.FindAction("Pause Game", throwIfNotFound: true);
        // Player Movement
        m_PlayerMovement = asset.FindActionMap("Player Movement", throwIfNotFound: true);
        m_PlayerMovement_Move = m_PlayerMovement.FindAction("Move", throwIfNotFound: true);
        m_PlayerMovement_Run = m_PlayerMovement.FindAction("Run", throwIfNotFound: true);
        m_PlayerMovement_Dodge = m_PlayerMovement.FindAction("Dodge", throwIfNotFound: true);
        m_PlayerMovement_Crawl = m_PlayerMovement.FindAction("Crawl", throwIfNotFound: true);
        m_PlayerMovement_Sneak = m_PlayerMovement.FindAction("Sneak", throwIfNotFound: true);
        m_PlayerMovement_Jump = m_PlayerMovement.FindAction("Jump", throwIfNotFound: true);
        m_PlayerMovement_Alternatemove = m_PlayerMovement.FindAction("Alternate move", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_Openmap;
    private readonly InputAction m_UI_Mapzoom;
    private readonly InputAction m_UI_ShowHideminimap;
    private readonly InputAction m_UI_Grabmap;
    public struct UIActions
    {
        private @InputManager m_Wrapper;
        public UIActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Openmap => m_Wrapper.m_UI_Openmap;
        public InputAction @Mapzoom => m_Wrapper.m_UI_Mapzoom;
        public InputAction @ShowHideminimap => m_Wrapper.m_UI_ShowHideminimap;
        public InputAction @Grabmap => m_Wrapper.m_UI_Grabmap;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @Openmap.started -= m_Wrapper.m_UIActionsCallbackInterface.OnOpenmap;
                @Openmap.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnOpenmap;
                @Openmap.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnOpenmap;
                @Mapzoom.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMapzoom;
                @Mapzoom.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMapzoom;
                @Mapzoom.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMapzoom;
                @ShowHideminimap.started -= m_Wrapper.m_UIActionsCallbackInterface.OnShowHideminimap;
                @ShowHideminimap.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnShowHideminimap;
                @ShowHideminimap.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnShowHideminimap;
                @Grabmap.started -= m_Wrapper.m_UIActionsCallbackInterface.OnGrabmap;
                @Grabmap.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnGrabmap;
                @Grabmap.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnGrabmap;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Openmap.started += instance.OnOpenmap;
                @Openmap.performed += instance.OnOpenmap;
                @Openmap.canceled += instance.OnOpenmap;
                @Mapzoom.started += instance.OnMapzoom;
                @Mapzoom.performed += instance.OnMapzoom;
                @Mapzoom.canceled += instance.OnMapzoom;
                @ShowHideminimap.started += instance.OnShowHideminimap;
                @ShowHideminimap.performed += instance.OnShowHideminimap;
                @ShowHideminimap.canceled += instance.OnShowHideminimap;
                @Grabmap.started += instance.OnGrabmap;
                @Grabmap.performed += instance.OnGrabmap;
                @Grabmap.canceled += instance.OnGrabmap;
            }
        }
    }
    public UIActions @UI => new UIActions(this);

    // Others
    private readonly InputActionMap m_Others;
    private IOthersActions m_OthersActionsCallbackInterface;
    private readonly InputAction m_Others_RestartGame;
    private readonly InputAction m_Others_PauseGame;
    public struct OthersActions
    {
        private @InputManager m_Wrapper;
        public OthersActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @RestartGame => m_Wrapper.m_Others_RestartGame;
        public InputAction @PauseGame => m_Wrapper.m_Others_PauseGame;
        public InputActionMap Get() { return m_Wrapper.m_Others; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(OthersActions set) { return set.Get(); }
        public void SetCallbacks(IOthersActions instance)
        {
            if (m_Wrapper.m_OthersActionsCallbackInterface != null)
            {
                @RestartGame.started -= m_Wrapper.m_OthersActionsCallbackInterface.OnRestartGame;
                @RestartGame.performed -= m_Wrapper.m_OthersActionsCallbackInterface.OnRestartGame;
                @RestartGame.canceled -= m_Wrapper.m_OthersActionsCallbackInterface.OnRestartGame;
                @PauseGame.started -= m_Wrapper.m_OthersActionsCallbackInterface.OnPauseGame;
                @PauseGame.performed -= m_Wrapper.m_OthersActionsCallbackInterface.OnPauseGame;
                @PauseGame.canceled -= m_Wrapper.m_OthersActionsCallbackInterface.OnPauseGame;
            }
            m_Wrapper.m_OthersActionsCallbackInterface = instance;
            if (instance != null)
            {
                @RestartGame.started += instance.OnRestartGame;
                @RestartGame.performed += instance.OnRestartGame;
                @RestartGame.canceled += instance.OnRestartGame;
                @PauseGame.started += instance.OnPauseGame;
                @PauseGame.performed += instance.OnPauseGame;
                @PauseGame.canceled += instance.OnPauseGame;
            }
        }
    }
    public OthersActions @Others => new OthersActions(this);

    // Player Movement
    private readonly InputActionMap m_PlayerMovement;
    private IPlayerMovementActions m_PlayerMovementActionsCallbackInterface;
    private readonly InputAction m_PlayerMovement_Move;
    private readonly InputAction m_PlayerMovement_Run;
    private readonly InputAction m_PlayerMovement_Dodge;
    private readonly InputAction m_PlayerMovement_Crawl;
    private readonly InputAction m_PlayerMovement_Sneak;
    private readonly InputAction m_PlayerMovement_Jump;
    private readonly InputAction m_PlayerMovement_Alternatemove;
    public struct PlayerMovementActions
    {
        private @InputManager m_Wrapper;
        public PlayerMovementActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerMovement_Move;
        public InputAction @Run => m_Wrapper.m_PlayerMovement_Run;
        public InputAction @Dodge => m_Wrapper.m_PlayerMovement_Dodge;
        public InputAction @Crawl => m_Wrapper.m_PlayerMovement_Crawl;
        public InputAction @Sneak => m_Wrapper.m_PlayerMovement_Sneak;
        public InputAction @Jump => m_Wrapper.m_PlayerMovement_Jump;
        public InputAction @Alternatemove => m_Wrapper.m_PlayerMovement_Alternatemove;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMovementActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMovementActions instance)
        {
            if (m_Wrapper.m_PlayerMovementActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Run.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRun;
                @Dodge.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDodge;
                @Dodge.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDodge;
                @Dodge.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDodge;
                @Crawl.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCrawl;
                @Crawl.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCrawl;
                @Crawl.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCrawl;
                @Sneak.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSneak;
                @Sneak.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSneak;
                @Sneak.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnSneak;
                @Jump.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Alternatemove.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnAlternatemove;
                @Alternatemove.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnAlternatemove;
                @Alternatemove.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnAlternatemove;
            }
            m_Wrapper.m_PlayerMovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
                @Dodge.started += instance.OnDodge;
                @Dodge.performed += instance.OnDodge;
                @Dodge.canceled += instance.OnDodge;
                @Crawl.started += instance.OnCrawl;
                @Crawl.performed += instance.OnCrawl;
                @Crawl.canceled += instance.OnCrawl;
                @Sneak.started += instance.OnSneak;
                @Sneak.performed += instance.OnSneak;
                @Sneak.canceled += instance.OnSneak;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Alternatemove.started += instance.OnAlternatemove;
                @Alternatemove.performed += instance.OnAlternatemove;
                @Alternatemove.canceled += instance.OnAlternatemove;
            }
        }
    }
    public PlayerMovementActions @PlayerMovement => new PlayerMovementActions(this);
    public interface IUIActions
    {
        void OnOpenmap(InputAction.CallbackContext context);
        void OnMapzoom(InputAction.CallbackContext context);
        void OnShowHideminimap(InputAction.CallbackContext context);
        void OnGrabmap(InputAction.CallbackContext context);
    }
    public interface IOthersActions
    {
        void OnRestartGame(InputAction.CallbackContext context);
        void OnPauseGame(InputAction.CallbackContext context);
    }
    public interface IPlayerMovementActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
        void OnDodge(InputAction.CallbackContext context);
        void OnCrawl(InputAction.CallbackContext context);
        void OnSneak(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnAlternatemove(InputAction.CallbackContext context);
    }
}
