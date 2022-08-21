using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    [Header("Ability 1")]
    public Image ability1Image;
    public float ability1Cooldown = 5f;
    bool ability1OnCooldown;
    public KeyCode ability1KeyCode;

    Vector3 position;
    public Canvas ability1Canvas;
    public Image ability1Indicator;
    public Transform player;

    [Header("Ability 2")]
    public Image ability2Image;
    public float ability2Cooldown = 10f;
    bool ability2OnCooldown;
    public KeyCode ability2KeyCode;

    public Canvas ability2Canvas;
    public Image ability2AoETargetIndicator;
    public Image ability2RangeIndicator;
    private Vector3 posUp;
    public float ability2MaxDistance;

    [Header("Ability 3")]
    public Image ability3Image;
    public float ability3Cooldown = 8f;
    bool ability3OnCooldown;
    public KeyCode ability3KeyCode;

    void Start()
    {
        ability1Image.fillAmount = 0;
        ability2Image.fillAmount = 0;
        ability3Image.fillAmount = 0;

        ability1Indicator.GetComponent<Image>().enabled = false;
        ability2AoETargetIndicator.GetComponent<Image>().enabled = false;
        ability2RangeIndicator.GetComponent<Image>().enabled = false;
    }

    void Update()
    {
        Ability1();
        Ability2();
        Ability3();

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Ability 1 inputs
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
        }

        // Ability 1 Canvas inputs
        Quaternion transRot = Quaternion.LookRotation(position - player.transform.position);
        transRot.eulerAngles = new Vector3(-90, transRot.eulerAngles.y, 90);
        ability1Canvas.transform.rotation = Quaternion.Lerp(transRot, ability1Canvas.transform.rotation, 0f);



        // Ability 2 Canvas Inputs
        Vector3 hitPosDir = (hit.point - transform.position).normalized;
        float distance = Vector3.Distance(hit.point, transform.position);
        distance = Mathf.Min(distance, ability2MaxDistance);

        Vector3 newHitPos = transform.position + hitPosDir * distance;
        ability2AoETargetIndicator.transform.position = newHitPos;
        ability2AoETargetIndicator.transform.position = new Vector3(
            ability2AoETargetIndicator.transform.position.x,
            1.5f,
            ability2AoETargetIndicator.transform.position.z
            );
    }

    void Ability1()
    {
        if (Input.GetKey(ability1KeyCode) && !ability1OnCooldown)
        {
            ability1Indicator.GetComponent<Image>().enabled = true;

            //Disable Other UI
            ability2AoETargetIndicator.GetComponent<Image>().enabled = false;
            ability2RangeIndicator.GetComponent<Image>().enabled = false;
        }

        if (ability1Indicator.GetComponent<Image>().enabled && Input.GetMouseButtonDown(0))
        {
            ability1OnCooldown = true;
            ability1Image.fillAmount = 1;
        }

        if (ability1OnCooldown)
        {
            ability1Image.fillAmount -= 1 / ability1Cooldown * Time.deltaTime;
            ability1Indicator.GetComponent<Image>().enabled = false;

            if (ability1Image.fillAmount <= 0)
            {
                ability1Image.fillAmount = 0;
                ability1OnCooldown = false;
            }
        }
    }

    void Ability2()
    {
        if (Input.GetKey(ability2KeyCode) && ability2OnCooldown == false)
        {
            ability2RangeIndicator.GetComponent<Image>().enabled = true;
            ability2AoETargetIndicator.GetComponent<Image>().enabled = true;

            //Disable Skillshot UI
            ability1Indicator.GetComponent<Image>().enabled = false;
        }

        if (ability2AoETargetIndicator.GetComponent<Image>().enabled == true && Input.GetMouseButtonDown(0))
        {
            ability2OnCooldown = true;
            ability2Image.fillAmount = 1;
        }

        if (ability2OnCooldown)
        {
            ability2Image.fillAmount -= 1 / ability2Cooldown * Time.deltaTime;

            ability2RangeIndicator.GetComponent<Image>().enabled = false;
            ability2AoETargetIndicator.GetComponent<Image>().enabled = false;

            if (ability2Image.fillAmount <= 0)
            {
                ability2Image.fillAmount = 0;
                ability2OnCooldown = false;
            }
        }
    }

    void Ability3()
    {
        if (Input.GetKey(ability3KeyCode) && ability3OnCooldown == false)
        {
            ability3OnCooldown = true;
            ability3Image.fillAmount = 1;
        }

        if (ability3OnCooldown)
        {
            ability3Image.fillAmount -= 1 / ability3Cooldown * Time.deltaTime;

            if (ability3Image.fillAmount <= 0)
            {
                ability3Image.fillAmount = 0;
                ability3OnCooldown = false;
            }
        }
    }
}
