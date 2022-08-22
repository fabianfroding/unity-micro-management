using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Slider playerSlider3D;
    Slider playerSlider2D;
    public int health;

    Stats statsScript;

    void Start()
    {
        statsScript = GameObject.FindGameObjectWithTag(EditorConstants.TAG_PLAYER).GetComponent<Stats>(); 

        playerSlider2D = GetComponent<Slider>();

        playerSlider2D.maxValue = statsScript.maxHealth;
        playerSlider3D.maxValue = statsScript.maxHealth;
        statsScript.health = statsScript.maxHealth;
    }

    void Update()
    {
        playerSlider2D.value = statsScript.health;
        playerSlider3D.value = playerSlider2D.value;
    }
}
