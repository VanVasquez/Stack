using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    [SerializeField] InputField nameInput;
    [SerializeField] Text score;
    [SerializeField] string filename;
    public GameObject savePanel;
    public GameObject endPanel; 

    List<PlayerData> entries = new List<PlayerData>();

    private void Start()
    {
        entries = FileHandler.ReadListFromJSON<PlayerData>(filename);
    }

    public void AddNameToList()
    {
        entries.Add(new PlayerData(nameInput.text, int.Parse(score.text)));
        nameInput.text = "";

        FileHandler.SaveToJSON<PlayerData>(entries, filename);

        savePanel.SetActive(false);
        endPanel.SetActive(true);
    }
}