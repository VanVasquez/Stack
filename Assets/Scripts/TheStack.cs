using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour
{
    public CameraShake cameraShake; // Reference to the CameraShake script
    public Text scoreText;
    public GameObject tutorialText;
    public Material stackMat;
    public GameObject savePanel; 
    public ColorManager colorManager; // Reference to the ColorManager script 
    public GameObject fog;
    public AudioSource src;
    public AudioClip sfx1;
    public AudioClip sfx2;

    private const float BOUND_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.1f;
    private const float STACK_BOUNDS_GAIN = 0.1f;
    private const int COMBO_START_GAIN = 3;


    private GameObject[] theStack;
    private Vector2 stackBounds = new Vector2(BOUND_SIZE, BOUND_SIZE);

    private int stackIndex;
    public int scoreCount = 0;
    private int combo = 0;

    private float tileTransition = -5.0f;
    private float tileSpeed = 1.5f;
    private float secondaryPosition;

    private bool isMovingOnX = false;
    private bool isGameOver = false;
    private bool isFogDisplayed = false;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;


    private void Start()
    {
        theStack = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            theStack[i] = transform.GetChild(i).gameObject;
            ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
        }
        stackIndex = transform.childCount - 1;
    }

    private void Update()
    {
        if (isGameOver) return;
         
        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceTile())
            {
                SpawnTile();
                scoreCount++;
                scoreText.text = scoreCount.ToString();

                if(scoreCount >= 10 && Random.value > 0.5f)
                {

                    float minDuration = 0.1f;
                    float maxDuration = 1.5f;
                    float minMagnitude = 0.1f;
                    float maxMagnitude = 0.5f;

                    cameraShake.StartRandomShake(minDuration, maxDuration, minMagnitude, maxMagnitude);
                }
            }
            else
            {
                EndGame();
            }
        }
        MoveTile();

        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);

        if(scoreCount == 30 && !isFogDisplayed)
        {
            AddFog();
        }
    }

    private IEnumerator StopSoundAfterDelay(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
    }

    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

        src.clip = sfx1;
        src.PlayOneShot(sfx1);
        StartCoroutine(StopSoundAfterDelay(src, 0.5f));

        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();

        go.GetComponent<MeshRenderer>().material = stackMat;
        ColorMesh(go.GetComponent<MeshFilter>().mesh);
    }
    
    private void MoveTile()
    { 
        tileTransition += Time.deltaTime * tileSpeed;
        if(isMovingOnX)
        { 
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUND_SIZE * 1.5f,scoreCount, secondaryPosition);
        }
        else
        {
            theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUND_SIZE * 1.5f);
        }
    }

    private void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;
        stackIndex--;
        if (stackIndex < 0)
        {
            stackIndex = transform.childCount - 1;
        }
        desiredPosition = (Vector3.down) * scoreCount;

        if (isMovingOnX)
        {
            theStack[stackIndex].transform.localPosition = new Vector3(-BOUND_SIZE * 1.5f, scoreCount, 0);
        }
        else
        {
            theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, BOUND_SIZE * 1.5f);
        }
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);
    }
    
    private bool PlaceTile()
    {
        Transform t = theStack[stackIndex].transform;
        tutorialText.SetActive(false);
        if (isMovingOnX)
        {
            float deltaX = lastTilePosition.x - t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                //Cut the tile
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                    return false;
                float middle = lastTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                CreateRubble(
                    new Vector3(
                        (t.position.x > 0)
                        ? t.position.x + (t.localScale.x / 2)
                        : t.position.x - (t.localScale.x / 2), 
                        t.position.y, 
                        t.position.z
                    ), 
                    new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z)
                );
                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
            }
            else
            {
                if(combo > COMBO_START_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;
                    if (stackBounds.x > BOUND_SIZE)
                        stackBounds.x = BOUND_SIZE;
                    float middle = lastTilePosition.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
                }
                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);

                src.clip = sfx2;
                src.Play();
            }
        }
        else
        {
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                //Cut the tile
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;
                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y); 
                CreateRubble(
                    new Vector3(
                        t.position.x,
                        t.position.y, 
                        (t.position.z > 0)
                        ? t.position.z + (t.localScale.z / 2)
                        : t.position.z - (t.localScale.z / 2)
                    ),
                    new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ))
                );
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.y += STACK_BOUNDS_GAIN;
                    if (stackBounds.y > BOUND_SIZE)
                        stackBounds.y = BOUND_SIZE;
                    float middle = lastTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                }
                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);

                src.clip = sfx2;
                src.Play();
            }
        }

        secondaryPosition = (isMovingOnX)
            ? t.localPosition.x
            : t.localPosition.z;

        isMovingOnX = !isMovingOnX;

        tileTransition = (isMovingOnX ? 5.0f : -5.0f);

        return true;
    }

    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(scoreCount * 0.25f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = colorManager.Lerp4(colorManager.gameColors[0], colorManager.gameColors[1], colorManager.gameColors[2], colorManager.gameColors[3], f);
        }
        mesh.colors32 = colors;
    }

    private void AddFog()
    {
        isFogDisplayed = true;
        Instantiate(fog, new Vector3(1.79999995f, -0.899999976f, -0.699999988f), new Quaternion(-0.223812401f, 0.565404415f, 0.776981473f, 0.162866831f));
    }

    private void EndGame()
    {
        if (PlayerPrefs.GetInt("score") < scoreCount)
            PlayerPrefs.SetInt("score", scoreCount);

        Debug.Log("LOSE");
        isGameOver = true;
        savePanel.SetActive(true);
        theStack[stackIndex].AddComponent<Rigidbody>();
    }

    public void OnButtonClick(string sceneName)
    {
       SceneManager.LoadScene(sceneName);
    }
}
