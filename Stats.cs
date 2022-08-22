using UnityEngine;

public class Stats : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public float attackDmg;
    public float attackSpeed;
    public float attackTime;

    HeroCombat heroCombatScript;

    private GameObject player;
    public float expValue;

    // Start is called before the first frame update
    void Start()
    {
        heroCombatScript = GameObject.FindGameObjectWithTag(EditorConstants.TAG_PLAYER).GetComponent<HeroCombat>();
        player = GameObject.FindGameObjectWithTag(EditorConstants.TAG_PLAYER);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            heroCombatScript.targetedEnemy = null;
            heroCombatScript.performMeleeAttack = false;

            //Give Exp
            player.GetComponent<LevelUpStats>().SetExperience(expValue);
        }
    }
}
