using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("UI References")]
    [SerializeField] Image healthbarImage;

    [SerializeField] GameObject healthUI;

    [Header("Health Stats")]
    const float maxHealth = 100f;
    public float currentHealth = maxHealth;
    public Coroutine healthRegeneration;
    [Range(0.0f, 10.0f)]
    public float healthRegen = 0.1f;
    public int healthRegenDelay = 2;

    [SerializeField]
    private Animator Animator;
    private const string Dead = "Dead";
    public bool IsAlive;

    public EnemyMovement enemyMovement;
    // Start is called before the first frame update
    void Start()
    {
        IsAlive = IsAlive;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        RPC_TakeDamage(damage); //PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    //[PunRPC]
    void RPC_TakeDamage(float damage)//, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        //if (currentHealth <= 30)
        //healthbarImage.color = new Color32(240, 0, 0, 140);

        if (healthRegeneration != null)
            StopCoroutine(healthRegeneration);

        healthRegeneration = StartCoroutine(HealthRegen());

        if (currentHealth <= 0)
        {
            //Finds Player weapon item index to correctly assign image of the weapon that killed them in the KillFeed
            //int howIndex = PlayerManager.Find(info.Sender).weaponIndex;

            //Handles sending kill data to Player Manager for Scoreboard
            //PlayerManager.Find(info.Sender).GetKill();

            //Finds Player Nickname for KillFeed
            //string killer = PlayerManager.Find(info.Sender).GetComponent<PhotonView>().Owner.NickName.ToString();

            //Calls RPC for KillFeed and sends data
            //KillListWithImage(killer, PV.Owner.NickName, howIndex); //, info);

            //PV.RPC("RPC_KillListWithHow", RpcTarget.All, killer, PV.Owner.NickName, "killed"); 
            StopCoroutine(healthRegeneration);
            healthUI.SetActive(false);

            Die(); //Kills player as normal
        }
        Debug.Log("took damage: " + damage);
    }

    private IEnumerator HealthRegen()
    {
        yield return new WaitForSeconds(healthRegenDelay);

        while (currentHealth < maxHealth)
        {
            currentHealth += maxHealth / 100;
            healthbarImage.fillAmount = currentHealth / maxHealth;
            //if (currentHealth >= 31)
            //healthbarImage.color = new Color32(255, 255, 255, 140);
            yield return new WaitForSeconds(healthRegen);
        }
        healthRegeneration = null;
    }

    void Die()
    {
        IsAlive = !IsAlive;
        Animator.SetTrigger(Dead);
        enemyMovement.Die();

        Destroy(this);

    }
}
