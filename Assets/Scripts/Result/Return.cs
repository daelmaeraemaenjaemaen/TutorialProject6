using UnityEngine;
using UnityEngine.SceneManagement;

public class Return : MonoBehaviour
{
   // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("2_MusicSelect");
    }
}
