//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

////Put your actions in this enum
//public enum KeybindingActions
//{
//    Forward,
//    Backward,
//    Jump
//}
//public class InputManager : MonoBehaviour
//{
//    #region Singleton Design
//    private static InputManager _instance;

//    public static InputManager Instance { get { return _instance; } }


//    private void Awake()
//    {
//        if (_instance != null && _instance != this)
//        {
//            Destroy(this.gameObject);
//        }
//        else
//        {
//            _instance = this;
//        }
//        DontDestroyOnLoad(this);
//    }
//    #endregion

//    [SerializeField] public Keybinds keybinds;

//    public KeyCode GetKeyForAction(KeybindingActions keybindingAction)
//    {
//        foreach(Keybinds.KeybindingCheck keybindingCheck in keybinds.keybindingChecks)
//        {
//            if(keybindingCheck.keybindingAction == keybindingAction)
//            {
//                return keybindingCheck.keyCode;
//            } 
//        }
//        return KeyCode.None;
//    }

//    public bool GetKeyDown(KeybindingActions key)
//    {
//        foreach (Keybinds.KeybindingCheck keybindingCheck in keybinds.keybindingChecks)
//        {
//            if (keybindingCheck.keybindingAction == key)
//            {
//                return Input.GetKeyDown(keybindingCheck.keyCode);
//            }
//        }
//        return false;
//    }

//    public BitArray GetKeys() 
//    {
//        BitArray flags = new BitArray(ECS.Components.InputComponent.keybindingChecks.Count);
//        for(int i = 0; i < ECS.Components.InputComponent.keybindingChecks.Count; i++)
//        {
//            if(Input.GetKey(ECS.Components.InputComponent.keybindingChecks[i].keyCode))
//            {
//                flags[i] = true;
//            }
//        }
//        return flags;
//    }

//    public bool GetKeyUp(KeybindingActions key)
//    {
//        foreach (Keybinds.KeybindingCheck keybindingCheck in keybinds.keybindingChecks)
//        {
//            if (keybindingCheck.keybindingAction == key)
//            {
//                return Input.GetKeyUp(keybindingCheck.keyCode);
//            }
//        }
//        return false;
//    }

//    public bool GetKey(KeybindingActions key)
//    {
//        foreach (Keybinds.KeybindingCheck keybindingCheck in keybinds.keybindingChecks)
//        {
//            if (keybindingCheck.keybindingAction == key)
//            {
//                return Input.GetKey(keybindingCheck.keyCode);
//            }
//        }
//        return false;
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
