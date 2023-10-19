using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public Text[] text;                 // Text elements used for UI display.
    public Vector2 vec2DAxis_L = Vector2.zero;   // 2D vector for left hand joystick input.
    public Vector2 vec2DAxis_R = Vector2.zero;   // 2D vector for right hand joystick input.
    public bool isGrip_L = false;       // Boolean indicating if the left hand grip button is pressed.
    private bool DownTrigger_L;
    private bool DownTrigger_R;
    public bool isTrigger_L = false;   // Boolean indicating if the left hand trigger button is pressed.
    public bool isGrip_R = false;       // Boolean indicating if the right hand grip button is pressed.
    public bool isTrigger_R = false;   // Boolean indicating if the right hand trigger button is pressed.
    public GameObject Hand_L;          // Game object representing the left hand.
    public GameObject Hand_R;          // Game object representing the right hand.
    public int I;                       // Integer used for controlling game state.
    public float time = 10;             // Timer for a specific game event.
    public float time2 = 300;           // Another timer for a different game event.
    public CharacterController character;  // Character controller for player movement.
    public GameObject Light;            // Game object representing a light source.
    public GameObject Light2;           // Game object representing another light source.
    private bool door = true;           // Boolean for controlling a door.
    public bool dooring;               // Boolean for controlling a door animation.
    public GameObject Door;            // Game object representing a door.
    public GameObject Door2;           // Another game object representing a door.
    public GameObject Door3;           // Yet another game object representing a door.
    public GameObject mm;              // Game object used for a specific purpose.
    public GameObject canvas;          // Game object representing a canvas for UI display.
    public int[] ints = new int[4];     // An array of integers for a specific purpose.
    public int[] ints2 = new int[4];    // Another array of integers for a specific purpose.
    public int i;                       // Integer used for another purpose.
    public int t = 10;                  // Integer with an initial value of 10.
    public AudioSource source;          // Audio source component.
    public AudioSource source2;         // Another audio source component.
    public AudioSource BGM;             // Audio source for background music.
    private float time3;                // Timer for a specific event.
    public Light L;                     // Light component.
    public Transform hand_L;            // Transform of the left hand.
    public Transform hand_R;            // Transform of the right hand.
    public Transform Pool;              // Transform used for a specific purpose.
    
    void Start()
    {
        // Code executed on the start of the game.
        Light.SetActive(false);
        
    }
    
    // Update is called once per frame
    void Update()
    {
        // Code executed in each frame update
        
        // Get input from left hand XR controller
        
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out vec2DAxis_L);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.gripButton, out isGrip_L);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.triggerButton, out isTrigger_L);
        
        Ray(Hand_L);
        
        if (time == 0)
        {
            // Enable background music
            BGM.enabled = true;
            
            // Decrement the time2 variable by Time.deltaTime
            time2 -= Time.deltaTime;
            
            // Update the text displaying the time
            text[1].text = (((int)time2) / 60).ToString() + ":" + (((int)time2) % 60).ToString();
            
            // Adjust the color of a light source based on time2
            
            if (time2 > 180)
            {
                L.color = Color.white;
            }else if (time2 <= 180 && time2 > 60)
            {
                L.color = Color.yellow;
            }
            else
            {
                L.color = Color.red;
            }
            // When time2 reaches below 0, perform certain actions
            if (time2 < 0)
            {
                I = 99;
                transform.Find("Camera").localEulerAngles = new Vector3(0, transform.Find("Main Camera").localEulerAngles.y * 1.5f, 0);
                transform.Find("Camera").position = new Vector3(transform.Find("Main Camera").position.x, 0, transform.Find("Main Camera").position.z);
                time2 = 0;
                BGM.enabled = false;
            }
            
        }
        if (dooring)
        {
            // If 'door' is also true, control the door opening
            if (door)
            {
                if (Door.transform.eulerAngles.y > 90)
                {
                    Door.transform.eulerAngles = new Vector3(0, Door.transform.eulerAngles.y - Time.deltaTime * 60, 0);
                }
                else
                {
                    Door.transform.eulerAngles = new Vector3(0, 90, 0);
                    door = false;
                    dooring = false;
                }
                
            }
            else
            {
                if (Door.transform.eulerAngles.y < 180)
                {
                    Door.transform.eulerAngles = new Vector3(0, Door.transform.eulerAngles.y + Time.deltaTime * 30, 0);
                }
                else
                {
                    Door.transform.eulerAngles = new Vector3(0, 180, 0);
                    door = true;
                    dooring = false;
                }
            }
        }
        // Check if both 'I' and 'i' are not equal to 0
        if (I != 0 && i != 99)
        {
            // Call the 'Move' method to handle character movement
            Move();
        }
        // Use a switch statement to handle different game states based on 'I'
        
        switch (I)
        {
            case 0:
                // Handle the initial countdown and UI display
                if (time > 0)
                {
                    text[0].gameObject.SetActive(true);
                    time -= Time.deltaTime;
                    text[0].text = ((int)time+1).ToString();
                }
                else
                {
                    text[0].gameObject.SetActive(false);
                    I = 1;
                    time = 0;
                    text[2].gameObject.SetActive(true);
                }
                break;
            case 1:
                if(OnTrigger("Light"))
                {
                    Light.SetActive(true);
                    text[3].gameObject.SetActive(true);
                    I = 88;
                }
                break;
            case 88:
                // Handle interactions with objects associated with "cup"
                Collider[] outputCols_L = Physics.OverlapSphere(hand_L.position, 0.2f, LayerMask.GetMask("cup"));
                Collider[] outputCols_R = Physics.OverlapSphere(hand_R.position, 0.2f, LayerMask.GetMask("cup"));
                if (outputCols_L.Length > 0 && isGrip_L)
                {
                    outputCols_L[0].GetComponent<Rigidbody>().isKinematic = true;
                    outputCols_L[0].transform.localPosition = Vector3.zero;
                    outputCols_L[0].transform.localEulerAngles = Vector3.zero;
                    outputCols_L[0].transform.parent = hand_L;
                }
                if (outputCols_R.Length > 0 && isGrip_R)
                {
                    outputCols_R[0].GetComponent<Rigidbody>().isKinematic = true;
                    outputCols_R[0].transform.localPosition = Vector3.zero;
                    outputCols_R[0].transform.localEulerAngles = Vector3.zero;
                    outputCols_R[0].transform.parent = hand_R;
                }
                if(hand_L.childCount > 0)
                {
                    hand_L.GetChild(0).localPosition = Vector3.zero;
                    hand_L.GetChild(0).localEulerAngles = Vector3.zero;
                    hand_L.parent.Find("Neo2_L").gameObject.SetActive(false);
                    hand_L.parent.Find("Neo2_L").gameObject.SetActive(false);
                }
                else
                {
                    hand_L.parent.Find("Neo2_L").gameObject.SetActive(true);
                    hand_L.parent.Find("Neo2_L").gameObject.SetActive(true);
                }
                if (hand_R.childCount > 0)
                {
                    hand_R.GetChild(0).localPosition = Vector3.zero;
                    hand_R.GetChild(0).localEulerAngles = Vector3.zero;
                    hand_R.parent.Find("Neo2_R").gameObject.SetActive(false);
                    hand_R.parent.Find("Neo2_R").gameObject.SetActive(false);
                }
                else
                {
                    hand_R.parent.Find("Neo2_R").gameObject.SetActive(true);
                    hand_R.parent.Find("Neo2_R").gameObject.SetActive(true);
                }
                
                if (hand_L.childCount > 0 !&& isGrip_L)
                {
                    hand_L.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
                    hand_L.GetChild(0).transform.parent = null;
                }
                if (hand_R.childCount > 0 && !isGrip_R)
                {
                    hand_R.GetChild(0).GetComponent<Rigidbody>().isKinematic = false;
                    hand_R.GetChild(0).transform.parent = null;
                }
                Collider[] pool = Physics.OverlapBox(Pool.position, Pool.localScale / 2, Pool.rotation, LayerMask.GetMask("cup"));
                if(hand_L.childCount==0&& hand_R.childCount==0&& pool.Length > 0)
                {
                    text[9].gameObject.SetActive(true);
                    I = 2;
                }
                
                
                break;
            case 2:
                if (OnTrigger("FlatTV"))
                {
                    text[4].gameObject.SetActive(true);
                    I = 3;
                }
                break;
            case 3:
                if (OnTrigger("Microwave"))
                {
                    text[5].gameObject.SetActive(true);
                    dooring = true;
                    source.Play();
                    I = 4;
                }
                break;
            case 4:
                if (OnTrigger("Light2"))
                {
                    text[6].gameObject.SetActive(true);
                    Light2.gameObject.SetActive(true);
                    I = 5;
                }
                break;
            case 5:
                if (OnTrigger("wsm"))
                {
                    dooring = true;
                    source.Play();
                }
                if (!dooring && door)
                {
                    I = 6;
                    
                    source.Play();
                }
                break;
            case 6:
                if (Door2.transform.eulerAngles.y > 1)
                {
                    Door2.transform.eulerAngles = new Vector3(0, Door2.transform.eulerAngles.y - Time.deltaTime * 30, 0);
                }
                else
                {
                    Door2.transform.eulerAngles = new Vector3(0, 0, 0);
                    I = 7;
                    mm.SetActive(true);
                }
                break;
            case 7:
                if (ints[0] == ints2[0]&& ints[1] == ints2[1] && ints[2] == ints2[2] && ints[3] == ints2[3])
                {
                    I = 8;
                    transform.Find("Camera").localEulerAngles = new Vector3(0, transform.Find("Main Camera").localEulerAngles.y * 1.5f, 0);
                    transform.Find("Camera").position = new Vector3(transform.Find("Main Camera").position.x, 0, transform.Find("Main Camera").position.z);
                    source.Play();
                }
                break;
            case 8:
                if (Door3.transform.eulerAngles.y < 180)
                {
                    Door3.transform.eulerAngles = new Vector3(0, Door3.transform.eulerAngles.y + Time.deltaTime * 30, 0);
                }
                canvas.SetActive(true);
                text[8].text = "You won!";
                if (OnTrigger("Button"))
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
                break;
            case 99:
                canvas.SetActive(true);
                text[8].text = "You lost...";
                if (OnTrigger("Button"))
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
                break;
            default:
                break;
        }
        MM();
    }
    
    
    // Function to perform a raycast from a hand
    public GameObject Ray(GameObject hand)
    {
        Ray ray = new Ray(hand.transform.position, hand.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            return hitInfo.collider.gameObject;
        }
        return null;
    }
    
    // Function to check if a specific object is being triggered
    public bool OnTrigger(string name)
    {
        if(Ray(Hand_L)!=null)
        {
            if ((isTrigger_L) && Ray(Hand_L).name == name)
            {
                return true;
            }
        }
        if (Ray(Hand_R) != null)
        {
            if ((isTrigger_R) && Ray(Hand_R).name == name)
            {
                return true;
            }
        }
        DownTrigger_L = isTrigger_L;
        return false;
    }
    // Function for handling a combination of inputs
    public void MM()
    {
        if (i > 3)
        {
            i = 3;
        }else if (i < -1)
        {
            i = -1;
        }
        if (time3 >=0)
        {
            time3 -= Time.deltaTime;
        }
        else
        {
            for (int x = 0; x < 10; x++)
            {
                if (OnTrigger(x.ToString()))
                {
                    i++;
                    ints[i] = x;
                    time3 = 0.3f;
                    break;
                }
            }
        }
        
        if (OnTrigger("ss")||Input.GetKeyDown("a"))
        {
            ints[i] = 0;
            i--;
        }
        switch (i)
        {
            case -1:
                text[7].text = null;
                break;
            case 0:
                text[7].text = ints[0].ToString();
                break;
            case 1:
                text[7].text = ints[0].ToString()+ ints[1].ToString();
                break;
            case 2:
                text[7].text = ints[0].ToString() + ints[1].ToString()+ints[2].ToString();
                break;
            case 3:
                text[7].text = ints[0].ToString() + ints[1].ToString() + ints[2].ToString()+ints[3].ToString();
                break;
            default:
                break;
        }
        
    }
    // Function to move the character
    public void Move()
    {
        character.Move(transform.forward * vec2DAxis_L.y*Time.deltaTime + transform.right * vec2DAxis_L.x * Time.deltaTime);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + vec2DAxis_R.x, 0);
        
    }
}
