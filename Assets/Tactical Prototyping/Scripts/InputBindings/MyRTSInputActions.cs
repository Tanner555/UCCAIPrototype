// GENERATED AUTOMATICALLY FROM 'Assets/Tactical Prototyping/SaveData/Inputs/MyRTSInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace RTSPrototype
{
    public class @MyRTSInputActions : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @MyRTSInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""MyRTSInputActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""db60a659-2bd8-4b69-8823-bff532b3d6e4"",
            ""actions"": [
                {
                    ""name"": ""PauseGame"",
                    ""type"": ""Button"",
                    ""id"": ""2a34802d-ea03-4ec9-9fa7-01992bb5c325"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""IGBPIMenuToggle"",
                    ""type"": ""Button"",
                    ""id"": ""adc8d5be-b527-4acb-84b2-2495c87345d5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NumberKeys"",
                    ""type"": ""Button"",
                    ""id"": ""d6fea892-a85f-4aea-87b1-c90bb4868aac"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PossessAllyAdd"",
                    ""type"": ""Button"",
                    ""id"": ""4150e910-4eda-4ca6-a76c-c97b325fea5e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PossessAllySubtract"",
                    ""type"": ""Button"",
                    ""id"": ""a5df70b5-04eb-4c2d-914c-a225fd48105e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2450b551-4ef8-42d2-8828-15aefd456efb"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseGame"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d4db5585-4e21-4bf5-a275-b43e5f5e2300"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""IGBPIMenuToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""236b7cf9-d15c-4bda-a80a-ebf63abe131e"",
                    ""path"": ""<Keyboard>/0"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0c89df2-68fa-45ca-8659-76a4795dca8f"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""10acb38d-dff4-48ae-81c2-6f4b6de18290"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ebd58bd-7e5a-4cd6-a12d-8170aa22f06e"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""31433cfe-6622-4391-b4e8-19e68423d546"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b85767bd-5bae-41e6-9391-1ed53e253eff"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""085a2bba-eb45-488c-9c76-553f2342fbaa"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b98a0018-9164-4194-aa9c-c47de5bcd878"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cf0ba8e4-2c34-43a2-8818-b9a866e8c808"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23e2f3ae-48f0-4fa9-b004-13f3ad44c00a"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NumberKeys"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b70ab07-1757-4213-930f-98ba3bd55964"",
                    ""path"": ""<Keyboard>/numpad1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PossessAllyAdd"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7bb96a62-b86b-484c-bbb4-401cdc14bbce"",
                    ""path"": ""<Keyboard>/numpad3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PossessAllySubtract"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Gameplay
            m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_PauseGame = m_Gameplay.FindAction("PauseGame", throwIfNotFound: true);
            m_Gameplay_IGBPIMenuToggle = m_Gameplay.FindAction("IGBPIMenuToggle", throwIfNotFound: true);
            m_Gameplay_NumberKeys = m_Gameplay.FindAction("NumberKeys", throwIfNotFound: true);
            m_Gameplay_PossessAllyAdd = m_Gameplay.FindAction("PossessAllyAdd", throwIfNotFound: true);
            m_Gameplay_PossessAllySubtract = m_Gameplay.FindAction("PossessAllySubtract", throwIfNotFound: true);
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

        // Gameplay
        private readonly InputActionMap m_Gameplay;
        private IGameplayActions m_GameplayActionsCallbackInterface;
        private readonly InputAction m_Gameplay_PauseGame;
        private readonly InputAction m_Gameplay_IGBPIMenuToggle;
        private readonly InputAction m_Gameplay_NumberKeys;
        private readonly InputAction m_Gameplay_PossessAllyAdd;
        private readonly InputAction m_Gameplay_PossessAllySubtract;
        public struct GameplayActions
        {
            private @MyRTSInputActions m_Wrapper;
            public GameplayActions(@MyRTSInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @PauseGame => m_Wrapper.m_Gameplay_PauseGame;
            public InputAction @IGBPIMenuToggle => m_Wrapper.m_Gameplay_IGBPIMenuToggle;
            public InputAction @NumberKeys => m_Wrapper.m_Gameplay_NumberKeys;
            public InputAction @PossessAllyAdd => m_Wrapper.m_Gameplay_PossessAllyAdd;
            public InputAction @PossessAllySubtract => m_Wrapper.m_Gameplay_PossessAllySubtract;
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
                {
                    @PauseGame.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseGame;
                    @PauseGame.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseGame;
                    @PauseGame.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPauseGame;
                    @IGBPIMenuToggle.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnIGBPIMenuToggle;
                    @IGBPIMenuToggle.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnIGBPIMenuToggle;
                    @IGBPIMenuToggle.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnIGBPIMenuToggle;
                    @NumberKeys.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnNumberKeys;
                    @NumberKeys.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnNumberKeys;
                    @NumberKeys.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnNumberKeys;
                    @PossessAllyAdd.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossessAllyAdd;
                    @PossessAllyAdd.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossessAllyAdd;
                    @PossessAllyAdd.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossessAllyAdd;
                    @PossessAllySubtract.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossessAllySubtract;
                    @PossessAllySubtract.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossessAllySubtract;
                    @PossessAllySubtract.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnPossessAllySubtract;
                }
                m_Wrapper.m_GameplayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @PauseGame.started += instance.OnPauseGame;
                    @PauseGame.performed += instance.OnPauseGame;
                    @PauseGame.canceled += instance.OnPauseGame;
                    @IGBPIMenuToggle.started += instance.OnIGBPIMenuToggle;
                    @IGBPIMenuToggle.performed += instance.OnIGBPIMenuToggle;
                    @IGBPIMenuToggle.canceled += instance.OnIGBPIMenuToggle;
                    @NumberKeys.started += instance.OnNumberKeys;
                    @NumberKeys.performed += instance.OnNumberKeys;
                    @NumberKeys.canceled += instance.OnNumberKeys;
                    @PossessAllyAdd.started += instance.OnPossessAllyAdd;
                    @PossessAllyAdd.performed += instance.OnPossessAllyAdd;
                    @PossessAllyAdd.canceled += instance.OnPossessAllyAdd;
                    @PossessAllySubtract.started += instance.OnPossessAllySubtract;
                    @PossessAllySubtract.performed += instance.OnPossessAllySubtract;
                    @PossessAllySubtract.canceled += instance.OnPossessAllySubtract;
                }
            }
        }
        public GameplayActions @Gameplay => new GameplayActions(this);
        public interface IGameplayActions
        {
            void OnPauseGame(InputAction.CallbackContext context);
            void OnIGBPIMenuToggle(InputAction.CallbackContext context);
            void OnNumberKeys(InputAction.CallbackContext context);
            void OnPossessAllyAdd(InputAction.CallbackContext context);
            void OnPossessAllySubtract(InputAction.CallbackContext context);
        }
    }
}
